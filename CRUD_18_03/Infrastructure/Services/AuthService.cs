using CRUD_18_03.Application.DTOs.Auth;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Domain.Entities;
using CRUD_18_03.Domain.Enums;
using CRUD_18_03.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IJwtService _jwtService;

    public AuthService(ApplicationDbContext dbContext, IJwtService jwtService)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var usuario = await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.EstaActivo)
            ?? throw new InvalidOperationException("Credenciales inválidas.");

        var passwordValida = BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash);
        if (!passwordValida)
            throw new InvalidOperationException("Credenciales inválidas.");

        var token = _jwtService.GenerarToken(usuario);

        return new AuthResponseDto
        {
            UsuarioId = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Token = token
        };
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegistroDto registroDto)
    {
        var emailExiste = await _dbContext.Usuarios
            .AnyAsync(u => u.Email == registroDto.Email);

        if (emailExiste)
            throw new InvalidOperationException("Ya existe un usuario con ese email.");

        var usuario = new Usuario
        {
            NombreCompleto = registroDto.NombreCompleto,
            Email = registroDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registroDto.Password),
            Rol = registroDto.Rol
        };

        _dbContext.Usuarios.Add(usuario);
        await _dbContext.SaveChangesAsync();

        var token = _jwtService.GenerarToken(usuario);

        return new AuthResponseDto
        {
            UsuarioId = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Token = token
        };
    }
}
