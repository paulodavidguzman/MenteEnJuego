using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador")]
    public class PlanificacionesTecnicasController
        : ControllerBase
    {
        private readonly IPlanificacionTecnicaService
            _planificacionService;

        public PlanificacionesTecnicasController(
            IPlanificacionTecnicaService planificacionService)
        {
            _planificacionService =
                planificacionService;
        }

        // ==================================================
        // GET: api/PlanificacionesTecnicas
        // ==================================================

        [HttpGet]
        public async Task<ActionResult<
            List<PlanificacionTecnicaDTO>>>
            ObtenerTodas()
        {
            var planificaciones =
                await _planificacionService
                    .ObtenerTodasAsync();

            return Ok(planificaciones);
        }

        // ==================================================
        // GET: api/PlanificacionesTecnicas/5
        // ==================================================

        [HttpGet("{id:int}")]
        public async Task<ActionResult<
            PlanificacionTecnicaDTO>>
            ObtenerPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la planificación no es válido."
                });
            }

            var planificacion =
                await _planificacionService
                    .ObtenerPorIdAsync(id);

            if (planificacion == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "No se encontró la planificación técnica solicitada."
                });
            }

            return Ok(planificacion);
        }

        // ==================================================
        // GET:
        // api/PlanificacionesTecnicas/sesion/5
        // ==================================================

        [HttpGet("sesion/{sesionDeportivaId:int}")]
        public async Task<ActionResult<
            PlanificacionTecnicaDTO>>
            ObtenerPorSesion(
                int sesionDeportivaId)
        {
            if (sesionDeportivaId <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la sesión deportiva no es válido."
                });
            }

            var planificacion =
                await _planificacionService
                    .ObtenerPorSesionAsync(
                        sesionDeportivaId);

            if (planificacion == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "La sesión deportiva no posee una planificación técnica."
                });
            }

            return Ok(planificacion);
        }

        // ==================================================
        // POST: api/PlanificacionesTecnicas
        // ==================================================

        [HttpPost]
        public async Task<ActionResult<
            PlanificacionTecnicaDTO>>
            Crear(
                [FromBody]
                CrearPlanificacionTecnicaDTO solicitud)
        {
            if (solicitud == null)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La solicitud no puede estar vacía."
                });
            }

            var usuarioResponsableId =
                ObtenerUsuarioIdAutenticado();

            if (!usuarioResponsableId.HasValue)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        mensaje =
                            "No fue posible identificar al usuario autenticado."
                    });
            }

            var resultado =
                await _planificacionService
                    .CrearAsync(
                        solicitud,
                        usuarioResponsableId);

            if (!resultado.Exito)
            {
                if (EsRecursoNoEncontrado(
                        resultado.Mensaje))
                {
                    return NotFound(new
                    {
                        mensaje =
                            resultado.Mensaje
                    });
                }

                if (EsConflicto(
                        resultado.Mensaje))
                {
                    return Conflict(new
                    {
                        mensaje =
                            resultado.Mensaje
                    });
                }

                return BadRequest(new
                {
                    mensaje =
                        resultado.Mensaje
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
        // PUT: api/PlanificacionesTecnicas/5
        // ==================================================

        [HttpPut("{id:int}")]
        public async Task<ActionResult<
            PlanificacionTecnicaDTO>>
            Actualizar(
                int id,
                [FromBody]
                ActualizarPlanificacionTecnicaDTO solicitud)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la planificación no es válido."
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
                await _planificacionService
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
                        mensaje =
                            resultado.Mensaje
                    });
                }

                return BadRequest(new
                {
                    mensaje =
                        resultado.Mensaje
                });
            }

            return Ok(resultado.Datos);
        }

        // ==================================================
        // PATCH:
        // api/PlanificacionesTecnicas/5/estado
        // ==================================================

        [HttpPatch("{id:int}/estado")]
        public async Task<ActionResult<
            PlanificacionTecnicaDTO>>
            CambiarEstado(
                int id,
                [FromBody]
                CambiarEstadoPlanificacionDTO solicitud)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la planificación no es válido."
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
                await _planificacionService
                    .CambiarEstadoAsync(
                        id,
                        solicitud);

            if (!resultado.Exito)
            {
                if (EsRecursoNoEncontrado(
                        resultado.Mensaje))
                {
                    return NotFound(new
                    {
                        mensaje =
                            resultado.Mensaje
                    });
                }

                return BadRequest(new
                {
                    mensaje =
                        resultado.Mensaje
                });
            }

            return Ok(resultado.Datos);
        }

        // ==================================================
        // IDENTIDAD DEL USUARIO AUTENTICADO
        // ==================================================

        private int? ObtenerUsuarioIdAutenticado()
        {
            var valorClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            /*
             * Compatibilidad con tokens que utilizan
             * un claim personalizado.
             */
            if (string.IsNullOrWhiteSpace(
                    valorClaim))
            {
                valorClaim =
                    User.FindFirstValue(
                        "idUsuario");
            }

            return int.TryParse(
                valorClaim,
                out var usuarioId)
                    ? usuarioId
                    : null;
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
                "ya posee",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}