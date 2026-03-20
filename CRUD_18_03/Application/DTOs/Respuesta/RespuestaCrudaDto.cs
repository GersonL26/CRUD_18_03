namespace CRUD_18_03.Application.DTOs.Respuesta;

public class RespuestaCrudaDto
{
    public int Orden { get; set; }
    public string Pregunta { get; set; } = string.Empty;
    public int Tipo { get; set; }
    public int PuntajeMaximo { get; set; }
    public string? Contenido { get; set; }
    public int? TiempoUsadoSegundos { get; set; }
    public DateTime? FechaRespuesta { get; set; }
}
