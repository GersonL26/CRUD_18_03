using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.Respuesta;

public class CrearRespuestaDto
{
    [Required(ErrorMessage = "El contenido de la respuesta es obligatorio.")]
    public string Contenido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ID del candidato es obligatorio.")]
    public Guid CandidatoId { get; set; }

    [Required(ErrorMessage = "El ID de la pregunta es obligatorio.")]
    public Guid PreguntaId { get; set; }

    public int? TiempoUsadoSegundos { get; set; }
}
