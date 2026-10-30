using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador")]
    public class BloquesSesionController : ControllerBase
    {
        private readonly IBloqueSesionService _bloqueSesionService;

        public BloquesSesionController(
            IBloqueSesionService bloqueSesionService)
        {
            _bloqueSesionService = bloqueSesionService;
        }

        // GET:
        // api/BloquesSesion/planificacion/1
        [HttpGet("planificacion/{planificacionTecnicaId:int}")]
        public async Task<IActionResult> ObtenerPorPlanificacion(
            int planificacionTecnicaId)
        {
            if (planificacionTecnicaId <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la planificación técnica no es válido."
                });
            }

            var bloques = await _bloqueSesionService
                .ObtenerPorPlanificacionAsync(
                    planificacionTecnicaId);

            return Ok(bloques);
        }

        // GET:
        // api/BloquesSesion/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del bloque no es válido."
                });
            }

            var bloque = await _bloqueSesionService
                .ObtenerPorIdAsync(id);

            if (bloque == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "No se encontró el bloque de sesión solicitado."
                });
            }

            return Ok(bloque);
        }

        // POST:
        // api/BloquesSesion
        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] CrearBloqueSesionDTO solicitud)
        {
            var resultado = await _bloqueSesionService
                .CrearAsync(solicitud);

            if (!resultado.Exito)
            {
                return ResolverError(resultado.Mensaje);
            }

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new
                {
                    id = resultado.Datos!.Id
                },
                resultado.Datos);
        }

        // PUT:
        // api/BloquesSesion/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(
            int id,
            [FromBody] ActualizarBloqueSesionDTO solicitud)
        {
            var resultado = await _bloqueSesionService
                .ActualizarAsync(
                    id,
                    solicitud);

            if (!resultado.Exito)
            {
                return ResolverError(resultado.Mensaje);
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                datos = resultado.Datos
            });
        }

        // DELETE:
        // api/BloquesSesion/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _bloqueSesionService
                .EliminarAsync(id);

            if (!resultado.Exito)
            {
                return ResolverError(resultado.Mensaje);
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje
            });
        }

        private IActionResult ResolverError(string mensaje)
        {
            if (mensaje.Contains(
                    "No se encontró",
                    StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new
                {
                    mensaje
                });
            }

            if (mensaje.Contains(
                    "Ya existe",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new
                {
                    mensaje
                });
            }

            return BadRequest(new
            {
                mensaje
            });
        }
    }
}