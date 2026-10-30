using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Servicios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador,medico,jugadora")]
    public class RiesgoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IIndicePreventivoService _indicePreventivoService;

        public RiesgoController(ApplicationDbContext context, IIndicePreventivoService indicePreventivoService)
        {
            _context = context;
            _indicePreventivoService = indicePreventivoService;
        }

        private bool PuedeConsultarJugadora(int idJugadora, out ActionResult? error)
        {
            error = null;

            if (idJugadora <= 0)
            {
                error = BadRequest(new
                {
                    mensaje = "El identificador de la jugadora no es válido."
                });
                return false;
            }

            var rol = User.FindFirstValue(ClaimTypes.Role)?
                .Trim()
                .ToLowerInvariant();

            if (rol == "jugadora")
            {
                var claim = User.FindFirstValue("idJugadora");

                if (!int.TryParse(claim, out var idAutenticada) ||
                    idAutenticada != idJugadora)
                {
                    error = StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        mensaje = "La jugadora solamente puede consultar sus propios indicadores."
                    });
                    return false;
                }
            }

            return true;
        }

        // Endpoint requerido por los gráficos de Análisis Técnico.
        [HttpGet("indice-preventivo/{idJugadora:int}")]
        public async Task<IActionResult> ObtenerIndicePreventivo(
            int idJugadora, [FromQuery] DateTime fecha)
        {
            if (!PuedeConsultarJugadora(idJugadora, out var error))
                return error!;

            var resultado = await _indicePreventivoService
                .CalcularIndicePreventivoAsync(idJugadora, fecha.Date);
            return Ok(resultado);
        }

        [HttpGet("riesgo-gps/{idJugadora:int}")]
        public async Task<IActionResult> RiesgoGps(int idJugadora)
        {
            if (!PuedeConsultarJugadora(idJugadora, out var error))
                return error!;

            var datos = await _context.MetricasGpsAvanzadas
                .Where(g => g.EvaluacionDiaria != null &&
                            g.EvaluacionDiaria.IdJugadora == idJugadora)
                .OrderByDescending(g => g.EvaluacionDiaria!.Fecha)
                .Select(g => new
                {
                    playerLoad = g.PlayerLoadIndice,
                    distanciaKm = g.DistanciaTotalKm,
                    cambiosDireccion = g.CantidadCambiosDireccion,
                    aceleraciones = g.CantidadAceleracionesIntensas,
                    desaceleraciones = g.CantidadDesaceleracionesIntensas
                })
                .FirstOrDefaultAsync();

            if (datos == null)
            {
                return Ok(new
                {
                    disponible = false,
                    mensaje = "No hay métricas GPS registradas para esta jugadora."
                });
            }

            return Ok(new
            {
                disponible = true,
                datos.playerLoad,
                datos.distanciaKm,
                datos.cambiosDireccion,
                datos.aceleraciones,
                datos.desaceleraciones
            });
        }

        [HttpGet("resumen-riesgo/{idJugadora:int}")]
        public async Task<IActionResult> ResumenRiesgo(int idJugadora)
        {
            if (!PuedeConsultarJugadora(idJugadora, out var error))
                return error!;

            var evaluacion = await _context.EvaluacionesDiarias
                .Where(e => e.IdJugadora == idJugadora)
                .OrderByDescending(e => e.Fecha)
                .ThenByDescending(e => e.Id)
                .Select(e => new
                {
                    e.Id,
                    e.IdJugadora,
                    e.Fecha,
                    e.MomentoEvaluacion,
                    e.EsfuerzoRpe,
                    e.DolorDoms,
                    e.DiaCicloMenstrual,
                    Zonas = e.ZonasDeDolor.Select(z => z.ZonaMuscular).ToList()
                })
                .FirstOrDefaultAsync();

            if (evaluacion == null)
            {
                return Ok(new
                {
                    disponible = false,
                    mensaje = "No hay evaluaciones registradas para esta jugadora."
                });
            }

            return Ok(new
            {
                disponible = true,
                evaluacion
            });
        }
    }
}