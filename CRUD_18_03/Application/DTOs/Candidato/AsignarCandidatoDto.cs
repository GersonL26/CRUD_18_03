using System.ComponentModel.DataAnnotations;

namespace CRUD_18_03.Application.DTOs.Candidato;

public class AsignarCandidatoDto
{
    [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
    public Guid UsuarioId { get; set; }
}
