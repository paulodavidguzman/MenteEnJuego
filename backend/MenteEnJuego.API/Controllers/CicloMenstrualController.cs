using Microsoft.AspNetCore.Mvc;
using MenteEnJuego.Servicios.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(
    Roles = "admin,entrenador,medico,jugadora")]
    public class CicloMenstrualController : ControllerBase
    {
        private readonly ICicloMenstrualService _cicloService;

        public CicloMenstrualController(ICicloMenstrualService cicloService)
        {
            _cicloService = cicloService;
        }

        // GET: api/ciclomenstrual/evaluar/13
        [HttpGet("evaluar/{diaCiclo:int}")]
        public IActionResult EvaluarFase(int diaCiclo)
        {
            if (diaCiclo < 1 || diaCiclo > 45)
            {
                return BadRequest(new { mensaje = "El número de día del ciclo menstrual debe estar comprendido entre 1 y 45." });
            }

            var analisisFisiologico = _cicloService.EvaluarFaseFisiologica(diaCiclo);
            return Ok(analisisFisiologico);
        }
    }
}
