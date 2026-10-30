
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.Entidades;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/evaluaciones-kinesiologicas")]
    [Authorize]
    public class EvaluacionesKinesiologicasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EvaluacionesKinesiologicasController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<StaffProfesional?> ObtenerProfesionalAsync()
        {
            var claim = User.FindFirstValue("idProfesional");

            if (!int.TryParse(claim, out int idProfesional))
                return null;

            return await _context.StaffProfesionales
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == idProfesional);
        }

        private static bool TienePermiso(StaffProfesional profesional)
        {
            var especialidad = profesional.Especialidad?
                .Trim()
                .ToLowerInvariant();

            return especialidad == "kinesiologo"
                || especialidad == "kinesiólogo"
                || especialidad == "fisioterapeuta"
                || especialidad == "medico"
                || especialidad == "médico clínico";
        }

        // GET api/evaluaciones-kinesiologicas/jugadora/10
        [HttpGet("jugadora/{idJugadora:int}")]
        public async Task<IActionResult> ObtenerPorJugadora(
            int idJugadora)
        {
            var profesional = await ObtenerProfesionalAsync();

            if (profesional == null || !TienePermiso(profesional))
                return Forbid();

            if (idJugadora <= 0)
                return BadRequest(new
                {
                    mensaje = "La jugadora indicada no es válida."
                });

            bool existe = await _context.Jugadoras
                .AnyAsync(j => j.Id == idJugadora);

            if (!existe)
                return NotFound(new
                {
                    mensaje = "La jugadora no existe."
                });

            var evaluaciones = await _context.EvaluacionesKinesiologicas
                .AsNoTracking()
                .Where(e => e.IdJugadora == idJugadora)
                .OrderByDescending(e => e.FechaEvaluacion)
                .Select(e => new
                {
                    e.Id,
                    e.IdJugadora,
                    Fecha = e.FechaEvaluacion,
                    e.RegionCorporal,
                    e.Lateralidad,
                    e.Dolor,
                    e.Movilidad,
                    e.LimitacionFuncional,
                    e.FaseRehabilitacion,
                    Tratamiento = e.TratamientoRealizado,
                    e.Objetivos,
                    e.ProximoControl,
                    e.Observaciones,
                    Profesional = e.Profesional != null
                        ? e.Profesional.Especialidad
                        : "No disponible"
                })
                .ToListAsync();

            return Ok(evaluaciones);
        }

        // POST api/evaluaciones-kinesiologicas
        [HttpPost]
        public async Task<IActionResult> Registrar(
            [FromBody] CrearEvaluacionKinesiologicaDto datos)
        {
            var profesional = await ObtenerProfesionalAsync();

            if (profesional == null || !TienePermiso(profesional))
                return Forbid();

            if (datos.IdJugadora <= 0)
                return BadRequest(new
                {
                    mensaje = "Seleccioná una jugadora válida."
                });

            bool existe = await _context.Jugadoras
                .AnyAsync(j => j.Id == datos.IdJugadora);

            if (!existe)
                return NotFound(new
                {
                    mensaje = "La jugadora no existe."
                });

            if (datos.Dolor.HasValue &&
                (datos.Dolor.Value < 0 || datos.Dolor.Value > 10))
                return BadRequest(new
                {
                    mensaje = "El dolor debe estar entre 0 y 10."
                });

            if (datos.Fecha == default)
                return BadRequest(new
                {
                    mensaje = "La fecha de evaluación es obligatoria."
                });

            var evaluacion = new EvaluacionKinesiologica
            {
                IdJugadora = datos.IdJugadora,
                IdProfesional = profesional.Id,
                FechaEvaluacion = datos.Fecha,
                RegionCorporal = datos.RegionCorporal.Trim(),
                Lateralidad = datos.Lateralidad,
                Dolor = datos.Dolor,
                Movilidad = datos.Movilidad,
                LimitacionFuncional = datos.LimitacionFuncional,
                FaseRehabilitacion = datos.FaseRehabilitacion,
                TratamientoRealizado = datos.Tratamiento,
                Objetivos = datos.Objetivos,
                ProximoControl = datos.ProximoControl,
                Observaciones = datos.Observaciones
            };

            _context.EvaluacionesKinesiologicas.Add(evaluacion);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Evaluación kinesiológica registrada correctamente.",
                id = evaluacion.Id
            });
        }
    }

    public class CrearEvaluacionKinesiologicaDto
    {
        [Range(1, int.MaxValue)]
        public int IdJugadora { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [MaxLength(100)]
        public string RegionCorporal { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Lateralidad { get; set; }

        [Range(0, 10)]
        public int? Dolor { get; set; }

        [MaxLength(500)]
        public string? Movilidad { get; set; }

        [MaxLength(100)]
        public string? LimitacionFuncional { get; set; }

        [MaxLength(100)]
        public string? FaseRehabilitacion { get; set; }

        public string? Tratamiento { get; set; }

        public string? Objetivos { get; set; }

        public DateTime? ProximoControl { get; set; }

        public string? Observaciones { get; set; }
    }
}
