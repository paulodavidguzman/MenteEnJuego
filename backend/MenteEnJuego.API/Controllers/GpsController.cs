using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenteEnJuego.Servicios.Interfaces;
using System;
using System.Threading.Tasks;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,entrenador")]
    public class GpsController : ControllerBase
    {
        private readonly IGpsService _gpsService;

        public GpsController(IGpsService gpsService)
        {
            _gpsService = gpsService;
        }

        // POST: api/gps/importar-csv
        [HttpPost("importar-csv")]
        public async Task<IActionResult> ImportarDatosGps(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return BadRequest(new { mensaje = "Debe seleccionar un archivo CSV válido." });
            }

            // Validar que la extensión sea estrictamente .csv
            if (!archivo.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { mensaje = "Formato inválido. El sistema solo acepta archivos con extensión .csv" });
            }

            try
            {
                var exito = await _gpsService.ProcesarArchivoGpsCsvAsync(archivo);

                if (!exito) return BadRequest(new { mensaje = "El archivo está vacío o no contiene registros válidos." });

                return Ok(new { mensaje = "¡Sincronización Élite Exitosa! Los datos del dispositivo GPS impactaron en las métricas de las jugadoras." });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje =
                            "No fue posible procesar el archivo GPS."
                    });
            }
        }
    }
}
