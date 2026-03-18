using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Application.Mapping;
using CRUD_18_03.Application.Metadata;
using CRUD_18_03.Infrastructure.OpenApi;
using CRUD_18_03.Infrastructure.Persistence;
using CRUD_18_03.Infrastructure.Services;
using CRUD_18_03.Middleware;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// MySQL connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register DbContext as DbContext for GenericService
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

// Application services
builder.Services.AddSingleton<IEntityMetadataProvider, EntityMetadataProvider>();
builder.Services.AddSingleton<IEntityMapper, ReflectionEntityMapper>();
builder.Services.AddScoped<IGenericService, GenericService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
    options.AddOperationTransformer<EntityOperationTransformer>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
