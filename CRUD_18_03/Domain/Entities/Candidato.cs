namespace CRUD_18_03.Domain.Entities;

public class Candidato : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime? FechaInicioRespuesta { get; set; }
    public DateTime? FechaFinRespuesta { get; set; }
    public Guid EvaluacionId { get; set; }

    public Evaluacion? Evaluacion { get; set; }
    public List<Respuesta> Respuestas { get; set; } = new();
    public ResultadoEvaluacion? Resultado { get; set; }
}
