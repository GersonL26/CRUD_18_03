namespace CRUD_18_03.Application.DTOs.ResultadoEvaluacion;

public class ResultadoCandidatoPublicoDto
{
    public string CandidatoNombre { get; set; } = string.Empty;
    public string Tecnologia { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string TituloEvaluacion { get; set; } = string.Empty;
    public decimal ScoreTotal { get; set; }
    public List<string> Fortalezas { get; set; } = new();
    public List<string> Brechas { get; set; } = new();
    public string? TiempoInvertido { get; set; }
    public DateTime GeneradoEn { get; set; }
    public List<DetallePreguntaPublicoDto> Respuestas { get; set; } = new();
}

public class DetallePreguntaPublicoDto
{
    public int OrdenEnEvaluacion { get; set; }
    public string TextoPregunta { get; set; } = string.Empty;
    public int PuntajeMaximo { get; set; }
    public decimal? ScoreIA { get; set; }
    public string? FeedbackIA { get; set; }
}
