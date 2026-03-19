using CRUD_18_03.Application.DTOs.ResultadoEvaluacion;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Domain.Entities;
using CRUD_18_03.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Infrastructure.Services;

public class ResultadoService : IResultadoService
{
    private readonly ApplicationDbContext _dbContext;

    public ResultadoService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResultadoCompletoDto?> ObtenerResultadoAsync(Guid candidatoId, Guid evaluadorId)
    {
        var candidato = await ObtenerCandidatoConDatos(candidatoId);
        ValidarPropietario(candidato.Evaluacion!, evaluadorId);

        if (candidato.Resultado is null)
            return null;

        return MapToCompletoDto(candidato, candidato.Resultado);
    }

    public async Task<ResumenEvaluacionResultadosDto> ObtenerResumenEvaluacionAsync(Guid evaluacionId, Guid evaluadorId)
    {
        var evaluacion = await _dbContext.Evaluaciones
            .Include(e => e.Candidatos.Where(c => c.EstaActivo))
                .ThenInclude(c => c.Resultado)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == evaluacionId && e.EstaActivo)
            ?? throw new KeyNotFoundException($"Evaluación con ID '{evaluacionId}' no encontrada.");

        ValidarPropietario(evaluacion, evaluadorId);

        var candidatosDto = evaluacion.Candidatos
            .OrderByDescending(c => c.Resultado?.ScoreTotal)
            .ThenBy(c => c.Nombre)
            .Select(c => MapToResumenCandidato(c))
            .ToList();

        var analizados = candidatosDto.Where(c => c.ScoreTotal.HasValue).ToList();

        return new ResumenEvaluacionResultadosDto
        {
            EvaluacionId = evaluacion.Id,
            Titulo = evaluacion.Titulo,
            Tecnologia = evaluacion.Tecnologia,
            Nivel = evaluacion.Nivel.ToString(),
            TotalCandidatos = candidatosDto.Count,
            CandidatosRespondieron = evaluacion.Candidatos.Count(c => c.FechaFinRespuesta.HasValue),
            CandidatosAnalizados = analizados.Count,
            ScorePromedio = analizados.Count > 0
                ? Math.Round(analizados.Average(c => c.ScoreTotal!.Value), 2)
                : null,
            Candidatos = candidatosDto
        };
    }

    public async Task<ResultadoCandidatoPublicoDto> ObtenerResultadoPorTokenAsync(string token)
    {
        var candidato = await _dbContext.Candidatos
            .Include(c => c.Evaluacion!)
                .ThenInclude(e => e.Preguntas.Where(p => p.EstaActivo))
            .Include(c => c.Respuestas.Where(r => r.EstaActivo))
            .Include(c => c.Resultado)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Token == token && c.EstaActivo)
            ?? throw new KeyNotFoundException("Token de candidato inválido.");

        if (candidato.Resultado is null)
            throw new InvalidOperationException("El análisis aún no ha sido generado.");

        var evaluacion = candidato.Evaluacion!;
        var resultado = candidato.Resultado;

        return new ResultadoCandidatoPublicoDto
        {
            CandidatoNombre = candidato.Nombre,
            Tecnologia = evaluacion.Tecnologia,
            Nivel = evaluacion.Nivel.ToString(),
            TituloEvaluacion = evaluacion.Titulo,
            ScoreTotal = resultado.ScoreTotal,
            Recomendacion = resultado.Recomendacion,
            ResumenIA = resultado.ResumenIA,
            Fortalezas = SplitLista(resultado.FortalezasDetectadas),
            Brechas = SplitLista(resultado.BrechasDetectadas),
            TiempoInvertido = ObtenerTiempoInvertido(candidato),
            GeneradoEn = resultado.GeneradoEn,
            Respuestas = evaluacion.Preguntas
                .Where(p => p.EstaActivo)
                .OrderBy(p => p.OrdenEnEvaluacion)
                .Select(p =>
                {
                    var respuesta = candidato.Respuestas.FirstOrDefault(r => r.PreguntaId == p.Id);
                    return new DetallePreguntaPublicoDto
                    {
                        OrdenEnEvaluacion = p.OrdenEnEvaluacion,
                        TextoPregunta = p.Texto,
                        PuntajeMaximo = p.PuntajeMaximo,
                        ScoreIA = respuesta?.ScoreIA,
                        FeedbackIA = respuesta?.FeedbackIA
                    };
                })
                .ToList()
        };
    }

    private async Task<Candidato> ObtenerCandidatoConDatos(Guid candidatoId)
    {
        return await _dbContext.Candidatos
            .Include(c => c.Evaluacion!)
                .ThenInclude(e => e.Preguntas.Where(p => p.EstaActivo))
            .Include(c => c.Respuestas.Where(r => r.EstaActivo))
            .Include(c => c.Resultado)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == candidatoId && c.EstaActivo)
            ?? throw new KeyNotFoundException($"Candidato con ID '{candidatoId}' no encontrado.");
    }

    private static void ValidarPropietario(Evaluacion evaluacion, Guid evaluadorId)
    {
        if (evaluacion.EvaluadorId != evaluadorId)
            throw new UnauthorizedAccessException("No tiene permiso para acceder a esta evaluación.");
    }

    private static ResumenCandidatoDto MapToResumenCandidato(Candidato c)
    {
        var estado = c.Resultado is not null ? "Analizado"
            : c.FechaFinRespuesta.HasValue ? "Respondió"
            : c.FechaInicioRespuesta.HasValue ? "En progreso"
            : "Pendiente";

        return new ResumenCandidatoDto
        {
            CandidatoId = c.Id,
            Nombre = c.Nombre,
            Email = c.Email,
            Estado = estado,
            ScoreTotal = c.Resultado?.ScoreTotal,
            Recomendacion = c.Resultado?.Recomendacion,
            TiempoInvertido = ObtenerTiempoInvertido(c),
            FechaFinRespuesta = c.FechaFinRespuesta,
            FechaAnalisis = c.Resultado?.GeneradoEn
        };
    }

    private static ResultadoCompletoDto MapToCompletoDto(Candidato candidato, ResultadoEvaluacion resultado)
    {
        var evaluacion = candidato.Evaluacion!;

        return new ResultadoCompletoDto
        {
            ResultadoId = resultado.Id,
            CandidatoNombre = candidato.Nombre,
            CandidatoEmail = candidato.Email,
            Tecnologia = evaluacion.Tecnologia,
            Nivel = evaluacion.Nivel.ToString(),
            TituloEvaluacion = evaluacion.Titulo,
            ScoreTotal = resultado.ScoreTotal,
            Recomendacion = resultado.Recomendacion,
            ResumenIA = resultado.ResumenIA,
            BrechasDetectadas = resultado.BrechasDetectadas,
            FortalezasDetectadas = resultado.FortalezasDetectadas,
            GeneradoEn = resultado.GeneradoEn,
            TiempoInvertido = ObtenerTiempoInvertido(candidato),
            Respuestas = evaluacion.Preguntas
                .Where(p => p.EstaActivo)
                .OrderBy(p => p.OrdenEnEvaluacion)
                .Select(pregunta =>
                {
                    var respuesta = candidato.Respuestas
                        .FirstOrDefault(r => r.PreguntaId == pregunta.Id);

                    return new DetalleRespuestaDto
                    {
                        PreguntaId = pregunta.Id,
                        TextoPregunta = pregunta.Texto,
                        OrdenEnEvaluacion = pregunta.OrdenEnEvaluacion,
                        PuntajeMaximo = pregunta.PuntajeMaximo,
                        ContenidoRespuesta = respuesta?.Contenido ?? "Sin respuesta",
                        TiempoUsadoSegundos = respuesta?.TiempoUsadoSegundos,
                        ScoreIA = respuesta?.ScoreIA,
                        FeedbackIA = respuesta?.FeedbackIA,
                        BrechasIdentificadas = respuesta?.BrechasIdentificadas
                    };
                })
                .ToList()
        };
    }

    private static string? ObtenerTiempoInvertido(Candidato c)
    {
        if (c.FechaInicioRespuesta.HasValue && c.FechaFinRespuesta.HasValue)
            return (c.FechaFinRespuesta.Value - c.FechaInicioRespuesta.Value).ToString(@"hh\:mm\:ss");
        return null;
    }

    private static List<string> SplitLista(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return new List<string>();

        return texto.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }
}
