using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.ResultadoEvaluacion;

public class ActualizarResultadoEvaluacionDto
{
    [Range(0, 100, ErrorMessage = "El score debe estar entre 0 y 100.")]
    public double? ScoreTotal { get; set; }

    public string? ResumenIA { get; set; }
    public string? BrechasDetectadas { get; set; }
    public string? FortalezasDetectadas { get; set; }
}
