using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Servicios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MenteEnJuego.Servicios.Seguridad;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LegajoDeportivoController : ControllerBase
    {
        private readonly ILegajoDeportivoService _legajoService;
        private readonly ILegajoPermissionService _permissionService;
        private readonly ApplicationDbContext _context;

        public LegajoDeportivoController(
            ILegajoDeportivoService legajoService,
            ILegajoPermissionService permissionService,
            ApplicationDbContext context)
        {
            _legajoService = legajoService;
            _permissionService = permissionService;
            _context = context;
        }

        // GET: api/LegajoDeportivo/{idJugadora}
        [HttpGet("{idJugadora:int}")]
        public async Task<IActionResult> ObtenerLegajo(
            int idJugadora)
        {
            if (idJugadora <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la jugadora no es válido."
                });
            }

            var rol = User.FindFirstValue(ClaimTypes.Role)?
                .Trim()
                .ToLowerInvariant();

            var idJugadoraClaim =
                User.FindFirstValue("idJugadora");

            /*
             * Una jugadora solamente puede consultar su propio legajo.
             * Administradores, profesionales y cuerpo técnico continúan
             * sujetos a los permisos generales del sistema.
             */
            if (rol == "jugadora")
            {
                if (!int.TryParse(
                        idJugadoraClaim,
                        out var idJugadoraAutenticada))
                {
                    return Forbid();
                }

                if (idJugadoraAutenticada != idJugadora)
                {
                    return StatusCode(
                        StatusCodes.Status403Forbidden,
                        new
                        {
                            mensaje =
                                "La jugadora solamente puede consultar su propio legajo."
                        }
                    );
                }
            }

            if (string.IsNullOrWhiteSpace(rol))
            {
                return Forbid();
            }

            var especialidad =
                User.FindFirstValue("especialidad");

            var permisos =
                _permissionService.ObtenerPermisos(
                    rol,
                    especialidad
                );

            var legajo =
                await _legajoService.ObtenerLegajoAsync(
                    idJugadora,
                    permisos
                );

            if (legajo == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "No se encontró el legajo deportivo de la jugadora."
                });
            }

            return Ok(legajo);
        }

        [Authorize(Roles = "admin,medico,entrenador")]
        [HttpGet("buscar-jugadoras")]
        [Authorize]
        public async Task<IActionResult> BuscarJugadoras(
            [FromQuery] string? termino,
            [FromQuery] string? equipo)
        {
            termino = termino?.Trim().ToLower();
            equipo = equipo?.Trim().ToLower();

            var query = _context.Jugadoras.AsQueryable();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                query = query.Where(j =>
                    j.Apellido.ToLower().Contains(termino) ||
                    j.Nombre.ToLower().Contains(termino) ||
                    j.Dni.Contains(termino));
            }

            if (!string.IsNullOrWhiteSpace(equipo))
            {
                query = query.Where(j =>
     j.Equipo != null &&
     j.Equipo.ToLower() == equipo);
            }

            var jugadoras = await query
                .OrderBy(j => j.Apellido)
                .ThenBy(j => j.Nombre)
                .Take(50)
                .Select(j => new
                {
                    idJugadora = j.Id,
                    apellido = j.Apellido,
                    nombre = j.Nombre,
                    dni = j.Dni,
                    posicion = j.Posicion,
                    equipo = j.Equipo
                })
                .ToListAsync();

            return Ok(jugadoras);
        }
    }
}