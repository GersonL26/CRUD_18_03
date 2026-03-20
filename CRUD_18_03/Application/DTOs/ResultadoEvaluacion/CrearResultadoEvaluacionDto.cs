using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.ResultadoEvaluacion;

public class CrearResultadoEvaluacionDto
{
    [Required]
    [Range(0, 100, ErrorMessage = "El score debe estar entre 0 y 100.")]
    public decimal ScoreTotal { get; set; }

    [Required(ErrorMessage = "El resumen de IA es obligatorio.")]
    public string ResumenIA { get; set; } = string.Empty;

    public string BrechasDetectadas { get; set; } = string.Empty;
    public string FortalezasDetectadas { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ID del candidato es obligatorio.")]
    public Guid CandidatoId { get; set; }
}
