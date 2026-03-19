using CRUD_18_03.Application.DTOs.Evaluacion;
using CRUD_18_03.Application.DTOs.Pregunta;

namespace CRUD_18_03.Application.Interfaces;

public interface IEvaluacionService
{
    Task<EvaluacionConPreguntasDto> CrearAsync(CrearEvaluacionConPreguntasDto dto, Guid evaluadorId);
    Task<IEnumerable<EvaluacionDto>> ListarPorEvaluadorAsync(Guid evaluadorId);
    Task<EvaluacionConPreguntasDto?> ObtenerConPreguntasAsync(Guid id, Guid evaluadorId);
    Task ActualizarAsync(Guid id, ActualizarEvaluacionDto dto, Guid evaluadorId);
    Task EliminarAsync(Guid id, Guid evaluadorId);
    Task ActivarAsync(Guid id, Guid evaluadorId);
    Task CerrarAsync(Guid id, Guid evaluadorId);
    Task ReactivarAsync(Guid id, Guid evaluadorId);
    Task<PreguntaDto> AgregarPreguntaAsync(Guid evaluacionId, AgregarPreguntaDto dto, Guid evaluadorId);
    Task ActualizarPreguntaAsync(Guid evaluacionId, Guid preguntaId, ActualizarPreguntaDto dto, Guid evaluadorId);
    Task EliminarPreguntaAsync(Guid evaluacionId, Guid preguntaId, Guid evaluadorId);
}
