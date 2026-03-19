using CRUD_18_03.Application.DTOs.ResultadoEvaluacion;

namespace CRUD_18_03.Application.Interfaces;

public interface IAnalisisService
{
    Task<ResultadoCompletoDto> EjecutarAnalisisAsync(Guid candidatoId, Guid evaluadorId);
    Task<ResultadoCompletoDto?> ObtenerResultadoAsync(Guid candidatoId, Guid evaluadorId);
}
