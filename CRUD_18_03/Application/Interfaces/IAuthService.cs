using CRUD_18_03.Application.DTOs.Auth;

namespace CRUD_18_03.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegistrarAsync(RegistroDto registroDto);
}
