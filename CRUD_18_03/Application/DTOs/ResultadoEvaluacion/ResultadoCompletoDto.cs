using CRUD_18_03.Application.DTOs.Respuesta;

namespace CRUD_18_03.Application.DTOs.ResultadoEvaluacion;

public class ResultadoCompletoDto
{
    public Guid ResultadoId { get; set; }
    public string CandidatoNombre { get; set; } = string.Empty;
    public string CandidatoEmail { get; set; } = string.Empty;
    public string Tecnologia { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string TituloEvaluacion { get; set; } = string.Empty;
    public double ScoreTotal { get; set; }
    public string Recomendacion { get; set; } = string.Empty;
    public string ResumenIA { get; set; } = string.Empty;
    public string BrechasDetectadas { get; set; } = string.Empty;
    public string FortalezasDetectadas { get; set; } = string.Empty;
    public DateTime GeneradoEn { get; set; }
    public string? TiempoInvertido { get; set; }
    public List<DetalleRespuestaDto> Respuestas { get; set; } = new();
}

public class DetalleRespuestaDto
{
    public Guid PreguntaId { get; set; }
    public string TextoPregunta { get; set; } = string.Empty;
    public int OrdenEnEvaluacion { get; set; }
    public int PuntajeMaximo { get; set; }
    public string ContenidoRespuesta { get; set; } = string.Empty;
    public int? TiempoUsadoSegundos { get; set; }
    public double? ScoreIA { get; set; }
    public string? FeedbackIA { get; set; }
    public string? BrechasIdentificadas { get; set; }
}
