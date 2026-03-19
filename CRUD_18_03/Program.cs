using System.Text;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Application.Mapping;
using CRUD_18_03.Application.Metadata;
using CRUD_18_03.Infrastructure.AI;
using CRUD_18_03.Infrastructure.OpenApi;
using CRUD_18_03.Infrastructure.Persistence;
using CRUD_18_03.Infrastructure.Services;
using CRUD_18_03.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEvaluacionService, EvaluacionService>();
builder.Services.AddScoped<ICandidatoService, CandidatoService>();
builder.Services.AddHttpClient<IAnalizadorIA, AnalizadorPowerAutomate>();
builder.Services.AddScoped<IAnalisisService, AnalisisService>();
builder.Services.AddScoped<IResultadoService, ResultadoService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key no está configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
