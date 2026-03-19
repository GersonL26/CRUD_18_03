using CRUD_18_03.Application.DTOs.ResultadoEvaluacion;

namespace CRUD_18_03.Application.Interfaces;

public interface IResultadoService
{
    Task<ResultadoCompletoDto?> ObtenerResultadoAsync(Guid candidatoId, Guid evaluadorId);

    Task<ResumenEvaluacionResultadosDto> ObtenerResumenEvaluacionAsync(Guid evaluacionId, Guid evaluadorId);

    Task<ResultadoCandidatoPublicoDto> ObtenerResultadoPorTokenAsync(string token);

    Task<RankingEvaluacionDto> ObtenerRankingAsync(Guid evaluacionId, Guid evaluadorId);

    Task<ComparacionCandidatosDto> CompararCandidatosAsync(Guid evaluacionId, List<Guid> candidatoIds, Guid evaluadorId);
}
