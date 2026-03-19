using CRUD_18_03.Application.DTOs.ResultadoEvaluacion;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Domain.Entities;
using CRUD_18_03.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Infrastructure.Services;

public class AnalisisService : IAnalisisService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IAnalizadorIA _analizadorIA;

    public AnalisisService(ApplicationDbContext dbContext, IAnalizadorIA analizadorIA)
    {
        _dbContext = dbContext;
        _analizadorIA = analizadorIA;
    }

    public async Task<ResultadoCompletoDto> EjecutarAnalisisAsync(Guid candidatoId, Guid evaluadorId)
    {
        var candidato = await ObtenerCandidatoConDatos(candidatoId);

        ValidarPropietario(candidato.Evaluacion!, evaluadorId);

        if (!candidato.FechaFinRespuesta.HasValue)
            throw new InvalidOperationException("El candidato aún no ha enviado sus respuestas.");

        if (candidato.Resultado is not null)
            throw new InvalidOperationException("El análisis ya fue generado para este candidato.");

        // Preparar datos para el analizador
        var preguntasConRespuestas = candidato.Evaluacion!.Preguntas
            .Where(p => p.EstaActivo)
            .OrderBy(p => p.OrdenEnEvaluacion)
            .Select(pregunta =>
            {
                var respuesta = candidato.Respuestas
                    .FirstOrDefault(r => r.PreguntaId == pregunta.Id);

                return new PreguntaConRespuesta
                {
                    PreguntaId = pregunta.Id,
                    TextoPregunta = pregunta.Texto,
                    Rubrica = pregunta.Rubrica,
                    PuntajeMaximo = pregunta.PuntajeMaximo,
                    RespuestaDelCandidato = respuesta?.Contenido ?? "Sin respuesta"
                };
            })
            .ToList();

        // Ejecutar análisis IA
        var analisis = await _analizadorIA.AnalizarRespuestasAsync(
            preguntasConRespuestas,
            candidato.Evaluacion.Tecnologia,
            candidato.Evaluacion.Nivel);

        // Guardar resultado global
        var resultado = new ResultadoEvaluacion
        {
            ScoreTotal = analisis.ScoreTotal,
            Recomendacion = analisis.Recomendacion,
            ResumenIA = analisis.ResumenGeneral,
            BrechasDetectadas = analisis.Brechas,
            FortalezasDetectadas = analisis.Fortalezas,
            CandidatoId = candidatoId,
            GeneradoEn = DateTime.UtcNow
        };

        _dbContext.ResultadosEvaluacion.Add(resultado);

        // Actualizar scores individuales en cada respuesta
        foreach (var detalle in analisis.DetallesPorPregunta)
        {
            var respuesta = candidato.Respuestas
                .FirstOrDefault(r => r.PreguntaId == detalle.PreguntaId);

            if (respuesta is not null)
            {
                respuesta.ScoreIA = detalle.Score;
                respuesta.FeedbackIA = detalle.Feedback;
                respuesta.BrechasIdentificadas = detalle.Brechas;
                respuesta.ModificadoEn = DateTime.UtcNow;
            }
        }

        await _dbContext.SaveChangesAsync();

        return MapToCompletoDto(candidato, resultado);
    }

    public async Task<ResultadoCompletoDto?> ObtenerResultadoAsync(Guid candidatoId, Guid evaluadorId)
    {
        var candidato = await ObtenerCandidatoConDatos(candidatoId);

        ValidarPropietario(candidato.Evaluacion!, evaluadorId);

        if (candidato.Resultado is null)
            return null;

        return MapToCompletoDto(candidato, candidato.Resultado);
    }

    // --- Helpers privados ---

    private async Task<Candidato> ObtenerCandidatoConDatos(Guid candidatoId)
    {
        return await _dbContext.Candidatos
            .Include(c => c.Evaluacion!)
                .ThenInclude(e => e.Preguntas.Where(p => p.EstaActivo))
            .Include(c => c.Respuestas.Where(r => r.EstaActivo))
            .Include(c => c.Resultado)
            .FirstOrDefaultAsync(c => c.Id == candidatoId && c.EstaActivo)
            ?? throw new KeyNotFoundException($"Candidato con ID '{candidatoId}' no encontrado.");
    }

    private static void ValidarPropietario(Evaluacion evaluacion, Guid evaluadorId)
    {
        if (evaluacion.EvaluadorId != evaluadorId)
            throw new UnauthorizedAccessException("No tiene permiso para acceder a esta evaluación.");
    }

    private static ResultadoCompletoDto MapToCompletoDto(Candidato candidato, ResultadoEvaluacion resultado)
    {
        var evaluacion = candidato.Evaluacion!;
        TimeSpan? tiempoInvertido = null;

        if (candidato.FechaInicioRespuesta.HasValue && candidato.FechaFinRespuesta.HasValue)
            tiempoInvertido = candidato.FechaFinRespuesta.Value - candidato.FechaInicioRespuesta.Value;

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
            TiempoInvertido = tiempoInvertido?.ToString(@"hh\:mm\:ss"),
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
}
