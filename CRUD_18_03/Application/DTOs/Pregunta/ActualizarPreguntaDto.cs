using System.ComponentModel.DataAnnotations;
using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Pregunta;

public class ActualizarPreguntaDto
{
    [MaxLength(2000)]
    public string? Texto { get; set; }

    public TipoPregunta? Tipo { get; set; }

    [MaxLength(1000)]
    public string? Rubrica { get; set; }

    [Range(1, 100, ErrorMessage = "El puntaje debe estar entre 1 y 100.")]
    public int? PuntajeMaximo { get; set; }

    [Range(1, 30)]
    public int? OrdenEnEvaluacion { get; set; }

    [Range(30, 3600, ErrorMessage = "El tiempo debe estar entre 30 y 3600 segundos.")]
    public int? TiempoLimiteSegundos { get; set; }
}
