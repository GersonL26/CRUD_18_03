using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Infrastructure.AI;

/// <summary>
/// Analizador simulado para desarrollo y testing.
/// Reemplazar con AnalizadorOpenAI, AnalizadorGemini, etc.
/// sin modificar AnalisisService ni ningún otro componente (Open/Closed Principle).
/// </summary>
public class AnalizadorSimulado : IAnalizadorIA
{
    public Task<ResultadoAnalisisIA> AnalizarRespuestasAsync(
        List<PreguntaConRespuesta> preguntasYRespuestas,
        string tecnologia,
        NivelTecnico nivel)
    {
        var detalles = preguntasYRespuestas.Select(item =>
        {
            var longitudRespuesta = item.RespuestaDelCandidato.Length;
            var score = CalcularScoreSimulado(longitudRespuesta, item.PuntajeMaximo);

            return new ScorePorPregunta
            {
                PreguntaId = item.PreguntaId,
                Score = score,
                Feedback = GenerarFeedback(score, item.PuntajeMaximo),
                Brechas = score < item.PuntajeMaximo * 0.6
                    ? $"Respuesta insuficiente para: {item.TextoPregunta[..Math.Min(50, item.TextoPregunta.Length)]}"
                    : string.Empty
            };
        }).ToList();

        var scoreTotal = detalles.Count > 0
            ? detalles.Average(d => d.Score / preguntasYRespuestas
                .First(p => p.PreguntaId == d.PreguntaId).PuntajeMaximo * 100)
            : 0;

        var resultado = new ResultadoAnalisisIA
        {
            ScoreTotal = Math.Round(scoreTotal, 2),
            Recomendacion = GenerarRecomendacion(scoreTotal),
            ResumenGeneral = $"Análisis simulado para candidato en {tecnologia} nivel {nivel}. " +
                             $"Se evaluaron {preguntasYRespuestas.Count} preguntas. " +
                             $"Score general: {scoreTotal:F1}/100.",
            Brechas = string.Join(", ", detalles
                .Where(d => !string.IsNullOrEmpty(d.Brechas))
                .Select(d => d.Brechas)),
            Fortalezas = string.Join(", ", detalles
                .Where(d => d.Score >= preguntasYRespuestas
                    .First(p => p.PreguntaId == d.PreguntaId).PuntajeMaximo * 0.7)
                .Select(d => $"Buen manejo en pregunta {preguntasYRespuestas
                    .First(p => p.PreguntaId == d.PreguntaId).TextoPregunta[..Math.Min(40, preguntasYRespuestas
                    .First(p => p.PreguntaId == d.PreguntaId).TextoPregunta.Length)]}"))
        };

        resultado.DetallesPorPregunta = detalles;

        return Task.FromResult(resultado);
    }

    private static double CalcularScoreSimulado(int longitudRespuesta, int puntajeMaximo)
    {
        // Heurística simple: respuestas más largas tienden a ser más completas
        var factor = Math.Min(longitudRespuesta / 100.0, 1.0);
        return Math.Round(puntajeMaximo * (0.4 + factor * 0.5), 2);
    }

    private static string GenerarFeedback(double score, int puntajeMaximo)
    {
        var porcentaje = score / puntajeMaximo * 100;
        return porcentaje switch
        {
            >= 80 => "Respuesta completa y bien estructurada.",
            >= 60 => "Respuesta aceptable, pero podría profundizar más.",
            >= 40 => "Respuesta parcial, falta desarrollo en conceptos clave.",
            _ => "Respuesta insuficiente, se requiere mayor conocimiento del tema."
        };
    }

    private static string GenerarRecomendacion(double scoreTotal)
    {
        return scoreTotal switch
        {
            >= 80 => "Contratar",
            >= 60 => "Segunda entrevista",
            >= 40 => "Reserva - Necesita refuerzo",
            _ => "No contratar"
        };
    }
}
