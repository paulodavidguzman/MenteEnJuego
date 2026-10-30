using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MenteEnJuego.Servicios.Interfaces;
using System;
using System.Threading.Tasks;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador")]
    public class RendimientoController : ControllerBase
    {
        private readonly IRendimientoService _rendimientoService;

        // Inyección del servicio de rendimiento matemático que configuramos en Program.cs
        public RendimientoController(IRendimientoService rendimientoService)
        {
            _rendimientoService = rendimientoService;
        }

        // GET: api/rendimiento/acumulado-semanal?fechaLunes=2026-05-18
        [HttpGet("acumulado-semanal")]
        public async Task<IActionResult> GetAcumuladoSemanal([FromQuery] DateTime fechaLunes)
        {
            if (fechaLunes == default)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La fecha inicial de la semana no es válida."
                });
            }
            try
            {
                // Forzar a que la fecha consultada sea evaluada desde el inicio de su día
                var reporte = await _rendimientoService.ObtenerReporteAcumuladoSemanalAsync(fechaLunes.Date);

                if (reporte == null || reporte.Count == 0)
                {
                    return NotFound(new { mensaje = "No se encontraron jugadoras registradas en el plantel." });
                }

                return Ok(reporte);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje =
                            "No fue posible procesar las estadísticas semanales."
                    });
            }
        }
    }
}
