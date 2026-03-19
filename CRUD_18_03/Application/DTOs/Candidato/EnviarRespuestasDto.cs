using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.Candidato;

public class EnviarRespuestasDto
{
    [Required(ErrorMessage = "Las respuestas son obligatorias.")]
    [MinLength(1, ErrorMessage = "Debe enviar al menos una respuesta.")]
    public List<RespuestaItemDto> Respuestas { get; set; } = new();
}

public class RespuestaItemDto
{
    [Required(ErrorMessage = "El ID de la pregunta es obligatorio.")]
    public Guid PreguntaId { get; set; }

    [Required(ErrorMessage = "El contenido de la respuesta es obligatorio.")]
    public string Contenido { get; set; } = string.Empty;

    [Range(0, 7200, ErrorMessage = "El tiempo debe estar entre 0 y 7200 segundos.")]
    public int? TiempoUsadoSegundos { get; set; }
}
