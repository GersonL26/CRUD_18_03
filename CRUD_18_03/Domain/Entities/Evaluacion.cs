using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Domain.Entities;

public class Evaluacion : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Tecnologia { get; set; } = string.Empty;
    public NivelTecnico Nivel { get; set; }
    public EstadoEvaluacion Estado { get; set; } = EstadoEvaluacion.Borrador;
    public int TiempoLimiteTotalMinutos { get; set; }
    public bool RequiereCamara { get; set; } = false;
    public bool RequiereMicrofono { get; set; } = false;
    public Guid EvaluadorId { get; set; }

    public List<Pregunta> Preguntas { get; set; } = new();
    public List<Candidato> Candidatos { get; set; } = new();
}
