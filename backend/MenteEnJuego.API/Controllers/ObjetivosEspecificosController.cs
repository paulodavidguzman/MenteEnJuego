using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador")]
    public class ObjetivosEspecificosController
        : ControllerBase
    {
        private readonly IObjetivoEspecificoService
            _objetivoService;

        public ObjetivosEspecificosController(
            IObjetivoEspecificoService objetivoService)
        {
            _objetivoService = objetivoService;
        }

        // ==================================================
        // GET:
        // api/ObjetivosEspecificos/planificacion/5
        // ==================================================

        [HttpGet("planificacion/{planificacionTecnicaId:int}")]
        public async Task<ActionResult<
            List<ObjetivoEspecificoDTO>>>
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

            var objetivos =
                await _objetivoService
                    .ObtenerPorPlanificacionAsync(
                        planificacionTecnicaId);

            return Ok(objetivos);
        }

        // ==================================================
        // GET:
        // api/ObjetivosEspecificos/5
        // ==================================================

        [HttpGet("{id:int}")]
        public async Task<ActionResult<
            ObjetivoEspecificoDTO>>
            ObtenerPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del objetivo no es válido."
                });
            }

            var objetivo =
                await _objetivoService
                    .ObtenerPorIdAsync(id);

            if (objetivo == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "No se encontró el objetivo específico solicitado."
                });
            }

            return Ok(objetivo);
        }

        // ==================================================
        // POST:
        // api/ObjetivosEspecificos
        // ==================================================

        [HttpPost]
        public async Task<ActionResult<
            ObjetivoEspecificoDTO>>
            Crear(
                [FromBody]
                CrearObjetivoEspecificoDTO solicitud)
        {
            if (solicitud == null)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La solicitud no puede estar vacía."
                });
            }

            var resultado =
                await _objetivoService
                    .CrearAsync(solicitud);

            if (!resultado.Exito)
            {
                if (EsRecursoNoEncontrado(
                        resultado.Mensaje))
                {
                    return NotFound(new
                    {
                        mensaje = resultado.Mensaje
                    });
                }

                if (EsConflicto(
                        resultado.Mensaje))
                {
                    return Conflict(new
                    {
                        mensaje = resultado.Mensaje
                    });
                }

                return BadRequest(new
                {
                    mensaje = resultado.Mensaje
                });
            }

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new
                {
                    id = resultado.Datos!.Id
                },
                resultado.Datos);
        }

        // ==================================================
        // PUT:
        // api/ObjetivosEspecificos/5
        // ==================================================

        [HttpPut("{id:int}")]
        public async Task<ActionResult<
            ObjetivoEspecificoDTO>>
            Actualizar(
                int id,
                [FromBody]
                ActualizarObjetivoEspecificoDTO solicitud)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del objetivo no es válido."
                });
            }

            if (solicitud == null)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La solicitud no puede estar vacía."
                });
            }

            var resultado =
                await _objetivoService
                    .ActualizarAsync(
                        id,
                        solicitud);

            if (!resultado.Exito)
            {
                if (EsRecursoNoEncontrado(
                        resultado.Mensaje))
                {
                    return NotFound(new
                    {
                        mensaje = resultado.Mensaje
                    });
                }

                if (EsConflicto(
                        resultado.Mensaje))
                {
                    return Conflict(new
                    {
                        mensaje = resultado.Mensaje
                    });
                }

                return BadRequest(new
                {
                    mensaje = resultado.Mensaje
                });
            }

            return Ok(resultado.Datos);
        }

        // ==================================================
        // PATCH:
        // api/ObjetivosEspecificos/5/evaluacion
        // ==================================================

        [HttpPatch("{id:int}/evaluacion")]
        public async Task<ActionResult<
            ObjetivoEspecificoDTO>>
            Evaluar(
                int id,
                [FromBody]
                EvaluarObjetivoEspecificoDTO solicitud)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del objetivo no es válido."
                });
            }

            if (solicitud == null)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La solicitud no puede estar vacía."
                });
            }

            var resultado =
                await _objetivoService
                    .EvaluarAsync(
                        id,
                        solicitud);

            if (!resultado.Exito)
            {
                if (EsRecursoNoEncontrado(
                        resultado.Mensaje))
                {
                    return NotFound(new
                    {
                        mensaje = resultado.Mensaje
                    });
                }

                return BadRequest(new
                {
                    mensaje = resultado.Mensaje
                });
            }

            return Ok(resultado.Datos);
        }

        // ==================================================
        // DELETE:
        // api/ObjetivosEspecificos/5
        // ==================================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult>
            Eliminar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del objetivo no es válido."
                });
            }

            var resultado =
                await _objetivoService
                    .EliminarAsync(id);

            if (!resultado.Exito)
            {
                if (EsRecursoNoEncontrado(
                        resultado.Mensaje))
                {
                    return NotFound(new
                    {
                        mensaje = resultado.Mensaje
                    });
                }

                return BadRequest(new
                {
                    mensaje = resultado.Mensaje
                });
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje
            });
        }

        // ==================================================
        // CLASIFICACIÓN DE RESPUESTAS
        // ==================================================

        private static bool EsRecursoNoEncontrado(
            string mensaje)
        {
            return mensaje.Contains(
                "No se encontró",
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsConflicto(
            string mensaje)
        {
            return mensaje.Contains(
                "mismo orden",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}