using CRUD_18_03.Domain.Entities;

namespace CRUD_18_03.Application.Interfaces;

public interface IJwtService
{
    string GenerarToken(Usuario usuario);
}
