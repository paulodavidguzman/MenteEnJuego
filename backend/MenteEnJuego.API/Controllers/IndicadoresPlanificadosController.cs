using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador")]
    public class IndicadoresPlanificadosController : ControllerBase
    {
        private readonly IIndicadorPlanificadoService
            _indicadorPlanificadoService;

        public IndicadoresPlanificadosController(
            IIndicadorPlanificadoService indicadorPlanificadoService)
        {
            _indicadorPlanificadoService =
                indicadorPlanificadoService;
        }

        // GET:
        // api/IndicadoresPlanificados/planificacion/1
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

            var indicadores =
                await _indicadorPlanificadoService
                    .ObtenerPorPlanificacionAsync(
                        planificacionTecnicaId);

            return Ok(indicadores);
        }

        // GET:
        // api/IndicadoresPlanificados/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del indicador no es válido."
                });
            }

            var indicador =
                await _indicadorPlanificadoService
                    .ObtenerPorIdAsync(id);

            if (indicador == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "No se encontró el indicador planificado solicitado."
                });
            }

            return Ok(indicador);
        }

        // POST:
        // api/IndicadoresPlanificados
        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] CrearIndicadorPlanificadoDTO solicitud)
        {
            var resultado =
                await _indicadorPlanificadoService
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
        // api/IndicadoresPlanificados/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(
            int id,
            [FromBody]
            ActualizarIndicadorPlanificadoDTO solicitud)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del indicador no es válido."
                });
            }

            var resultado =
                await _indicadorPlanificadoService
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
        // api/IndicadoresPlanificados/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del indicador no es válido."
                });
            }

            var resultado =
                await _indicadorPlanificadoService
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