using System.ComponentModel.DataAnnotations;
using CRUD_18_03.Domain.Enums;

namespace CRUD_18_03.Application.DTOs.Evaluacion;

public class ActualizarEvaluacionDto
{
    [MaxLength(200)]
    public string? Titulo { get; set; }

    [MaxLength(1000)]
    public string? Descripcion { get; set; }

    [MaxLength(100)]
    public string? Tecnologia { get; set; }

    public NivelTecnico? Nivel { get; set; }

    [Range(10, 300, ErrorMessage = "El tiempo límite debe estar entre 10 y 300 minutos.")]
    public int? TiempoLimiteTotalMinutos { get; set; }

    public bool? RequiereCamara { get; set; }
    public bool? RequiereMicrofono { get; set; }
}
