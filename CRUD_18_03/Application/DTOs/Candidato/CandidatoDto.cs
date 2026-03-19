namespace CRUD_18_03.Application.DTOs.Candidato;

public class CandidatoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime? FechaInicioRespuesta { get; set; }
    public DateTime? FechaFinRespuesta { get; set; }
    public Guid EvaluacionId { get; set; }
    public DateTime CreadoEn { get; set; }
    public bool EstaActivo { get; set; }
}
