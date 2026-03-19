using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Evaluacion;

public class EvaluacionDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Tecnologia { get; set; } = string.Empty;
    public NivelTecnico Nivel { get; set; }
    public EstadoEvaluacion Estado { get; set; }
    public int TiempoLimiteTotalMinutos { get; set; }
    public bool RequiereCamara { get; set; }
    public bool RequiereMicrofono { get; set; }
    public Guid EvaluadorId { get; set; }
    public DateTime CreadoEn { get; set; }
    public bool EstaActivo { get; set; }
}
