using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador")]
    public class ContenidosPlanificadosController : ControllerBase
    {
        private readonly IContenidoPlanificadoService
            _contenidoPlanificadoService;

        public ContenidosPlanificadosController(
            IContenidoPlanificadoService contenidoPlanificadoService)
        {
            _contenidoPlanificadoService =
                contenidoPlanificadoService;
        }

        // GET:
        // api/ContenidosPlanificados/planificacion/1
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
                        "El identificador de la planificación técnica no es válido."
                });
            }

            var contenidos =
                await _contenidoPlanificadoService
                    .ObtenerPorPlanificacionAsync(
                        planificacionTecnicaId);

            return Ok(contenidos);
        }

        // GET:
        // api/ContenidosPlanificados/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult>
            ObtenerPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del contenido no es válido."
                });
            }

            var contenido =
                await _contenidoPlanificadoService
                    .ObtenerPorIdAsync(id);

            if (contenido == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "No se encontró el contenido planificado solicitado."
                });
            }

            return Ok(contenido);
        }

        // POST:
        // api/ContenidosPlanificados
        [HttpPost]
        public async Task<IActionResult>
            Crear(
                [FromBody]
                CrearContenidoPlanificadoDTO solicitud)
        {
            var resultado =
                await _contenidoPlanificadoService
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
                resultado.Datos);
        }

        // PUT:
        // api/ContenidosPlanificados/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult>
            Actualizar(
                int id,
                [FromBody]
                ActualizarContenidoPlanificadoDTO solicitud)
        {
            var resultado =
                await _contenidoPlanificadoService
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

        // DELETE:
        // api/ContenidosPlanificados/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult>
            Eliminar(int id)
        {
            var resultado =
                await _contenidoPlanificadoService
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