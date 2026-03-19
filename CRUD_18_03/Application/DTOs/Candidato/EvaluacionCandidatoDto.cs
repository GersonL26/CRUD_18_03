using CRUD_18_03.Application.DTOs.Pregunta;
using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Candidato;

public class EvaluacionCandidatoDto
{
    public Guid EvaluacionId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Tecnologia { get; set; } = string.Empty;
    public NivelTecnico Nivel { get; set; }
    public int TiempoLimiteTotalMinutos { get; set; }
    public bool RequiereCamara { get; set; }
    public bool RequiereMicrofono { get; set; }

    public Guid CandidatoId { get; set; }
    public string NombreCandidato { get; set; } = string.Empty;
    public DateTime? FechaInicioRespuesta { get; set; }
    public bool YaRespondio { get; set; }

    public List<PreguntaCandidatoDto> Preguntas { get; set; } = new();
}

public class PreguntaCandidatoDto
{
    public Guid Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public TipoPregunta Tipo { get; set; }
    public int PuntajeMaximo { get; set; }
    public int OrdenEnEvaluacion { get; set; }
    public int TiempoLimiteSegundos { get; set; }
}
