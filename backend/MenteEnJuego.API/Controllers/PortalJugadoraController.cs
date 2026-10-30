using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "jugadora")]
    public class PortalJugadoraController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PortalJugadoraController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{jugadoraId:int}")]
        public async Task<ActionResult> ObtenerPortalJugadora(int jugadoraId)
        {
            if (jugadoraId <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la jugadora no es válido."
                });
            }

            var idJugadoraClaim =
                User.FindFirstValue("idJugadora");

            if (!int.TryParse(
                    idJugadoraClaim,
                    out var idJugadoraAutenticada))
            {
                return Forbid();
            }

            if (idJugadoraAutenticada != jugadoraId)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        mensaje =
                            "La jugadora solamente puede consultar su propio portal."
                    });
            }
            var jugadora = await _context.Jugadoras
                .AsNoTracking()
                .Where(j => j.Id == jugadoraId)
                .Select(j => new
                {
                    j.Id,
                    j.Nombre,
                    j.Apellido,

                    ProximaSesion = _context.SesionesDeportivas
                        .Where(s => s.Convocatorias.Any(c => c.JugadoraId == j.Id))
                        .OrderBy(s => s.Fecha)
                        .ThenBy(s => s.HoraInicio)
                        .Select(s => new
                        {
                            s.Id,
                            s.TipoSesion,
                            s.Titulo,
                            s.Fecha,
                            s.HoraInicio,
                            s.HoraFin,
                            s.Lugar,
                            s.Estado
                        })
                        .FirstOrDefault(),

                    FormulariosPendientes = _context.FormulariosSesion
                        .Where(f => f.JugadoraId == j.Id && f.Estado == "Pendiente")
                        .Select(f => new
                        {
                            f.Id,
                            f.SesionDeportivaId,
                            f.TipoFormulario,
                            f.Estado,
                            f.FechaHabilitacion
                        })
                        .ToList(),

                    NotificacionesPendientes = _context.Notificaciones
                        .Where(n => n.JugadoraId == j.Id && n.Estado == "Pendiente")
                        .OrderByDescending(n => n.FechaCreacion)
                        .Select(n => new
                        {
                            n.Id,
                            n.Canal,
                            n.TipoNotificacion,
                            n.Titulo,
                            n.Mensaje,
                            n.FechaProgramada
                        })
                        .ToList(),

                    AlertasActivas = _context.Alertas
                        .Where(a => j.Id == a.JugadoraId &&
                                    (a.Estado == "Activa" || a.Estado == "EnRevision"))
                        .Select(a => new
                        {
                            a.Id,
                            a.TipoAlerta,
                            a.Nivel,
                            a.Titulo,
                            a.Mensaje,
                            a.Estado
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (jugadora == null)
                return NotFound("Jugadora no encontrada.");

            return Ok(jugadora);
        }
    }
}