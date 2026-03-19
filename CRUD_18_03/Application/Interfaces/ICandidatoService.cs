using CRUD_18_03.Application.DTOs.Candidato;

namespace CRUD_18_03.Application.Interfaces;

public interface ICandidatoService
{
    // --- Evaluador: gestión de candidatos ---
    Task<CandidatoDto> InvitarAsync(Guid evaluacionId, InvitarCandidatoDto dto, Guid evaluadorId);
    Task<IEnumerable<CandidatoDto>> ListarPorEvaluacionAsync(Guid evaluacionId, Guid evaluadorId);
    Task EliminarAsync(Guid evaluacionId, Guid candidatoId, Guid evaluadorId);

    // --- Candidato: acceso público con token ---
    Task<EvaluacionCandidatoDto> ObtenerPorTokenAsync(string token);
    Task IniciarRespuestaAsync(string token);
    Task EnviarRespuestasAsync(string token, EnviarRespuestasDto dto);
}
