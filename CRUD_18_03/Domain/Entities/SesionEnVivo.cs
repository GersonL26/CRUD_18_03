namespace CRUD_18_03.Domain.Entities;

public class SesionEnVivo : BaseEntity
{
    public Guid EvaluacionId { get; set; }
    public Guid CandidatoId { get; set; }
    public int PreguntaActualIndex { get; set; } = 0;
    public DateTime? InicioFaseActual { get; set; }
    public bool SesionActiva { get; set; } = false;
    public bool FueCompletada { get; set; } = false;

    public Evaluacion? Evaluacion { get; set; }
    public Candidato? Candidato { get; set; }
}
