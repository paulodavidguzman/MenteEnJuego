using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador")]
    public class StaffPlanificacionesController : ControllerBase
    {
        private readonly IStaffPlanificacionService
            _staffPlanificacionService;

        public StaffPlanificacionesController(
            IStaffPlanificacionService staffPlanificacionService)
        {
            _staffPlanificacionService =
                staffPlanificacionService;
        }

        [HttpGet("planificacion/{planificacionTecnicaId:int}")]
        public async Task<IActionResult>
            ObtenerPorPlanificacion(
                int planificacionTecnicaId)
        {
            if (planificacionTecnicaId <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la planificación no es válido."
                });
            }

            var staff =
                await _staffPlanificacionService
                    .ObtenerPorPlanificacionAsync(
                        planificacionTecnicaId);

            return Ok(staff);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult>
            ObtenerPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la asignación no es válido."
                });
            }

            var asignacion =
                await _staffPlanificacionService
                    .ObtenerPorIdAsync(id);

            if (asignacion == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "No se encontró la asignación de staff solicitada."
                });
            }

            return Ok(asignacion);
        }

        [HttpPost]
        public async Task<IActionResult>
            Crear(
                [FromBody]
                CrearStaffPlanificacionDTO solicitud)
        {
            var resultado =
                await _staffPlanificacionService
                    .CrearAsync(solicitud);

            if (!resultado.Exito)
            {
                return ResolverError(
                    resultado.Mensaje);
            }

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new
                {
                    id = resultado.Datos!.Id
                },
                new
                {
                    mensaje = resultado.Mensaje,
                    datos = resultado.Datos
                });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult>
            Actualizar(
                int id,
                [FromBody]
                ActualizarStaffPlanificacionDTO solicitud)
        {
            var resultado =
                await _staffPlanificacionService
                    .ActualizarAsync(
                        id,
                        solicitud);

            if (!resultado.Exito)
            {
                return ResolverError(
                    resultado.Mensaje);
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                datos = resultado.Datos
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult>
            Eliminar(int id)
        {
            var resultado =
                await _staffPlanificacionService
                    .EliminarAsync(id);

            if (!resultado.Exito)
            {
                return ResolverError(
                    resultado.Mensaje);
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje
            });
        }

        private IActionResult ResolverError(
            string mensaje)
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
                    StringComparison.OrdinalIgnoreCase)
                ||
                mensaje.Contains(
                    "repetido",
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