using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "jugadora")]
    public class EvaluacionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EvaluacionesController> _logger;

        // Inyección del contexto de base de datos que configuramos en Program.cs
        public EvaluacionesController(ApplicationDbContext context, ILogger<EvaluacionesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("validar-ventana-encuesta")]
        public async Task<IActionResult> ValidarVentanaEncuesta(
    [FromQuery] string? momento = null)
        {
            var ahora = DateTime.Now;
            var fechaHoy = DateTime.Today;
            Console.WriteLine(
    $"MEJ - Hora API: {ahora:dd/MM/yyyy HH:mm:ss} | " +
    $"Fecha: {fechaHoy:dd/MM/yyyy}"
);

            if (!string.IsNullOrWhiteSpace(momento))
            {
                momento = momento.Trim().ToUpperInvariant();

                if (momento != "PRE" && momento != "POST")
                {
                    return BadRequest(new
                    {
                        mensaje = "El momento debe ser PRE o POST."
                    });
                }
            }

            var sesionesHoy = await _context.SesionesDeportivas
                .Where(s =>
                    s.Fecha.Date == fechaHoy &&
                    (s.Estado == "Programada" || s.Estado == "EnCurso" || s.Estado == "En Curso" || s.Estado == "Finalizada"))
                .OrderBy(s => s.HoraInicio)
                .ToListAsync();

            if (sesionesHoy.Count == 0)
            {
                return Ok(new
                {
                    habilitado = false,
                    tipo = "Ninguno",
                    sesionDeportivaId = (int?)null,
                    mensaje = "No hay actividades programadas para hoy."
                });
            }

            foreach (var sesion in sesionesHoy)
            {
                if (!sesion.HoraFin.HasValue)
                    continue;

                var inicio = sesion.Fecha.Date + sesion.HoraInicio;
                var fin = sesion.Fecha.Date + sesion.HoraFin.Value;

                bool esPre =
    (sesion.Estado == "Programada" ||
     sesion.Estado == "EnCurso" ||
     sesion.Estado == "En Curso") &&
    ahora >= inicio.AddMinutes(-60) &&
    ahora < fin;

                bool esPost =
                    (sesion.Estado == "EnCurso" ||
                     sesion.Estado == "En Curso" ||
                     sesion.Estado == "Finalizada") &&
                    ahora >= fin &&
                    ahora <= fin.AddMinutes(60);

                if (esPre && (momento == null || momento == "PRE"))
                {
                    return Ok(new
                    {
                        habilitado = true,
                        tipo = "Pre",
                        sesionDeportivaId = sesion.Id,
                        mensaje = "Evaluación PRE habilitada."
                    });
                }

                if (esPost && (momento == null || momento == "POST"))
                {
                    return Ok(new
                    {
                        habilitado = true,
                        tipo = "Post",
                        sesionDeportivaId = sesion.Id,
                        mensaje = "Evaluación POST habilitada."
                    });
                }
            }

            return Ok(new
            {
                habilitado = false,
                tipo = momento ?? "Bloqueado",
                sesionDeportivaId = (int?)null,
                mensaje = "No hay una sesión deportiva con la evaluación solicitada habilitada en este momento."
            });
        }


        // POST: api/evaluaciones
        [HttpPost]
        public async Task<IActionResult> GuardarEvaluacionDiaria([FromBody] EvaluacionDiaria nuevaEvaluacion)
        {
            if (nuevaEvaluacion == null)
            {
                return BadRequest(new
                {
                    mensaje = "Los datos del formulario son inválidos o están vacíos."
                });
            }

            if (string.IsNullOrWhiteSpace(nuevaEvaluacion.MomentoEvaluacion))
            {
                return BadRequest(new
                {
                    mensaje = "Debés indicar si la evaluación es PRE o POST."
                });
            }

            nuevaEvaluacion.MomentoEvaluacion =
                nuevaEvaluacion.MomentoEvaluacion.Trim().ToUpperInvariant();

            if (nuevaEvaluacion.MomentoEvaluacion != "PRE" &&
                nuevaEvaluacion.MomentoEvaluacion != "POST")
            {
                return BadRequest(new
                {
                    mensaje = "El momento de evaluación debe ser PRE o POST."
                });
            }


            try
            {
                // 1. Validar si la jugadora existe en la base de datos de Talleres
                var existeJugadora = await _context.Jugadoras.AnyAsync(j => j.Id == nuevaEvaluacion.IdJugadora);
                if (!existeJugadora)
                {
                    return BadRequest(new { mensaje = $"La jugadora con ID {nuevaEvaluacion.IdJugadora} no está registrada en el club." });
                }
                var idJugadoraClaim =
    User.FindFirstValue("idJugadora");

                if (!int.TryParse(
                        idJugadoraClaim,
                        out var idJugadoraAutenticada))
                {
                    return Forbid();
                }

                if (idJugadoraAutenticada !=
                    nuevaEvaluacion.IdJugadora)
                {
                    return StatusCode(
                        StatusCodes.Status403Forbidden,
                        new
                        {
                            mensaje =
                                "La jugadora solamente puede registrar su propia evaluación."
                        });
                }

                // VALIDAR SESIÓN DEPORTIVA Y MOMENTO PRE/POST

                if (!nuevaEvaluacion.SesionDeportivaId.HasValue)
                {
                    return BadRequest(new
                    {
                        mensaje = "Debés seleccionar una sesión deportiva válida."
                    });
                }

                var sesion = await _context.SesionesDeportivas
                    .FirstOrDefaultAsync(s =>
                        s.Id == nuevaEvaluacion.SesionDeportivaId.Value);

                if (sesion == null)
                {
                    return BadRequest(new
                    {
                        mensaje = "La sesión deportiva indicada no existe."
                    });
                }

                if (sesion.Estado != "Programada" &&
                    sesion.Estado != "EnCurso" &&
                    sesion.Estado != "En Curso" &&
                    sesion.Estado != "Finalizada")
                {
                    return BadRequest(new
                    {
                        mensaje = "La sesión deportiva no está habilitada para evaluaciones."
                    });
                }

                if (!sesion.HoraFin.HasValue)
                {
                    return BadRequest(new
                    {
                        mensaje = "La sesión deportiva no tiene hora de finalización."
                    });
                }

                var ahora = DateTime.Now;
                var inicio = sesion.Fecha.Date + sesion.HoraInicio;
                var fin = sesion.Fecha.Date + sesion.HoraFin.Value;

                bool horarioValido;

                if (nuevaEvaluacion.MomentoEvaluacion == "PRE")
                {
                    horarioValido =
                        sesion.Estado == "Programada" &&
                        ahora >= inicio.AddMinutes(-60) &&
                        ahora < inicio;
                }
                else
                {
                    horarioValido =
                        ahora >= fin &&
                        ahora <= fin.AddMinutes(60);
                }

                if (!horarioValido)
                {
                    return BadRequest(new
                    {
                        mensaje = $"La evaluación {nuevaEvaluacion.MomentoEvaluacion} no está habilitada en este horario."
                    });
                }

                // La fecha de evaluación corresponde a la sesión validada.
                nuevaEvaluacion.Fecha = sesion.Fecha.Date;

                // 2. Controlar la fecha para evitar desfases de horarios de red
                if (nuevaEvaluacion.Fecha == DateTime.MinValue)
                {
                    nuevaEvaluacion.Fecha = DateTime.Today;
                }

                // 3. Validar si ya envió un reporte para la misma fecha y tipo de sesión
                var yaExisteReporte = await _context.EvaluacionesDiarias.AnyAsync(e =>
        e.IdJugadora == nuevaEvaluacion.IdJugadora &&
        e.Fecha.Date == nuevaEvaluacion.Fecha.Date &&
        e.TipoSesion == nuevaEvaluacion.TipoSesion &&
        e.MomentoEvaluacion == nuevaEvaluacion.MomentoEvaluacion &&
        e.SesionDeportivaId == nuevaEvaluacion.SesionDeportivaId);

                if (yaExisteReporte)
                {
                    return Conflict(new
                    {
                        mensaje = $"Ya registraste la evaluación {nuevaEvaluacion.MomentoEvaluacion} para esta sesión deportiva."
                    });
                }
                // VALIDAR LOS DATOS OBLIGATORIOS DE POST

                if (nuevaEvaluacion.MomentoEvaluacion == "POST")
                {
                    if (!nuevaEvaluacion.EsfuerzoRpe.HasValue ||
                        nuevaEvaluacion.EsfuerzoRpe.Value > 10)
                    {
                        return BadRequest(new
                        {
                            mensaje = "Debés indicar un esfuerzo RPE válido, entre 0 y 10."
                        });
                    }

                    if (!nuevaEvaluacion.MinutosSesion.HasValue ||
                        nuevaEvaluacion.MinutosSesion.Value <= 0)
                    {
                        return BadRequest(new
                        {
                            mensaje = "Los minutos de entrenamiento deben estar entre 10 y 180."
                        });
                    }
                }

                // VALIDAR LOS DATOS OBLIGATORIOS DE PRE

                if (nuevaEvaluacion.MomentoEvaluacion == "PRE")
                {
                    if (!nuevaEvaluacion.Estres.HasValue ||
                        !nuevaEvaluacion.Humor.HasValue ||
                        !nuevaEvaluacion.Sueño.HasValue ||
                        !nuevaEvaluacion.DolorDoms.HasValue ||
                        !nuevaEvaluacion.RecuperacionTqr.HasValue)
                    {
                        return BadRequest(new
                        {
                            mensaje = "Debés completar todos los indicadores de bienestar PRE."
                        });
                    }

                    if (nuevaEvaluacion.DiaCicloMenstrual.HasValue &&
                        (nuevaEvaluacion.DiaCicloMenstrual.Value < 1 ||
                         nuevaEvaluacion.DiaCicloMenstrual.Value > 45))
                    {
                        return BadRequest(new
                        {
                            mensaje = "El día del ciclo menstrual debe estar entre 1 y 45."
                        });
                    }
                }

                // SEPARAR LOS DATOS DE LAS EVALUACIONES PRE Y POST


                if (nuevaEvaluacion.MomentoEvaluacion == "PRE")
                {
                    // PRE: no corresponde registrar esfuerzo ni duración.
                    nuevaEvaluacion.EsfuerzoRpe = null;
                    nuevaEvaluacion.MinutosSesion = null;
                }
                else if (nuevaEvaluacion.MomentoEvaluacion == "POST")
                {
                    // POST: no corresponde registrar indicadores de bienestar PRE.
                    nuevaEvaluacion.Estres = null;
                    nuevaEvaluacion.Humor = null;
                    nuevaEvaluacion.Sueño = null;
                    nuevaEvaluacion.DolorDoms = null;
                    nuevaEvaluacion.RecuperacionTqr = null;
                    nuevaEvaluacion.DiaCicloMenstrual = null;

                    // Las zonas de dolor también se conservan en POST.
                    // Cada registro queda asociado a esta evaluación y sesión.
                }

                // 4. Vincular y preparar las zonas de dolor muscular que tildó en los checkboxes
                if (nuevaEvaluacion.ZonasDeDolor != null)
                {
                    foreach (var zona in nuevaEvaluacion.ZonasDeDolor)
                    {
                        zona.EvaluacionDiaria = nuevaEvaluacion;

                        // Validación intensidad dolor (escala clínica 0–10)
                        if (
                            zona.IntensidadDolor == null ||
                            zona.IntensidadDolor < 0 ||
                            zona.IntensidadDolor > 10
                        )
                        {
                            zona.IntensidadDolor = 0;
                        }
                    }
                }


                // 5. Insertar todo el bloque unificado en SSMS de forma transaccional
                _context.EvaluacionesDiarias.Add(nuevaEvaluacion);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "¡Éxito! Tu reporte diario e historial de dolores musculares se guardaron correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando evaluación {Momento} de jugadora {IdJugadora} en sesión {SesionId}", nuevaEvaluacion.MomentoEvaluacion, nuevaEvaluacion.IdJugadora, nuevaEvaluacion.SesionDeportivaId);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje =
                            "Error interno al procesar el formulario."
                    });
            }
        }
    }
}
