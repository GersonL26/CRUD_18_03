using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.Interfaces;

public interface IAnalizadorIA
{
    Task<ResultadoAnalisisIA> AnalizarRespuestasAsync(
        List<PreguntaConRespuesta> preguntasYRespuestas,
        string tecnologia,
        NivelTecnico nivel);
}

public class PreguntaConRespuesta
{
    public Guid PreguntaId { get; set; }
    public string TextoPregunta { get; set; } = string.Empty;
    public string? Rubrica { get; set; }
    public int PuntajeMaximo { get; set; }
    public string RespuestaDelCandidato { get; set; } = string.Empty;
}

public class ResultadoAnalisisIA
{
    public decimal ScoreTotal { get; set; }
    public string Recomendacion { get; set; } = string.Empty;
    public string ResumenGeneral { get; set; } = string.Empty;
    public string Brechas { get; set; } = string.Empty;
    public string Fortalezas { get; set; } = string.Empty;
    public List<ScorePorPregunta> DetallesPorPregunta { get; set; } = new();
}

public class ScorePorPregunta
{
    public Guid PreguntaId { get; set; }
    public decimal Score { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public string Brechas { get; set; } = string.Empty;
}
