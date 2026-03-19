namespace CRUD_18_03.Application.DTOs.SesionEnVivo;

public class EstadoSesionDto
{
    public Guid SesionId { get; set; }
    public Guid EvaluacionId { get; set; }
    public Guid CandidatoId { get; set; }
    public string NombreCandidato { get; set; } = string.Empty;
    public int PreguntaActualIndex { get; set; }
    public int TotalPreguntas { get; set; }
    public int SegundosTranscurridos { get; set; }
    public int TiempoLimitePreguntaSegundos { get; set; }
    public bool SesionActiva { get; set; }
    public bool FueCompletada { get; set; }
}

public class PreguntaEnVivoDto
{
    public Guid PreguntaId { get; set; }
    public int Index { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int Tipo { get; set; }
    public int PuntajeMaximo { get; set; }
    public int TiempoLimiteSegundos { get; set; }
    public int TotalPreguntas { get; set; }
}

public class ResponderPreguntaEnVivoDto
{
    public Guid PreguntaId { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public int TiempoUsadoSegundos { get; set; }
}
