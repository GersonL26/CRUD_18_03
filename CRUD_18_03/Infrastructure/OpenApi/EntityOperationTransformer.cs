using System.Text.Json.Nodes;
using CRUD_18_03.Application.Metadata;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CRUD_18_03.Infrastructure.OpenApi;

public class EntityOperationTransformer(IEntityMetadataProvider metadataProvider) : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var param = operation.Parameters?.FirstOrDefault(p => p.Name == "entityName");
        if (param is not OpenApiParameter entityParam)
            return Task.CompletedTask;

        var entityNames = metadataProvider.GetRegisteredEntityNames().OrderBy(n => n).ToList();

        entityParam.Description = $"Entidades disponibles: {string.Join(", ", entityNames)}";

        var schema = entityParam.Schema as OpenApiSchema ?? new OpenApiSchema();
        entityParam.Schema = schema;

        schema.Enum ??= new List<JsonNode>();
        foreach (var name in entityNames)
            schema.Enum.Add(JsonValue.Create(name)!);

        return Task.CompletedTask;
    }
}
