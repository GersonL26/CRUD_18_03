namespace CRUD_18_03.Application.DTOs.Proctoring;

public class EventoProctoringDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public DateTime Timestamp { get; set; }
}

public class CrearEventoProctoringDto
{
    public string Tipo { get; set; } = string.Empty;
    public string? Detalle { get; set; }
}

public class ResumenProctoringDto
{
    public int VecesSalioFoco { get; set; }
    public int VecesCopyPaste { get; set; }
    public int TranscripcionesAudio { get; set; }
    public List<EventoProctoringDto> Eventos { get; set; } = new();
}
