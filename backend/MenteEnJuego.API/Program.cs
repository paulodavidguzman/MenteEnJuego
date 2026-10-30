using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MenteEnJuego.API.Configuracion;
using MenteEnJuego.API.Seguridad;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Servicios.Implementaciones;
using MenteEnJuego.Servicios.Interfaces;
using MenteEnJuego.Servicios.Mapping.Planificacion;
using MenteEnJuego.Servicios.Seguridad;
using MenteEnJuego.Servicios.Validaciones.Planificacion;
using MenteEnJuego.Servicios;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// BASE DE DATOS
// ======================================================

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión DefaultConnection.");
}

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlServer(connectionString));

// ======================================================
// SERVICIOS EXISTENTES
// ======================================================

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IGpsService, GpsService>();

builder.Services.AddScoped<
    ILegajoDeportivoService,
    LegajoDeportivoService>();

builder.Services.AddScoped<
    IRendimientoService,
    RendimientoService>();

builder.Services.AddScoped<
    ICicloMenstrualService,
    CicloMenstrualService>();

builder.Services.AddScoped<
    IIndicePreventivoService,
    IndicePreventivoService>();

// ======================================================
// SERVICIOS DE PERMISOS
// ======================================================

builder.Services.AddScoped<
    ISaludPermissionService,
    SaludPermissionService>();

builder.Services.AddScoped<
    ILegajoPermissionService,
    LegajoPermissionService>();

builder.Services.AddScoped<
    IDashboardPermissionService,
    DashboardPermissionService>();

// ======================================================
// MÓDULO 2 — MICROCICLOS
// ======================================================

// AutoMapper
builder.Services.AddAutoMapper(
    configuracion => { },
    typeof(MicrocicloProfile).Assembly);

// FluentValidation
builder.Services.AddScoped<
    IValidator<CrearMicrocicloDTO>,
    CrearMicrocicloValidator>();

builder.Services.AddScoped<
    IValidator<ActualizarMicrocicloDTO>,
    ActualizarMicrocicloValidator>();

builder.Services.AddScoped<
    IValidator<CambiarEstadoMicrocicloDTO>,
    CambiarEstadoMicrocicloValidator>();

//Validador PlanificacionTecnica
builder.Services.AddScoped<
    IValidator<CrearPlanificacionTecnicaDTO>,
    CrearPlanificacionTecnicaValidator>();

builder.Services.AddScoped<
    IValidator<ActualizarPlanificacionTecnicaDTO>,
    ActualizarPlanificacionTecnicaValidator>();

builder.Services.AddScoped<
    IValidator<CambiarEstadoPlanificacionDTO>,
    CambiarEstadoPlanificacionValidator>();

// Validar objetivos especificos
builder.Services.AddScoped<
    IValidator<CrearObjetivoEspecificoDTO>,
    CrearObjetivoEspecificoValidator>();

builder.Services.AddScoped<
    IValidator<ActualizarObjetivoEspecificoDTO>,
    ActualizarObjetivoEspecificoValidator>();

builder.Services.AddScoped<
    IValidator<EvaluarObjetivoEspecificoDTO>,
    EvaluarObjetivoEspecificoValidator>();

// Validadores de contenidos planificados
builder.Services.AddScoped<
    IValidator<CrearContenidoPlanificadoDTO>,
    CrearContenidoPlanificadoValidator>();

builder.Services.AddScoped<
    IValidator<ActualizarContenidoPlanificadoDTO>,
    ActualizarContenidoPlanificadoValidator>();

// Validadores de bloques de sesión
builder.Services.AddScoped<
    IValidator<CrearBloqueSesionDTO>,
    CrearBloqueSesionValidator>();

builder.Services.AddScoped<
    IValidator<ActualizarBloqueSesionDTO>,
    ActualizarBloqueSesionValidator>();

// Validadores de indicadores planificados
builder.Services.AddScoped<
    IValidator<CrearIndicadorPlanificadoDTO>,
    CrearIndicadorPlanificadoValidator>();

builder.Services.AddScoped<
    IValidator<ActualizarIndicadorPlanificadoDTO>,
    ActualizarIndicadorPlanificadoValidator>();

// Validadores de staff de planificación
builder.Services.AddScoped<
    IValidator<CrearStaffPlanificacionDTO>,
    CrearStaffPlanificacionValidator>();

builder.Services.AddScoped<
    IValidator<ActualizarStaffPlanificacionDTO>,
    ActualizarStaffPlanificacionValidator>();

// Servicio de Microciclos
builder.Services.AddScoped<
    IMicrocicloService,
    MicrocicloService>();

//Servicio panificaciontecnica
builder.Services.AddScoped<
    IPlanificacionTecnicaService,
    PlanificacionTecnicaService>();

// servicio objetivosespecifico
builder.Services.AddScoped<
    IObjetivoEspecificoService,
    ObjetivoEspecificoService>();

// Servicio de contenidos planificados
builder.Services.AddScoped<
    IContenidoPlanificadoService,
    ContenidoPlanificadoService>();

// Servicio de bloques de sesión
builder.Services.AddScoped<
    IBloqueSesionService,
    BloqueSesionService>();

// Servicio de indicadores planificados
builder.Services.AddScoped<
    IIndicadorPlanificadoService,
    IndicadorPlanificadoService>();

// Servicio de staff de planificación
builder.Services.AddScoped<
    IStaffPlanificacionService,
    StaffPlanificacionService>();

// ======================================================
// CORS
// ======================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "PermitirAccesoTotalJS",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// ======================================================
// CONTROLADORES Y SWAGGER
// ======================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Mente en Juego API",
            Version = "v1",
            Description =
                "API del Sistema Integral de Gestión y Seguimiento Deportivo."
        });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Ingrese únicamente el token JWT generado por el login."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

// ======================================================
// JWT
// ======================================================

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(
        JwtOptions.SectionName));

builder.Services.AddScoped<
    ITokenService,
    TokenService>();

var jwtOptions =
    builder.Configuration
        .GetSection(JwtOptions.SectionName)
        .Get<JwtOptions>();

if (jwtOptions == null)
{
    throw new InvalidOperationException(
        "No se encontró la configuración JWT.");
}

if (string.IsNullOrWhiteSpace(
        jwtOptions.SecretKey))
{
    throw new InvalidOperationException(
        "La clave secreta JWT no está configurada.");
}

if (string.IsNullOrWhiteSpace(
        jwtOptions.Issuer))
{
    throw new InvalidOperationException(
        "El emisor JWT no está configurado.");
}

if (string.IsNullOrWhiteSpace(
        jwtOptions.Audience))
{
    throw new InvalidOperationException(
        "La audiencia JWT no está configurada.");
}

var claveJwt =
    Encoding.UTF8.GetBytes(
        jwtOptions.SecretKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer =
                    jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience =
                    jwtOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        claveJwt),

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

// ======================================================
// CREACIÓN DE LA APLICACIÓN
// ======================================================

var app = builder.Build();

// ======================================================
// PIPELINE HTTP
// ======================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Mente en Juego API v1.0");
    });
}

//app.UseHttpsRedirection();

app.UseCors("PermitirAccesoTotalJS");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();