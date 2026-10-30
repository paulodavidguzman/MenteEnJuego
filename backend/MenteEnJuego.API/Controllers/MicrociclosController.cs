using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador")]
    public class MicrociclosController : ControllerBase
    {
        private readonly IMicrocicloService
            _microcicloService;

        public MicrociclosController(
            IMicrocicloService microcicloService)
        {
            _microcicloService =
                microcicloService;
        }

        // ============================================
        // GET: api/Microciclos
        // ============================================

        [HttpGet]
        public async Task<ActionResult<
            List<MicrocicloDTO>>>
            ObtenerTodos()
        {
            var microciclos =
                await _microcicloService
                    .ObtenerTodosAsync();

            return Ok(microciclos);
        }

        // ============================================
        // GET: api/Microciclos/5
        // ============================================

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MicrocicloDTO>>
            ObtenerPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del microciclo no es válido."
                });
            }

            var microciclo =
                await _microcicloService
                    .ObtenerPorIdAsync(id);

            if (microciclo == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "No se encontró el microciclo solicitado."
                });
            }

            return Ok(microciclo);
        }

        // ============================================
        // POST: api/Microciclos
        // ============================================

        [HttpPost]
        public async Task<ActionResult<MicrocicloDTO>>
            Crear(
                [FromBody]
                CrearMicrocicloDTO solicitud)
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
                await _microcicloService
                    .CrearAsync(solicitud);

            if (!resultado.Exito)
            {
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

        // ============================================
        // PUT: api/Microciclos/5
        // ============================================

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MicrocicloDTO>>
            Actualizar(
                int id,
                [FromBody]
                ActualizarMicrocicloDTO solicitud)
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
                await _microcicloService
                    .ActualizarAsync(
                        id,
                        solicitud);

            if (!resultado.Exito)
            {
                if (resultado.Mensaje.Contains(
                        "No se encontró",
                        StringComparison.OrdinalIgnoreCase))
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

        // ============================================
        // PATCH: api/Microciclos/5/estado
        // ============================================

        [HttpPatch("{id:int}/estado")]
        public async Task<ActionResult<MicrocicloDTO>>
            CambiarEstado(
                int id,
                [FromBody]
                CambiarEstadoMicrocicloDTO solicitud)
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
                await _microcicloService
                    .CambiarEstadoAsync(
                        id,
                        solicitud);

            if (!resultado.Exito)
            {
                if (resultado.Mensaje.Contains(
                        "No se encontró",
                        StringComparison.OrdinalIgnoreCase))
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
    }
}