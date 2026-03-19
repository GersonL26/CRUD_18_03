using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.Candidato;

public class InvitarCandidatoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [MaxLength(250)]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    public string Email { get; set; } = string.Empty;
}
