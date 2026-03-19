using CRUD_18_03.Application.DTOs.SesionEnVivo;

namespace CRUD_18_03.Application.Interfaces;

public interface ISesionEnVivoService
{
    Task<EstadoSesionDto> CrearSesionAsync(Guid evaluacionId, Guid candidatoId, Guid evaluadorId);
    Task<EstadoSesionDto> ObtenerEstadoAsync(Guid sesionId);
    Task<PreguntaEnVivoDto> IniciarSesionAsync(Guid sesionId, string tokenCandidato);
    Task<PreguntaEnVivoDto> ObtenerPreguntaActualAsync(Guid sesionId, string tokenCandidato);
    Task<PreguntaEnVivoDto?> ResponderYAvanzarAsync(Guid sesionId, string tokenCandidato, ResponderPreguntaEnVivoDto dto);
    Task FinalizarSesionAsync(Guid sesionId, string tokenCandidato);
}
