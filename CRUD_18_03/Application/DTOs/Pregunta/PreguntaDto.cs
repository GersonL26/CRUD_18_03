using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Pregunta;

public class PreguntaDto
{
    public Guid Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public TipoPregunta Tipo { get; set; }
    public string? Rubrica { get; set; }
    public int PuntajeMaximo { get; set; }
    public int OrdenEnEvaluacion { get; set; }
    public int TiempoLimiteSegundos { get; set; }
    public Guid EvaluacionId { get; set; }
    public DateTime CreadoEn { get; set; }
    public bool EstaActivo { get; set; }
}
