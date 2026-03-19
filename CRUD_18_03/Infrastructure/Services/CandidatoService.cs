using CRUD_18_03.Application.DTOs.Candidato;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Domain.Entities;
using CRUD_18_03.Domain.Enums;
using CRUD_18_03.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Infrastructure.Services;

public class CandidatoService : ICandidatoService
{
    private readonly ApplicationDbContext _dbContext;

    public CandidatoService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // --- Evaluador: gestión de candidatos ---

    public async Task<CandidatoDto> InvitarAsync(Guid evaluacionId, InvitarCandidatoDto dto, Guid evaluadorId)
    {
        var evaluacion = await _dbContext.Evaluaciones
            .FirstOrDefaultAsync(e => e.Id == evaluacionId && e.EstaActivo)
            ?? throw new KeyNotFoundException($"Evaluación con ID '{evaluacionId}' no encontrada.");

        if (evaluacion.EvaluadorId != evaluadorId)
            throw new UnauthorizedAccessException("No tiene permiso para acceder a esta evaluación.");

        if (evaluacion.Estado != EstadoEvaluacion.Activa)
            throw new InvalidOperationException("Solo se pueden invitar candidatos a evaluaciones en estado Activa.");

        var emailDuplicado = await _dbContext.Candidatos
            .AnyAsync(c => c.EvaluacionId == evaluacionId && c.Email == dto.Email && c.EstaActivo);

        if (emailDuplicado)
            throw new InvalidOperationException($"Ya existe un candidato con el email '{dto.Email}' en esta evaluación.");

        var candidato = new Candidato
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            EvaluacionId = evaluacionId
        };

        _dbContext.Candidatos.Add(candidato);
        await _dbContext.SaveChangesAsync();

        return MapToDto(candidato);
    }

    public async Task<IEnumerable<CandidatoDto>> ListarPorEvaluacionAsync(Guid evaluacionId, Guid evaluadorId)
    {
        var evaluacion = await _dbContext.Evaluaciones
            .FirstOrDefaultAsync(e => e.Id == evaluacionId && e.EstaActivo)
            ?? throw new KeyNotFoundException($"Evaluación con ID '{evaluacionId}' no encontrada.");

        if (evaluacion.EvaluadorId != evaluadorId)
            throw new UnauthorizedAccessException("No tiene permiso para acceder a esta evaluación.");

        var candidatos = await _dbContext.Candidatos
            .Where(c => c.EvaluacionId == evaluacionId && c.EstaActivo)
            .AsNoTracking()
            .OrderByDescending(c => c.CreadoEn)
            .ToListAsync();

        return candidatos.Select(MapToDto);
    }

    public async Task EliminarAsync(Guid evaluacionId, Guid candidatoId, Guid evaluadorId)
    {
        var evaluacion = await _dbContext.Evaluaciones
            .FirstOrDefaultAsync(e => e.Id == evaluacionId && e.EstaActivo)
            ?? throw new KeyNotFoundException($"Evaluación con ID '{evaluacionId}' no encontrada.");

        if (evaluacion.EvaluadorId != evaluadorId)
            throw new UnauthorizedAccessException("No tiene permiso para acceder a esta evaluación.");

        var candidato = await _dbContext.Candidatos
            .FirstOrDefaultAsync(c => c.Id == candidatoId && c.EvaluacionId == evaluacionId && c.EstaActivo)
            ?? throw new KeyNotFoundException($"Candidato con ID '{candidatoId}' no encontrado en esta evaluación.");

        if (candidato.FechaFinRespuesta.HasValue)
            throw new InvalidOperationException("No se puede eliminar un candidato que ya envió sus respuestas.");

        candidato.EstaActivo = false;
        candidato.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    // --- Candidato: acceso público con token ---

    public async Task<EvaluacionCandidatoDto> ObtenerPorTokenAsync(string token)
    {
        var candidato = await _dbContext.Candidatos
            .Include(c => c.Evaluacion!)
                .ThenInclude(e => e.Preguntas.Where(p => p.EstaActivo))
            .FirstOrDefaultAsync(c => c.Token == token && c.EstaActivo)
            ?? throw new KeyNotFoundException("Token de candidato inválido.");

        var evaluacion = candidato.Evaluacion!;

        return new EvaluacionCandidatoDto
        {
            EvaluacionId = evaluacion.Id,
            Titulo = evaluacion.Titulo,
            Descripcion = evaluacion.Descripcion,
            Tecnologia = evaluacion.Tecnologia,
            Nivel = evaluacion.Nivel,
            TiempoLimiteTotalMinutos = evaluacion.TiempoLimiteTotalMinutos,
            RequiereCamara = evaluacion.RequiereCamara,
            RequiereMicrofono = evaluacion.RequiereMicrofono,
            CandidatoId = candidato.Id,
            NombreCandidato = candidato.Nombre,
            FechaInicioRespuesta = candidato.FechaInicioRespuesta,
            YaRespondio = candidato.FechaFinRespuesta.HasValue,
            Preguntas = evaluacion.Preguntas
                .OrderBy(p => p.OrdenEnEvaluacion)
                .Select(p => new PreguntaCandidatoDto
                {
                    Id = p.Id,
                    Texto = p.Texto,
                    Tipo = p.Tipo,
                    PuntajeMaximo = p.PuntajeMaximo,
                    OrdenEnEvaluacion = p.OrdenEnEvaluacion,
                    TiempoLimiteSegundos = p.TiempoLimiteSegundos
                })
                .ToList()
        };
    }

    public async Task IniciarRespuestaAsync(string token)
    {
        var candidato = await _dbContext.Candidatos
            .Include(c => c.Evaluacion)
            .FirstOrDefaultAsync(c => c.Token == token && c.EstaActivo)
            ?? throw new KeyNotFoundException("Token de candidato inválido.");

        if (candidato.Evaluacion!.Estado != EstadoEvaluacion.Activa)
            throw new InvalidOperationException("La evaluación no está activa.");

        if (candidato.FechaFinRespuesta.HasValue)
            throw new InvalidOperationException("El candidato ya envió sus respuestas.");

        if (!candidato.FechaInicioRespuesta.HasValue)
        {
            candidato.FechaInicioRespuesta = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task EnviarRespuestasAsync(string token, EnviarRespuestasDto dto)
    {
        var candidato = await _dbContext.Candidatos
            .Include(c => c.Evaluacion!)
                .ThenInclude(e => e.Preguntas.Where(p => p.EstaActivo))
            .Include(c => c.Respuestas)
            .FirstOrDefaultAsync(c => c.Token == token && c.EstaActivo)
            ?? throw new KeyNotFoundException("Token de candidato inválido.");

        if (candidato.Evaluacion!.Estado != EstadoEvaluacion.Activa)
            throw new InvalidOperationException("La evaluación no está activa.");

        if (candidato.FechaFinRespuesta.HasValue)
            throw new InvalidOperationException("El candidato ya envió sus respuestas.");

        if (candidato.Respuestas.Count > 0)
            throw new InvalidOperationException("El candidato ya envió sus respuestas.");

        // Validar que las preguntas respondidas pertenecen a la evaluación
        var preguntaIds = candidato.Evaluacion.Preguntas.Select(p => p.Id).ToHashSet();
        foreach (var respuesta in dto.Respuestas)
        {
            if (!preguntaIds.Contains(respuesta.PreguntaId))
                throw new InvalidOperationException($"La pregunta '{respuesta.PreguntaId}' no pertenece a esta evaluación.");
        }

        // Registrar inicio si no se hizo
        if (!candidato.FechaInicioRespuesta.HasValue)
            candidato.FechaInicioRespuesta = DateTime.UtcNow;

        // Crear respuestas con timestamp UTC
        var ahora = DateTime.UtcNow;
        foreach (var item in dto.Respuestas)
        {
            _dbContext.Respuestas.Add(new Respuesta
            {
                Contenido = item.Contenido,
                Timestamp = ahora,
                TiempoUsadoSegundos = item.TiempoUsadoSegundos,
                CandidatoId = candidato.Id,
                PreguntaId = item.PreguntaId
            });
        }

        // Finalizar
        candidato.FechaFinRespuesta = ahora;
        candidato.ModificadoEn = ahora;
        await _dbContext.SaveChangesAsync();
    }

    // --- Helpers ---

    private static CandidatoDto MapToDto(Candidato c) => new()
    {
        Id = c.Id,
        Nombre = c.Nombre,
        Email = c.Email,
        Token = c.Token,
        FechaInicioRespuesta = c.FechaInicioRespuesta,
        FechaFinRespuesta = c.FechaFinRespuesta,
        EvaluacionId = c.EvaluacionId,
        CreadoEn = c.CreadoEn,
        EstaActivo = c.EstaActivo
    };
}
