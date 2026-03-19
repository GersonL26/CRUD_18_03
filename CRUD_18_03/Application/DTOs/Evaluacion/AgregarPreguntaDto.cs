using System.ComponentModel.DataAnnotations;
using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Evaluacion;

public class AgregarPreguntaDto
{
    [Required(ErrorMessage = "El texto de la pregunta es obligatorio.")]
    [MaxLength(2000)]
    public string Texto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de pregunta es obligatorio.")]
    public TipoPregunta Tipo { get; set; }

    [MaxLength(1000)]
    public string? Rubrica { get; set; }

    [Required(ErrorMessage = "El puntaje máximo es obligatorio.")]
    [Range(1, 100, ErrorMessage = "El puntaje debe estar entre 1 y 100.")]
    public int PuntajeMaximo { get; set; }

    [Required(ErrorMessage = "El tiempo límite es obligatorio.")]
    [Range(30, 3600, ErrorMessage = "El tiempo debe estar entre 30 y 3600 segundos.")]
    public int TiempoLimiteSegundos { get; set; }
}
