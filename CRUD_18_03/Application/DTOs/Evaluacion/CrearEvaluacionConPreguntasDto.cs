using System.ComponentModel.DataAnnotations;
using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Evaluacion;

public class CrearEvaluacionConPreguntasDto
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La tecnología es obligatoria.")]
    [MaxLength(100)]
    public string Tecnologia { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nivel técnico es obligatorio.")]
    public NivelTecnico Nivel { get; set; }

    [Required(ErrorMessage = "El tiempo límite es obligatorio.")]
    [Range(10, 300, ErrorMessage = "El tiempo límite debe estar entre 10 y 300 minutos.")]
    public int TiempoLimiteTotalMinutos { get; set; }

    public bool RequiereCamara { get; set; } = false;
    public bool RequiereMicrofono { get; set; } = false;

    public List<AgregarPreguntaDto>? Preguntas { get; set; }
}
