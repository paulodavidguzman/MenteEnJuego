using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.Entidades;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador")]
    public class ConvocatoriasSesionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConvocatoriasSesionController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // ============================================
        // GET: api/ConvocatoriasSesion/sesion/5
        // ============================================

        [HttpGet("sesion/{sesionId:int}")]
        public async Task<ActionResult> ObtenerPorSesion(
            int sesionId)
        {
            if (sesionId <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la sesión no es válido."
                });
            }

            var existeSesion =
                await _context.SesionesDeportivas
                    .AnyAsync(s => s.Id == sesionId);

            if (!existeSesion)
            {
                return NotFound(new
                {
                    mensaje =
                        "La sesión deportiva no existe."
                });
            }

            var convocatorias =
                await _context.ConvocatoriasSesion
                    .AsNoTracking()
                    .Where(c =>
                        c.SesionDeportivaId == sesionId)
                    .Include(c => c.Jugadora)
                    .OrderBy(c => c.Jugadora!.Apellido)
                    .ThenBy(c => c.Jugadora!.Nombre)
                    .Select(c => new
                    {
                        c.Id,
                        c.SesionDeportivaId,
                        c.JugadoraId,

                        jugadora = new
                        {
                            c.Jugadora!.Id,
                            c.Jugadora.Apellido,
                            c.Jugadora.Nombre,
                            c.Jugadora.Posicion
                        },

                        c.EstadoConvocatoria,
                        c.EstadoAsistencia,
                        c.Observaciones,
                        c.FechaCreacion
                    })
                    .ToListAsync();

            return Ok(convocatorias);
        }


        // ============================================
        // POST: api/ConvocatoriasSesion/sesion/5
        // ============================================

        [HttpPost("sesion/{sesionId:int}")]
        public async Task<ActionResult> CrearConvocatorias(
            int sesionId,
            [FromBody] CrearConvocatoriasSesionDTO solicitud)
        {
            if (sesionId <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la sesión no es válido."
                });
            }

            if (solicitud == null ||
                solicitud.JugadoraIds == null ||
                solicitud.JugadoraIds.Count == 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "Debe seleccionar al menos una jugadora."
                });
            }

            var sesion =
    await _context.SesionesDeportivas
        .Include(s => s.Microciclo)
        .FirstOrDefaultAsync(
            s => s.Id == sesionId);

            if (sesion == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "La sesión deportiva no existe."
                });
            }

            if (!string.Equals(
         sesion.Estado,
         "Programada",
         StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    mensaje =
                        "Solo se pueden modificar convocatorias de sesiones programadas."
                });
            }

            if (sesion.Microciclo == null ||
    string.IsNullOrWhiteSpace(
        sesion.Microciclo.Division))
            {
                return BadRequest(new
                {
                    mensaje =
                        "El microciclo de la sesión no posee una división válida."
                });
            }

            var division =
                sesion.Microciclo.Division.Trim();
        

            var idsSolicitados =
                solicitud.JugadoraIds
                    .Distinct()
                    .ToList();

        var jugadorasValidas =
 await _context.Jugadoras
     .Where(j =>
         idsSolicitados.Contains(j.Id) &&
         j.Activo &&
         j.FechaBaja == null &&
         j.Equipo != null &&
         j.Equipo.Trim() == division)
     .Select(j => j.Id)
     .ToListAsync();

        var idsInvalidos =
    idsSolicitados
        .Except(jugadorasValidas)
        .ToList();

            if (idsInvalidos.Count > 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "Una o más jugadoras no existen, están inactivas o no pertenecen a la división del microciclo.",
                    idsInvalidos
                });
            }

            var yaConvocadas =
                await _context.ConvocatoriasSesion
                    .Where(c =>
                        c.SesionDeportivaId == sesionId &&
                        idsSolicitados.Contains(c.JugadoraId))
                    .Select(c => c.JugadoraId)
                    .ToListAsync();

            var nuevasIds =
                idsSolicitados
                    .Except(yaConvocadas)
                    .ToList();

            foreach (var jugadoraId in nuevasIds)
            {
                _context.ConvocatoriasSesion.Add(
                    new ConvocatoriaSesion
                    {
                        SesionDeportivaId = sesionId,
                        JugadoraId = jugadoraId,
                        EstadoConvocatoria = "Convocada",
                        EstadoAsistencia = null,
                        Observaciones =
                            string.IsNullOrWhiteSpace(
                                solicitud.Observaciones)
                                ? null
                                : solicitud.Observaciones.Trim(),
                        FechaCreacion = DateTime.Now
                    }
                );
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje =
                    "Las convocatorias fueron procesadas correctamente.",

                cantidadSolicitada =
                    idsSolicitados.Count,

                cantidadCreada =
                    nuevasIds.Count,

                cantidadYaExistente =
                    yaConvocadas.Count
            });
        }

        // ============================================
        // GET: api/ConvocatoriasSesion/sesion/8/convocables
        // ============================================

        [HttpGet("sesion/{sesionId:int}/convocables")]
        public async Task<ActionResult> ObtenerConvocables(int sesionId)
        {
            var sesion = await _context.SesionesDeportivas
                .AsNoTracking()
                .Include(s => s.Microciclo)
                .FirstOrDefaultAsync(s => s.Id == sesionId);

            if (sesion == null)
            {
                return NotFound(new
                {
                    mensaje = "La sesión deportiva no existe."
                });
            }

            if (sesion.Microciclo == null ||
                string.IsNullOrWhiteSpace(sesion.Microciclo.Division))
            {
                return BadRequest(new
                {
                    mensaje = "La sesión no tiene una división asignada."
                });
            }

            var division = sesion.Microciclo.Division.Trim();

            var jugadoras = await _context.Jugadoras
                .AsNoTracking()
                .Where(j =>
                    j.Activo &&
                    j.FechaBaja == null &&
                    j.Equipo != null &&
                    j.Equipo.Trim() == division)
                .OrderBy(j => j.Apellido)
                .ThenBy(j => j.Nombre)
                .Select(j => new
                {
                    id = j.Id,
                    nombre = j.Nombre,
                    apellido = j.Apellido,
                    posicion = j.Posicion,
                    division = j.Equipo
                })
                .ToListAsync();

            return Ok(jugadoras);
        }
        // ============================================
        // DELETE: api/ConvocatoriasSesion/sesion/8/jugadora/10
        // ============================================

        [HttpDelete("sesion/{sesionId:int}/jugadora/{jugadoraId:int}")]
        public async Task<ActionResult> EliminarConvocatoria(
            int sesionId,
            int jugadoraId)
        {
            if (sesionId <= 0 || jugadoraId <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Los identificadores no son válidos."
                });
            }

            var sesion = await _context.SesionesDeportivas
                .FirstOrDefaultAsync(s => s.Id == sesionId);

            if (sesion == null)
            {
                return NotFound(new
                {
                    mensaje = "La sesión deportiva no existe."
                });
            }

            if (!string.Equals(
                sesion.Estado,
                "Programada",
                StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    mensaje = "Solo se pueden modificar convocatorias de sesiones programadas."
                });
            }

            var convocatoria = await _context.ConvocatoriasSesion
                .FirstOrDefaultAsync(c =>
                    c.SesionDeportivaId == sesionId &&
                    c.JugadoraId == jugadoraId);

            if (convocatoria == null)
            {
                return NotFound(new
                {
                    mensaje = "La jugadora no está convocada a esta sesión."
                });
            }

            _context.ConvocatoriasSesion.Remove(convocatoria);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "La jugadora fue desconvocada correctamente.",
                sesionId,
                jugadoraId
            });
        }

    }
}