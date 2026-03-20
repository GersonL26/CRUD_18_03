using CRUD_18_03.Application.DTOs.Candidato;
using CRUD_18_03.Application.DTOs.Respuesta;

namespace CRUD_18_03.Application.Interfaces;

public interface ICandidatoService
{
    Task<CandidatoDto> AsignarAsync(Guid evaluacionId, AsignarCandidatoDto dto, Guid evaluadorId);
    Task<IEnumerable<CandidatoDto>> ListarPorEvaluacionAsync(Guid evaluacionId, Guid evaluadorId);
    Task<IEnumerable<RespuestaCrudaDto>> ObtenerRespuestasCrudasAsync(Guid evaluacionId, Guid candidatoId, Guid evaluadorId);
    Task EliminarAsync(Guid evaluacionId, Guid candidatoId, Guid evaluadorId);
    Task<IEnumerable<EvaluacionAsignadaDto>> ListarEvaluacionesPorUsuarioAsync(Guid usuarioId);

    Task<EvaluacionCandidatoDto> ObtenerPorTokenAsync(string token);
    Task IniciarRespuestaAsync(string token);
    Task EnviarRespuestasAsync(string token, EnviarRespuestasDto dto);
    Task RegistrarPerdidaFocoAsync(string token);
}
