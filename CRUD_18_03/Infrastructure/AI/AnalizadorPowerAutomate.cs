using System.Text;
using System.Text.Json;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Infrastructure.AI;

/// <summary>
/// Analizador que envía prompts al flujo de Power Automate.
/// El flujo espera: POST { "prompt": "..." } y devuelve JSON con score + justificacion.
/// Implementa IAnalizadorIA (Open/Closed Principle).
/// </summary>
public class AnalizadorPowerAutomate : IAnalizadorIA
{
    private readonly HttpClient _httpClient;
    private readonly string _endpointUrl;

    public AnalizadorPowerAutomate(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _endpointUrl = configuration["PowerAutomate:EndpointUrl"]
            ?? throw new InvalidOperationException("PowerAutomate:EndpointUrl no está configurada.");
    }

    public async Task<ResultadoAnalisisIA> AnalizarRespuestasAsync(
        List<PreguntaConRespuesta> preguntasYRespuestas,
        string tecnologia,
        NivelTecnico nivel)
    {
        // 1. Evaluar cada pregunta individualmente
        var detalles = new List<ScorePorPregunta>();
        foreach (var item in preguntasYRespuestas)
        {
            var detalle = await EvaluarPreguntaAsync(item, tecnologia, nivel);
            detalles.Add(detalle);
        }

        // 2. Obtener resumen general
        var resumen = await ObtenerResumenAsync(preguntasYRespuestas, detalles, tecnologia, nivel);

        resumen.DetallesPorPregunta = detalles;
        return resumen;
    }

    private async Task<ScorePorPregunta> EvaluarPreguntaAsync(
        PreguntaConRespuesta item,
        string tecnologia,
        NivelTecnico nivel)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine($"Eres un evaluador tecnico experto en {tecnologia} evaluando un candidato de nivel {nivel}.");
        prompt.AppendLine($"Evalua del 1 al {item.PuntajeMaximo} la siguiente respuesta tecnica.");
        prompt.AppendLine($"Pregunta: {item.TextoPregunta}");
        if (!string.IsNullOrEmpty(item.Rubrica))
            prompt.AppendLine($"Criterios de evaluacion: {item.Rubrica}");
        prompt.AppendLine($"Respuesta del candidato: {item.RespuestaDelCandidato}");
        prompt.AppendLine($"Responde UNICAMENTE con un JSON: {{\"score\": <numero del 1 al {item.PuntajeMaximo}>, \"justificacion\": \"<breve justificacion>\", \"brechas\": \"<brechas identificadas o vacio>\"}}");

        var respuesta = await EnviarPromptAsync(prompt.ToString());

        return new ScorePorPregunta
        {
            PreguntaId = item.PreguntaId,
            Score = respuesta.Score,
            Feedback = respuesta.Justificacion,
            Brechas = respuesta.Brechas
        };
    }

    private async Task<ResultadoAnalisisIA> ObtenerResumenAsync(
        List<PreguntaConRespuesta> preguntas,
        List<ScorePorPregunta> detalles,
        string tecnologia,
        NivelTecnico nivel)
    {
        var puntajeMaximoTotal = preguntas.Sum(p => p.PuntajeMaximo);
        var puntajeObtenido = detalles.Sum(d => d.Score);
        var porcentaje = puntajeMaximoTotal > 0
            ? Math.Round(puntajeObtenido / puntajeMaximoTotal * 100, 2)
            : 0;

        var prompt = new StringBuilder();
        prompt.AppendLine($"Eres un evaluador tecnico experto en {tecnologia} nivel {nivel}.");
        prompt.AppendLine($"Un candidato obtuvo {puntajeObtenido}/{puntajeMaximoTotal} puntos ({porcentaje}%) en una evaluacion tecnica.");
        prompt.AppendLine("Detalle por pregunta:");

        for (int i = 0; i < preguntas.Count; i++)
        {
            prompt.AppendLine($"- Pregunta {i + 1}: {preguntas[i].TextoPregunta} -> Score: {detalles[i].Score}/{preguntas[i].PuntajeMaximo}. {detalles[i].Feedback}");
        }

        prompt.AppendLine("Responde UNICAMENTE con un JSON: {\"recomendacion\": \"Contratar|Segunda entrevista|No contratar\", \"resumen\": \"<parrafo resumen>\", \"fortalezas\": \"<lista separada por coma>\", \"brechas\": \"<lista separada por coma>\"}");

        var respuesta = await EnviarPromptResumenAsync(prompt.ToString());

        return new ResultadoAnalisisIA
        {
            ScoreTotal = porcentaje,
            Recomendacion = respuesta.Recomendacion,
            ResumenGeneral = respuesta.Resumen,
            Fortalezas = respuesta.Fortalezas,
            Brechas = respuesta.Brechas
        };
    }

    private async Task<RespuestaIAPregunta> EnviarPromptAsync(string prompt)
    {
        var json = JsonSerializer.Serialize(new { prompt });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_endpointUrl, content);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Error en Power Automate: {response.StatusCode} - {responseBody}");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<RespuestaIAPregunta>(responseBody, options)
            ?? new RespuestaIAPregunta();
    }

    private async Task<RespuestaIAResumen> EnviarPromptResumenAsync(string prompt)
    {
        var json = JsonSerializer.Serialize(new { prompt });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_endpointUrl, content);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Error en Power Automate: {response.StatusCode} - {responseBody}");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<RespuestaIAResumen>(responseBody, options)
            ?? new RespuestaIAResumen();
    }

    // DTOs internos para deserializar respuestas de Power Automate
    private class RespuestaIAPregunta
    {
        public decimal Score { get; set; }
        public string Justificacion { get; set; } = string.Empty;
        public string Brechas { get; set; } = string.Empty;
    }

    private class RespuestaIAResumen
    {
        public string Recomendacion { get; set; } = "Sin recomendación";
        public string Resumen { get; set; } = "Sin resumen disponible.";
        public string Fortalezas { get; set; } = string.Empty;
        public string Brechas { get; set; } = string.Empty;
    }
}
