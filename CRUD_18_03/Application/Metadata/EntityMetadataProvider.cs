using CRUD_18_03.Application.DTOs.Candidato;
using CRUD_18_03.Application.DTOs.Evaluacion;
using CRUD_18_03.Application.DTOs.Pregunta;
using CRUD_18_03.Application.DTOs.Respuesta;
using CRUD_18_03.Application.DTOs.ResultadoEvaluacion;
using CRUD_18_03.Application.DTOs.SesionEnVivo;
using CRUD_18_03.Domain.Entities;

namespace CRUD_18_03.Application.Metadata;

public class EntityMetadataProvider : IEntityMetadataProvider
{
    private readonly Dictionary<string, EntityMetadata> _registry = new(StringComparer.OrdinalIgnoreCase);

    public EntityMetadataProvider()
    {
        Register("evaluacion",  typeof(Evaluacion),          typeof(EvaluacionDto),          typeof(CrearEvaluacionDto),          typeof(ActualizarEvaluacionDto));
        Register("pregunta",    typeof(Pregunta),             typeof(PreguntaDto),             typeof(CrearPreguntaDto),             typeof(ActualizarPreguntaDto));
        Register("candidato",   typeof(Candidato),            typeof(CandidatoDto),            typeof(CrearCandidatoDto),            typeof(ActualizarCandidatoDto));
        Register("respuesta",   typeof(Respuesta),            typeof(RespuestaDto),            typeof(CrearRespuestaDto),            typeof(ActualizarRespuestaDto));
        Register("resultado",   typeof(ResultadoEvaluacion),  typeof(ResultadoEvaluacionDto),  typeof(CrearResultadoEvaluacionDto),  typeof(ActualizarResultadoEvaluacionDto));
        Register("sesion",      typeof(SesionEnVivo),         typeof(SesionEnVivoDto),         typeof(CrearSesionEnVivoDto),         typeof(ActualizarSesionEnVivoDto));
    }

    public EntityMetadata? GetMetadata(string entityName)
    {
        _registry.TryGetValue(entityName, out var metadata);
        return metadata;
    }

    public IEnumerable<string> GetRegisteredEntityNames()
    {
        return _registry.Keys;
    }

    private void Register(string entityName, Type entityType, Type responseDtoType, Type createDtoType, Type updateDtoType)
    {
        var metadata = new EntityMetadata(entityName, entityType, responseDtoType, createDtoType, updateDtoType);
        _registry[entityName] = metadata;
    }
}
