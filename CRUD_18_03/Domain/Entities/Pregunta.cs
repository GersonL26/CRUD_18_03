using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Domain.Entities;

public class Pregunta : BaseEntity
{
    public string Texto { get; set; } = string.Empty;
    public TipoPregunta Tipo { get; set; }
    public string? Rubrica { get; set; }
    public int PuntajeMaximo { get; set; }
    public int OrdenEnEvaluacion { get; set; }
    public int TiempoLimiteSegundos { get; set; }
    public Guid EvaluacionId { get; set; }

    public Evaluacion? Evaluacion { get; set; }
}
