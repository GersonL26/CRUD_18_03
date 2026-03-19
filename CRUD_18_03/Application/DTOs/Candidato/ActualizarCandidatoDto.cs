using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.Candidato;

public class ActualizarCandidatoDto
{
    [MaxLength(200)]
    public string? Nombre { get; set; }

    [MaxLength(250)]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    public string? Email { get; set; }
}
