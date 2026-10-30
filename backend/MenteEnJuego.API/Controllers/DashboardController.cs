using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MenteEnJuego.Servicios.Interfaces;
using MenteEnJuego.Servicios.Seguridad;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IIndicePreventivoService _indicePreventivoService;
        private readonly IDashboardPermissionService _permissionService;

        public DashboardController(
        ApplicationDbContext context,
        IIndicePreventivoService indicePreventivoService,
        IDashboardPermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
            _indicePreventivoService = indicePreventivoService;

        }

        private DashboardPermissions ObtenerPermisosUsuario()
        {
            var rol =
                User.FindFirstValue(ClaimTypes.Role)
                ?? string.Empty;

            var especialidad =
                User.FindFirstValue("especialidad");

            return _permissionService.ObtenerPermisos(
                rol,
                especialidad);
        }

        private bool PuedeConsultarJugadora(
    int idJugadora,
    out ActionResult? resultadoError)
        {
            resultadoError = null;

            if (idJugadora <= 0)
            {
                resultadoError = BadRequest(new
                {
                    mensaje =
                        "El identificador de la jugadora no es válido."
                });

                return false;
            }

            var rol = User.FindFirstValue(ClaimTypes.Role)?
                .Trim()
                .ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(rol))
            {
                resultadoError = Forbid();
                return false;
            }

            /*
             * Una jugadora solamente puede consultar
             * sus propios indicadores.
             */
            if (rol == "jugadora")
            {
                var idJugadoraClaim =
                    User.FindFirstValue("idJugadora");

                if (!int.TryParse(
                        idJugadoraClaim,
                        out var idJugadoraAutenticada))
                {
                    resultadoError = Forbid();
                    return false;
                }

                if (idJugadoraAutenticada != idJugadora)
                {
                    resultadoError = StatusCode(
                        StatusCodes.Status403Forbidden,
                        new
                        {
                            mensaje =
                                "La jugadora solamente puede consultar sus propios indicadores."
                        });

                    return false;
                }
            }

            return true;
        }

        [HttpGet("centro-operaciones")]
        public async Task<ActionResult> ObtenerCentroOperaciones()
        {
            var permisos = ObtenerPermisosUsuario();

            if (!permisos.PuedeVerCentroOperaciones)
            {
                return Forbid();
            }

            var resultado = new
            {
                TotalJugadoras = await _context.Jugadoras.CountAsync(),

                SesionesProgramadas = await _context.SesionesDeportivas
                    .CountAsync(s => s.Estado == "Programada"),

                FormulariosPendientes = await _context.FormulariosSesion
                    .CountAsync(f => f.Estado == "Pendiente"),

                AlertasActivas = await _context.Alertas
                    .CountAsync(a => a.Estado == "Activa" || a.Estado == "EnRevision"),

                Presentes = await _context.ConvocatoriasSesion
                    .CountAsync(c => c.EstadoAsistencia == "Presente"),

                Diferenciadas = await _context.ConvocatoriasSesion
                    .CountAsync(c => c.EstadoAsistencia == "Diferenciado"),

                Ausentes = await _context.ConvocatoriasSesion
                    .CountAsync(c => c.EstadoAsistencia == "Ausente"),

                NotificacionesPendientes = await _context.Notificaciones
                    .CountAsync(n => n.Estado == "Pendiente")
            };

            return Ok(resultado);
        }

        [HttpGet("sesiones")]
        public async Task<ActionResult> ObtenerResumenSesiones()
        {
            var permisos = ObtenerPermisosUsuario();

            if (!permisos.PuedeVerSesiones)
            {
                return Forbid();
            }

            var sesiones = await _context.SesionesDeportivas
                .OrderByDescending(s => s.Fecha)
                .Select(s => new
                {
                    s.Id,
                    s.TipoSesion,
                    s.Titulo,
                    s.Fecha,
                    s.HoraInicio,
                    s.HoraFin,
                    s.Lugar,
                    s.Estado,

                    TotalConvocadas = _context.ConvocatoriasSesion
                        .Count(c => c.SesionDeportivaId == s.Id),

                    FormulariosPendientes = _context.FormulariosSesion
                        .Count(f => f.SesionDeportivaId == s.Id && f.Estado == "Pendiente"),

                    AlertasActivas = _context.Alertas
                        .Count(a => a.SesionDeportivaId == s.Id &&
                                    (a.Estado == "Activa" || a.Estado == "EnRevision"))
                })
                .ToListAsync();

            return Ok(sesiones);
        }
        [HttpGet("tecnico")]
        public async Task<ActionResult<DashboardTecnicoDto>> ObtenerDashboardTecnico()
        {
            var permisos = ObtenerPermisosUsuario();

            if (!permisos.PuedeVerDashboardTecnico)
            {
                return Forbid();
            }

            var totalJugadoras = await _context.Jugadoras.CountAsync();

            var sesionesProgramadas = await _context.SesionesDeportivas
                .CountAsync(s => s.Estado == "Programada");

            var formulariosPendientes = await _context.FormulariosSesion
                .CountAsync(f => f.Estado == "Pendiente");

            var alertasActivas = await _context.Alertas
                .CountAsync(a => a.Estado == "Activa" || a.Estado == "EnRevision");

            var evaluaciones = await _context.EvaluacionesDiarias
                .OrderByDescending(e => e.Fecha)
                .Take(30)
                .ToListAsync();

            var recuperacionPromedio = evaluaciones.Any()
                ? Math.Round((decimal)evaluaciones.Average(e => e.RecuperacionTqr), 2)
                : 0;

            var fatigaPromedio = evaluaciones.Any()
                ? Math.Round((decimal)evaluaciones.Average(e => e.EsfuerzoRpe), 2)
                : 0;

            var disponibilidadPlantel = totalJugadoras > 0
                ? Math.Round(((decimal)(totalJugadoras - alertasActivas) / totalJugadoras) * 100, 2)
                : 0;

            var riesgoLesion = totalJugadoras > 0
                ? Math.Round(((decimal)alertasActivas / totalJugadoras) * 100, 2)
                : 0;

            var evolucionCarga = evaluaciones
                .OrderBy(e => e.Fecha)
                .Select(e => new SerieDto
                {
                    Fecha = e.Fecha.ToString("yyyy-MM-dd"),
                    Valor = (decimal)(e.UnidadesArbitrarias ?? 0)
                })
                .ToList();

            var evolucionWellness = evaluaciones
                .OrderBy(e => e.Fecha)
                .Select(e => new SerieDto
                {
                    Fecha = e.Fecha.ToString("yyyy-MM-dd"),
                    Valor = (decimal)((e.Estres ?? 0) + (e.Humor ?? 0) + (e.Sueño ?? 0) + (e.DolorDoms ?? 0))
                })
                .ToList();

            var mapaDolor = await _context.EvaluacionesZonasDolor
                .GroupBy(z => z.ZonaMuscular)
                .Select(g => new ZonaDolorDto
                {
                    Zona = g.Key,
                    Intensidad = g.Count()
                })
                .ToListAsync();

            var dto = new DashboardTecnicoDto
            {
                TotalJugadoras = totalJugadoras,
                SesionesProgramadas = sesionesProgramadas,
                FormulariosPendientes = formulariosPendientes,
                AlertasActivas = alertasActivas,
                RecuperacionPromedio = recuperacionPromedio,
                FatigaPromedio = fatigaPromedio,
                RiesgoLesion = riesgoLesion,
                DisponibilidadPlantel = disponibilidadPlantel,
                EvolucionCarga = evolucionCarga,
                EvolucionWellness = evolucionWellness,
                MapaDolor = mapaDolor
            };

            return Ok(dto);
        }

        [HttpGet("graficos")]
        public async Task<ActionResult> ObtenerGraficos()
        {
            var permisos = ObtenerPermisosUsuario();

            if (!permisos.PuedeVerDashboardTecnico)
            {
                return Forbid();
            }

            var carga = await _context.EvaluacionesDiarias
                .OrderBy(e => e.Fecha)
                .Select(e => new
                {
                    Fecha =
                        e.Fecha
                        .ToString("yyyy-MM-dd"),

                    Carga =
                        e.UnidadesArbitrarias,

                    Fatiga =
                        e.EsfuerzoRpe
                })
                .ToListAsync();


            var bienestar = await _context.EvaluacionesDiarias
                .OrderBy(e => e.Fecha)
                .Select(e => new
                {
                    Fecha =
                        e.Fecha
                        .ToString("yyyy-MM-dd"),

                    Wellness =
                        e.Estres
                        + e.Humor
                        + e.Sueño
                        + e.DolorDoms
                })
                .ToListAsync();


            var dolor =
                await _context
                .EvaluacionesZonasDolor

                .GroupBy(
                    x => x.ZonaMuscular
                )

                .Select(
                    x => new
                    {
                        Zona =
                            x.Key,

                        Valor =
                            x.Count()
                    }
                )
                .ToListAsync();


            return Ok(
                new
                {
                    carga,
                    bienestar,
                    dolor
                }
            );

        }
        [HttpGet("graficos/{jugadoraId:int}")]
        public async Task<ActionResult> ObtenerGraficosPorJugadora(int jugadoraId)
        {
            var permisos = ObtenerPermisosUsuario();

            if (!permisos.PuedeConsultarGraficosIndividuales)
            {
                return Forbid();
            }

            if (!PuedeConsultarJugadora(
        jugadoraId,
        out var resultadoError))
            {
                return resultadoError!;
            }
            var carga = await _context.EvaluacionesDiarias
                .Where(e => e.IdJugadora == jugadoraId)
                .OrderBy(e => e.Fecha)
                .Select(e => new
                {
                    Fecha = e.Fecha.ToString("yyyy-MM-dd"),
                    Carga = e.UnidadesArbitrarias,
                    Fatiga = e.EsfuerzoRpe,
                    DuracionMinutos = e.MinutosSesion
                })
                .ToListAsync();

            var bienestar = await _context.EvaluacionesDiarias
                .Where(e => e.IdJugadora == jugadoraId)
                .OrderBy(e => e.Fecha)
                .Select(e => new
                {
                    Fecha = e.Fecha.ToString("yyyy-MM-dd"),
                    Wellness =
                        e.Estres +
                        e.Humor +
                        e.Sueño +
                        e.DolorDoms
                })
                .ToListAsync();

            var idsEvaluaciones = await _context.EvaluacionesDiarias
                .Where(e => e.IdJugadora == jugadoraId)
                .Select(e => e.Id)
                .ToListAsync();

            var dolor = await _context.EvaluacionesZonasDolor
                .Where(z => idsEvaluaciones.Contains(z.IdEvaluacion))
                .GroupBy(z => z.ZonaMuscular)
                .Select(g => new
                {
                    Zona = g.Key,
                    Valor = g.Count(),
                     IntensidadDolor = g.Max(x => x.IntensidadDolor ?? 0)
                })
                .ToListAsync();

            var dolorPorFechaDatos = await _context.EvaluacionesZonasDolor
      .Where(z => idsEvaluaciones.Contains(z.IdEvaluacion))
      .Select(z => new
      {
          Fecha = z.EvaluacionDiaria.Fecha!.Date,
          Zona = z.ZonaMuscular,
          Intensidad = z.IntensidadDolor ?? 0
      })
      .ToListAsync();

            var dolorPorFecha = dolorPorFechaDatos
                .GroupBy(x => x.Fecha)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var maximo = g
                        .OrderByDescending(x => x.Intensidad)
                        .First();

                    return new
                    {
                        Fecha = g.Key.ToString("yyyy-MM-dd"),
                        DolorMaximo = maximo.Intensidad,
                        Zona = maximo.Zona
                    };
                })
                .ToList();
            // Seguimiento evolutivo del dolor por zona muscular
            var dolorPorZona = dolorPorFechaDatos
                .GroupBy(x => new { x.Fecha, x.Zona })
                .OrderBy(g => g.Key.Fecha)
                .ThenBy(g => g.Key.Zona)
                .Select(g => new
                {
                    Fecha = g.Key.Fecha.ToString("yyyy-MM-dd"),
                    Zona = g.Key.Zona,
                    Intensidad = g.Max(x => x.Intensidad)
                })
                .ToList();
            return Ok(new
            {
                carga,
                bienestar,
                dolor,
                dolorPorFecha,
                dolorPorZona
            });
        }
        [HttpGet("evolucion-dolor/{idJugadora:int}")]
        public async Task<IActionResult> EvolucionDolor(int idJugadora)
        {
            var permisos = ObtenerPermisosUsuario();

            if (!permisos.PuedeConsultarDolorIndividual)
            {
                return Forbid();
            }
            if (!PuedeConsultarJugadora(
        idJugadora,
        out var resultadoError))
            {
                return resultadoError!;
            }
            var datos = await _context.EvaluacionesZonasDolor
                .Where(z => z.EvaluacionDiaria.IdJugadora == idJugadora)
                .OrderByDescending(z => z.EvaluacionDiaria.Fecha)
                .Select(z => new
                {
                    fecha = z.EvaluacionDiaria.Fecha,
                    zona = z.ZonaMuscular,
                    intensidad = z.IntensidadDolor ?? 0
                })
                .ToListAsync();

            var evolucion = datos
                .GroupBy(x => x.zona)
                .Select(g =>
                {
                    var ultimos = g
                        .OrderByDescending(x => x.fecha)
                        .Take(2)
                        .ToList();

                    var actual = ultimos.ElementAtOrDefault(0);
                    var anterior = ultimos.ElementAtOrDefault(1);

                    var variacion =
                        anterior == null ? 0 : actual.intensidad - anterior.intensidad;

                    return new
                    {
                        zona = g.Key,
                        fechaActual = actual.fecha,
                        intensidadActual = actual.intensidad,
                        intensidadAnterior = anterior != null ? anterior.intensidad : (int?)null,
                        variacion = variacion,
                        tendencia =
                            anterior == null ? "Sin historial" :
                            variacion > 0 ? "Empeoró" :
                            variacion < 0 ? "Mejoró" :
                            "Estable"
                    };
                })
                .OrderByDescending(x => x.intensidadActual)
                .ToList();

            return Ok(evolucion);
        }
        [HttpGet("comparativa-gps")]
        public async Task<ActionResult> ObtenerComparativaGps()
        {
            var permisos = ObtenerPermisosUsuario();

            if (!permisos.PuedeVerComparativaGps)
            {
                return Forbid();
            }

            var datos = await _context.MetricasGpsAvanzadas
                .Where(g => g.EvaluacionDiaria != null)
                .GroupBy(g => new
                {
                    g.EvaluacionDiaria!.IdJugadora,
                    g.EvaluacionDiaria!.Jugadora!.Apellido,
                    g.EvaluacionDiaria!.Jugadora!.Nombre
                
                })
                .Select(g => new
                {
                    Jugadora =
                        g.Key.Apellido + ", " + g.Key.Nombre,

                    PlayerLoad =
                        Math.Round(g.Average(x => x.PlayerLoadIndice), 2),

                    Kilometros =
                        Math.Round(g.Average(x => x.DistanciaTotalKm), 2)
                })
                .OrderByDescending(x => x.PlayerLoad)
                .ToListAsync();

            return Ok(datos);
        }
        [HttpGet("heatmap-gps")]
        public async Task<ActionResult> ObtenerHeatmapGps()
        {
            var permisos = ObtenerPermisosUsuario();

            if (!permisos.PuedeVerHeatmapGps)
            {
                return Forbid();
            }

            var datos = await _context.MetricasGpsAvanzadas
                .Where(g =>
                    g.EvaluacionDiaria != null &&
                    g.EvaluacionDiaria.Jugadora != null)
                .GroupBy(g => new
                {
                    g.EvaluacionDiaria!.IdJugadora,
                    g.EvaluacionDiaria!.Jugadora!.Apellido,
                    g.EvaluacionDiaria!.Jugadora!.Nombre
                })
                .Select(g => new
                {
                    Jugadora =
                        g.Key.Apellido + ", " + g.Key.Nombre,

                    PosicionMediaX =
                       Math.Round(
                           g.Average(x => x.PosicionMediaX) ?? 0,2),

                    PosicionMediaY =
                       Math.Round(
                           g.Average(x => x.PosicionMediaY) ?? 0,2),

                    PlayerLoad =
                        Math.Round(g.Average(x => x.PlayerLoadIndice), 2),

                    DistanciaKm =
                        Math.Round(g.Average(x => x.DistanciaTotalKm), 2)
                })
                .OrderByDescending(x => x.PlayerLoad)
                .ToListAsync();

            return Ok(datos);
        }
    

    [Authorize(Roles = "admin,entrenador")]
    [HttpGet("planilla-diaria")]
    public async Task<IActionResult> ObtenerPlanillaDiaria(
    [FromQuery] DateTime fecha)
    {
        var permisos = ObtenerPermisosUsuario();

        if (!permisos.PuedeVerCentroOperaciones &&
            !permisos.PuedeConsultarGraficosIndividuales)
        {
            return Forbid();
        }

        var inicio = fecha.Date;
        var fin = inicio.AddDays(1);

        var jugadoras = await _context.Jugadoras
            .Where(j => j.Activo)
            .OrderBy(j => j.Apellido)
            .ThenBy(j => j.Nombre)
            .Select(j => new
            {
                idJugadora = j.Id,
                apellido = j.Apellido,
                nombre = j.Nombre,
                posicion = j.Posicion,
                division = j.Equipo,

                evaluacion = _context.EvaluacionesDiarias
                    .Where(e =>
                        e.IdJugadora == j.Id &&
                        e.Fecha >= inicio &&
                        e.Fecha < fin)
                    .OrderByDescending(e => e.Id)
                    .Select(e => new
                    {
                        e.Id,
                        e.Estres,
                        e.Humor,
                        e.Sueño,
                        e.DolorDoms,
                        e.EsfuerzoRpe,
                        e.DiaCicloMenstrual,

                        dolorMaximo = e.ZonasDeDolor
                            .Select(z => (int?)z.IntensidadDolor)
                            .Max() ?? 0,

                        cantidadZonasDolor =
                            e.ZonasDeDolor.Count()
                    })
                    .FirstOrDefault()
            })
            .ToListAsync();

            var resultado = new List<object>();

            foreach (var j in jugadoras)
            {
                if (j.evaluacion == null)
                {
                    resultado.Add(new
                    {
                        j.idJugadora,
                        j.apellido,
                        j.nombre,
                        j.posicion,
                        j.division,

                        tieneEvaluacion = false,

                        wellness = (double?)null,
                        rpe = (int?)null,
                        dolor = (int?)null,
                        ciclo = (int?)null,
                        riesgo = (double?)null,

                        nivelRiesgo = "SIN_DATOS",
                        estado = "Sin evaluar"
                    });

                    continue;
                }

                /*
                 * Wellness de la Planilla.
                 * Conservamos el dato visual que ya mostraba.
                 */
                double wellness = (double)((j.evaluacion.Estres ?? 0) + (j.evaluacion.Humor ?? 0) + (j.evaluacion.Sueño ?? 0) + (j.evaluacion.DolorDoms ?? 0));

                /*
                 * Índice Preventivo centralizado.
                 *
                 * Se procesa una jugadora por vez para evitar
                 * operaciones concurrentes sobre el mismo DbContext.
                 */
                var indicePreventivo =
                    await _indicePreventivoService
                        .CalcularIndicePreventivoAsync(
                            j.idJugadora,
                            fecha);

                var riesgo =
                    indicePreventivo.IndicePreventivo;

                var nivel =
                    indicePreventivo.NivelPreventivo;

                var estado =
                    nivel == "ALTO" ? "Restringir" :
                    nivel == "MODERADO" ? "Control" :
                    nivel == "BAJO" ? "Disponible" :
                    "Sin evaluar";

                resultado.Add(new
                {
                    j.idJugadora,
                    j.apellido,
                    j.nombre,
                    j.posicion,
                    j.division,

                    tieneEvaluacion = true,

                    wellness = (double?)wellness,
                    rpe = (int?)j.evaluacion.EsfuerzoRpe,

                    dolor = (int?)(
                        j.evaluacion.dolorMaximo > 0
                            ? j.evaluacion.dolorMaximo
                            : j.evaluacion.DolorDoms),

                    ciclo =
                        (int?)j.evaluacion.DiaCicloMenstrual,

                    riesgo = riesgo,

                    nivelRiesgo = nivel,
                    estado
                });
            }

            return Ok(resultado);
        }


        [HttpGet("carga-3d")]
        public async Task<IActionResult> ObtenerCarga3D(
            [FromQuery] DateTime desde,
            [FromQuery] DateTime hasta)
        {
            var permisos = ObtenerPermisosUsuario();

            if (!permisos.PuedeVerDashboardTecnico)
                return Forbid();

            if (desde == default || hasta == default ||
                desde.Date > hasta.Date ||
                (hasta.Date - desde.Date).TotalDays > 90)
            {
                return BadRequest(new
                {
                    mensaje = "Indicá un período válido de hasta 90 días."
                });
            }

            var inicio = desde.Date;
            var finExclusivo = hasta.Date.AddDays(1);

            var datos = await _context.EvaluacionesDiarias
                .AsNoTracking()
                .Where(e =>
                    e.Fecha >= inicio &&
                    e.Fecha < finExclusivo &&
                    e.Jugadora != null)
                .OrderBy(e => e.Fecha)
                .ThenBy(e => e.Jugadora!.Apellido)
                .ThenBy(e => e.Jugadora!.Nombre)
                .Select(e => new
                {
                    e.Id,
                    e.IdJugadora,
                    Nombre = e.Jugadora!.Apellido + ", " +
                             e.Jugadora!.Nombre,
                    e.Fecha,
                    Carga = e.UnidadesArbitrarias,
                    Rpe = e.EsfuerzoRpe,
                    DuracionMinutos = e.MinutosSesion,
                    e.SesionDeportivaId
                })
                .ToListAsync();

            return Ok(datos.Select(e => new
            {
                e.Id,
                e.IdJugadora,
                e.Nombre,
                Fecha = e.Fecha.ToString("yyyy-MM-dd"),
                e.Carga,
                e.Rpe,
                e.DuracionMinutos,
                e.SesionDeportivaId
            }));
        }


    }

}