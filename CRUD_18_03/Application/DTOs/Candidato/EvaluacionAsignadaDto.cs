using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Candidato;

public class EvaluacionAsignadaDto
{
    public Guid EvaluacionId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Tecnologia { get; set; } = string.Empty;
    public NivelTecnico Nivel { get; set; }
    public int TiempoLimiteTotalMinutos { get; set; }
    public int TotalPreguntas { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime? FechaInicioRespuesta { get; set; }
    public DateTime? FechaFinRespuesta { get; set; }
    public DateTime CreadoEn { get; set; }
}
