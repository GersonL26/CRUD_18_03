namespace CRUD_18_03.Application.DTOs.SesionEnVivo;

public class SesionEnVivoDto
{
    public Guid Id { get; set; }
    public Guid EvaluacionId { get; set; }
    public Guid CandidatoId { get; set; }
    public int PreguntaActualIndex { get; set; }
    public DateTime? InicioFaseActual { get; set; }
    public bool SesionActiva { get; set; }
    public bool FueCompletada { get; set; }
    public DateTime CreadoEn { get; set; }
    public bool EstaActivo { get; set; }
}
