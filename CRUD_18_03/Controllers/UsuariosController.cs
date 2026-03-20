using CRUD_18_03.Application.DTOs.Auth;
using CRUD_18_03.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public UsuariosController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _dbContext.Usuarios
            .OrderByDescending(u => u.CreadoEn)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                NombreCompleto = u.NombreCompleto,
                Email = u.Email,
                Rol = u.Rol,
                CreadoEn = u.CreadoEn,
                EstaActivo = u.EstaActivo
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ActualizarUsuarioDto dto)
    {
        var usuario = await _dbContext.Usuarios.FindAsync(id);
        if (usuario is null)
            return NotFound(new { message = "Usuario no encontrado." });

        if (dto.NombreCompleto is not null)
            usuario.NombreCompleto = dto.NombreCompleto;

        if (dto.Rol.HasValue)
            usuario.Rol = dto.Rol.Value;

        if (dto.EstaActivo.HasValue)
            usuario.EstaActivo = dto.EstaActivo.Value;

        usuario.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Usuario actualizado correctamente." });
    }
}
