using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.SesionEnVivo;

public class CrearSesionEnVivoDto
{
    [Required(ErrorMessage = "El ID de la evaluación es obligatorio.")]
    public Guid EvaluacionId { get; set; }

    [Required(ErrorMessage = "El ID del candidato es obligatorio.")]
    public Guid CandidatoId { get; set; }
}
