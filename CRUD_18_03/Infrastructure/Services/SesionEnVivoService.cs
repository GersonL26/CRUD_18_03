using CRUD_18_03.Application.DTOs.SesionEnVivo;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Domain.Entities;
using CRUD_18_03.Domain.Enums;
using CRUD_18_03.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Infrastructure.Services;

public class SesionEnVivoService : ISesionEnVivoService
{
    private readonly ApplicationDbContext _dbContext;

    public SesionEnVivoService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EstadoSesionDto> CrearSesionAsync(Guid evaluacionId, Guid candidatoId, Guid evaluadorId)
    {
        var evaluacion = await _dbContext.Evaluaciones
            .Include(e => e.Preguntas.Where(p => p.EstaActivo))
            .FirstOrDefaultAsync(e => e.Id == evaluacionId && e.EstaActivo)
            ?? throw new KeyNotFoundException($"Evaluación con ID '{evaluacionId}' no encontrada.");

        if (evaluacion.EvaluadorId != evaluadorId)
            throw new UnauthorizedAccessException("No tiene permiso para esta evaluación.");

        if (evaluacion.Estado != EstadoEvaluacion.Activa)
            throw new InvalidOperationException("La evaluación debe estar activa.");

        var candidato = await _dbContext.Candidatos
            .FirstOrDefaultAsync(c => c.Id == candidatoId && c.EvaluacionId == evaluacionId && c.EstaActivo)
            ?? throw new KeyNotFoundException($"Candidato con ID '{candidatoId}' no encontrado en esta evaluación.");

        if (candidato.FechaFinRespuesta.HasValue)
            throw new InvalidOperationException("El candidato ya finalizó sus respuestas.");

        var sesionExistente = await _dbContext.SesionesEnVivo
            .FirstOrDefaultAsync(s => s.CandidatoId == candidatoId && s.EvaluacionId == evaluacionId && s.EstaActivo);

        if (sesionExistente is not null)
            throw new InvalidOperationException("Ya existe una sesión activa para este candidato.");

        var sesion = new SesionEnVivo
        {
            EvaluacionId = evaluacionId,
            CandidatoId = candidatoId
        };

        _dbContext.SesionesEnVivo.Add(sesion);
        await _dbContext.SaveChangesAsync();

        var preguntas = evaluacion.Preguntas.Where(p => p.EstaActivo).OrderBy(p => p.OrdenEnEvaluacion).ToList();

        return MapToEstado(sesion, candidato.Nombre, preguntas);
    }

    public async Task<EstadoSesionDto> ObtenerEstadoAsync(Guid sesionId)
    {
        var sesion = await ObtenerSesionConDatos(sesionId);
        var preguntas = ObtenerPreguntasOrdenadas(sesion);

        return MapToEstado(sesion, sesion.Candidato!.Nombre, preguntas);
    }

    public async Task<PreguntaEnVivoDto> IniciarSesionAsync(Guid sesionId, string tokenCandidato)
    {
        var sesion = await ObtenerSesionConDatos(sesionId);
        ValidarTokenCandidato(sesion, tokenCandidato);

        if (sesion.SesionActiva)
            throw new InvalidOperationException("La sesión ya fue iniciada.");

        if (sesion.FueCompletada)
            throw new InvalidOperationException("La sesión ya fue completada.");

        sesion.SesionActiva = true;
        sesion.InicioFaseActual = DateTime.UtcNow;
        sesion.ModificadoEn = DateTime.UtcNow;

        if (!sesion.Candidato!.FechaInicioRespuesta.HasValue)
            sesion.Candidato.FechaInicioRespuesta = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var preguntas = ObtenerPreguntasOrdenadas(sesion);
        return MapToPreguntaDto(preguntas[0], 0, preguntas.Count);
    }

    public async Task<PreguntaEnVivoDto> ObtenerPreguntaActualAsync(Guid sesionId, string tokenCandidato)
    {
        var sesion = await ObtenerSesionConDatos(sesionId);
        ValidarTokenCandidato(sesion, tokenCandidato);
        ValidarSesionActiva(sesion);

        var preguntas = ObtenerPreguntasOrdenadas(sesion);
        var preguntaActual = preguntas[sesion.PreguntaActualIndex];

        return MapToPreguntaDto(preguntaActual, sesion.PreguntaActualIndex, preguntas.Count);
    }

    public async Task<PreguntaEnVivoDto?> ResponderYAvanzarAsync(
        Guid sesionId, string tokenCandidato, ResponderPreguntaEnVivoDto dto)
    {
        var sesion = await ObtenerSesionConDatos(sesionId);
        ValidarTokenCandidato(sesion, tokenCandidato);
        ValidarSesionActiva(sesion);

        var preguntas = ObtenerPreguntasOrdenadas(sesion);
        var preguntaActual = preguntas[sesion.PreguntaActualIndex];

        if (dto.PreguntaId != preguntaActual.Id)
            throw new InvalidOperationException("La pregunta respondida no corresponde a la pregunta actual.");

        var respuestaExistente = await _dbContext.Respuestas
            .FirstOrDefaultAsync(r => r.CandidatoId == sesion.CandidatoId && r.PreguntaId == dto.PreguntaId && r.EstaActivo);

        if (respuestaExistente is not null)
            throw new InvalidOperationException("Esta pregunta ya fue respondida.");

        _dbContext.Respuestas.Add(new Respuesta
        {
            Contenido = dto.Contenido,
            Timestamp = DateTime.UtcNow,
            TiempoUsadoSegundos = dto.TiempoUsadoSegundos,
            CandidatoId = sesion.CandidatoId,
            PreguntaId = dto.PreguntaId
        });

        var esUltima = sesion.PreguntaActualIndex >= preguntas.Count - 1;

        if (esUltima)
        {
            sesion.FueCompletada = true;
            sesion.SesionActiva = false;
            sesion.Candidato!.FechaFinRespuesta = DateTime.UtcNow;
            sesion.Candidato.ModificadoEn = DateTime.UtcNow;
        }
        else
        {
            sesion.PreguntaActualIndex++;
            sesion.InicioFaseActual = DateTime.UtcNow;
        }

        sesion.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        if (esUltima)
            return null;

        return MapToPreguntaDto(preguntas[sesion.PreguntaActualIndex], sesion.PreguntaActualIndex, preguntas.Count);
    }

    public async Task FinalizarSesionAsync(Guid sesionId, string tokenCandidato)
    {
        var sesion = await ObtenerSesionConDatos(sesionId);
        ValidarTokenCandidato(sesion, tokenCandidato);

        if (sesion.FueCompletada)
            throw new InvalidOperationException("La sesión ya fue completada.");

        sesion.FueCompletada = true;
        sesion.SesionActiva = false;
        sesion.ModificadoEn = DateTime.UtcNow;

        if (!sesion.Candidato!.FechaFinRespuesta.HasValue)
        {
            sesion.Candidato.FechaFinRespuesta = DateTime.UtcNow;
            sesion.Candidato.ModificadoEn = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<SesionEnVivo> ObtenerSesionConDatos(Guid sesionId)
    {
        return await _dbContext.SesionesEnVivo
            .Include(s => s.Evaluacion!)
                .ThenInclude(e => e.Preguntas.Where(p => p.EstaActivo))
            .Include(s => s.Candidato)
            .FirstOrDefaultAsync(s => s.Id == sesionId && s.EstaActivo)
            ?? throw new KeyNotFoundException($"Sesión con ID '{sesionId}' no encontrada.");
    }

    private async Task<SesionEnVivo> ObtenerSesionConDatosYValidarEvaluador(Guid sesionId, Guid evaluadorId)
    {
        var sesion = await ObtenerSesionConDatos(sesionId);
        if (sesion.Evaluacion!.EvaluadorId != evaluadorId)
            throw new UnauthorizedAccessException("No tiene permiso sobre esta sesión.");
        return sesion;
    }

    // ── Evaluator-driven methods (in-person interview) ──

    public async Task<PreguntaEnVivoDto> IniciarSesionPorEvaluadorAsync(Guid sesionId, Guid evaluadorId)
    {
        var sesion = await ObtenerSesionConDatosYValidarEvaluador(sesionId, evaluadorId);

        if (sesion.SesionActiva)
            throw new InvalidOperationException("La sesión ya fue iniciada.");
        if (sesion.FueCompletada)
            throw new InvalidOperationException("La sesión ya fue completada.");

        sesion.SesionActiva = true;
        sesion.InicioFaseActual = DateTime.UtcNow;
        sesion.ModificadoEn = DateTime.UtcNow;

        if (!sesion.Candidato!.FechaInicioRespuesta.HasValue)
            sesion.Candidato.FechaInicioRespuesta = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var preguntas = ObtenerPreguntasOrdenadas(sesion);
        return MapToPreguntaDto(preguntas[0], 0, preguntas.Count);
    }

    public async Task<PreguntaEnVivoDto> ObtenerPreguntaActualPorEvaluadorAsync(Guid sesionId, Guid evaluadorId)
    {
        var sesion = await ObtenerSesionConDatosYValidarEvaluador(sesionId, evaluadorId);
        ValidarSesionActiva(sesion);

        var preguntas = ObtenerPreguntasOrdenadas(sesion);
        return MapToPreguntaDto(preguntas[sesion.PreguntaActualIndex], sesion.PreguntaActualIndex, preguntas.Count);
    }

    public async Task<PreguntaEnVivoDto?> ResponderYAvanzarPorEvaluadorAsync(
        Guid sesionId, Guid evaluadorId, ResponderPreguntaEnVivoDto dto)
    {
        var sesion = await ObtenerSesionConDatosYValidarEvaluador(sesionId, evaluadorId);
        ValidarSesionActiva(sesion);

        var preguntas = ObtenerPreguntasOrdenadas(sesion);
        var preguntaActual = preguntas[sesion.PreguntaActualIndex];

        if (dto.PreguntaId != preguntaActual.Id)
            throw new InvalidOperationException("La pregunta respondida no corresponde a la pregunta actual.");

        var respuestaExistente = await _dbContext.Respuestas
            .FirstOrDefaultAsync(r => r.CandidatoId == sesion.CandidatoId && r.PreguntaId == dto.PreguntaId && r.EstaActivo);

        if (respuestaExistente is not null)
            throw new InvalidOperationException("Esta pregunta ya fue respondida.");

        _dbContext.Respuestas.Add(new Respuesta
        {
            Contenido = dto.Contenido,
            Timestamp = DateTime.UtcNow,
            TiempoUsadoSegundos = dto.TiempoUsadoSegundos,
            CandidatoId = sesion.CandidatoId,
            PreguntaId = dto.PreguntaId
        });

        var esUltima = sesion.PreguntaActualIndex >= preguntas.Count - 1;

        if (esUltima)
        {
            sesion.FueCompletada = true;
            sesion.SesionActiva = false;
            sesion.Candidato!.FechaFinRespuesta = DateTime.UtcNow;
            sesion.Candidato.ModificadoEn = DateTime.UtcNow;
        }
        else
        {
            sesion.PreguntaActualIndex++;
            sesion.InicioFaseActual = DateTime.UtcNow;
        }

        sesion.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        if (esUltima)
            return null;

        return MapToPreguntaDto(preguntas[sesion.PreguntaActualIndex], sesion.PreguntaActualIndex, preguntas.Count);
    }

    public async Task FinalizarSesionPorEvaluadorAsync(Guid sesionId, Guid evaluadorId)
    {
        var sesion = await ObtenerSesionConDatosYValidarEvaluador(sesionId, evaluadorId);

        if (sesion.FueCompletada)
            throw new InvalidOperationException("La sesión ya fue completada.");

        sesion.FueCompletada = true;
        sesion.SesionActiva = false;
        sesion.ModificadoEn = DateTime.UtcNow;

        if (!sesion.Candidato!.FechaFinRespuesta.HasValue)
        {
            sesion.Candidato.FechaFinRespuesta = DateTime.UtcNow;
            sesion.Candidato.ModificadoEn = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }

    private static void ValidarTokenCandidato(SesionEnVivo sesion, string token)
    {
        if (sesion.Candidato!.Token != token)
            throw new UnauthorizedAccessException("Token de candidato inválido para esta sesión.");
    }

    private static void ValidarSesionActiva(SesionEnVivo sesion)
    {
        if (!sesion.SesionActiva)
            throw new InvalidOperationException("La sesión no está activa.");

        if (sesion.FueCompletada)
            throw new InvalidOperationException("La sesión ya fue completada.");
    }

    private static List<Pregunta> ObtenerPreguntasOrdenadas(SesionEnVivo sesion)
    {
        return sesion.Evaluacion!.Preguntas
            .Where(p => p.EstaActivo)
            .OrderBy(p => p.OrdenEnEvaluacion)
            .ToList();
    }

    private static EstadoSesionDto MapToEstado(SesionEnVivo sesion, string nombreCandidato, List<Pregunta> preguntas)
    {
        var preguntaActual = sesion.PreguntaActualIndex < preguntas.Count
            ? preguntas[sesion.PreguntaActualIndex]
            : preguntas.Last();

        var segundosTranscurridos = sesion.InicioFaseActual.HasValue
            ? (int)(DateTime.UtcNow - sesion.InicioFaseActual.Value).TotalSeconds
            : 0;

        return new EstadoSesionDto
        {
            SesionId = sesion.Id,
            EvaluacionId = sesion.EvaluacionId,
            CandidatoId = sesion.CandidatoId,
            NombreCandidato = nombreCandidato,
            PreguntaActualIndex = sesion.PreguntaActualIndex,
            TotalPreguntas = preguntas.Count,
            SegundosTranscurridos = segundosTranscurridos,
            TiempoLimitePreguntaSegundos = preguntaActual.TiempoLimiteSegundos,
            SesionActiva = sesion.SesionActiva,
            FueCompletada = sesion.FueCompletada
        };
    }

    private static PreguntaEnVivoDto MapToPreguntaDto(Pregunta pregunta, int index, int total)
    {
        return new PreguntaEnVivoDto
        {
            PreguntaId = pregunta.Id,
            Index = index,
            Texto = pregunta.Texto,
            Tipo = (int)pregunta.Tipo,
            PuntajeMaximo = pregunta.PuntajeMaximo,
            TiempoLimiteSegundos = pregunta.TiempoLimiteSegundos,
            TotalPreguntas = total
        };
    }
}
