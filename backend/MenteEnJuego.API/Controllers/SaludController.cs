using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MenteEnJuego.Servicios.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SaludController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISaludPermissionService _permissionService;

        public SaludController(
      ApplicationDbContext context,
      ISaludPermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        private async Task<StaffProfesional?>
     ObtenerStaffAutenticadoAsync()
        {
            var idProfesionalClaim =
                User.FindFirstValue("idProfesional");

            if (!int.TryParse(
                    idProfesionalClaim,
                    out var idProfesional))
            {
                return null;
            }

            return await _context.StaffProfesionales
                .AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    s.Id == idProfesional);
        }

        private bool TieneEspecialidadPermitida(
    StaffProfesional staff,
    params string[] especialidadesPermitidas)
        {
            var especialidadActual =
                staff.Especialidad?
                    .Trim()
                    .ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(especialidadActual))
            {
                return false;
            }

            return especialidadesPermitidas.Any(e =>
                string.Equals(
                    e.Trim(),
                    especialidadActual,
                    StringComparison.OrdinalIgnoreCase));
        }

        //ficha por jugadora
        [HttpGet("ficha-jugadora/{idJugadora:int}")]
        public async Task<IActionResult> ObtenerFichaJugadora(
    int idJugadora)
        {
            if (idJugadora <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la jugadora no es válido."
                });
            }

            var staff =
                await ObtenerStaffAutenticadoAsync();

            if (staff == null)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        mensaje =
                            "No se encontró un profesional activo asociado al usuario autenticado."
                    });
            }

            if (!TieneEspecialidadPermitida(
                    staff,
                    "medico",
                    "kinesiologo",
                    "fisioterapeuta",
                    "nutricionista",
                    "psicologo",
                    "psicologa"))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        mensaje =
                            "No tiene permisos para consultar la ficha de salud de la jugadora."
                    });
            }

            var ficha = await _context.Jugadoras
                .AsNoTracking()
                .Where(j => j.Id == idJugadora)
                .Select(j => new
                {
                    j.Id,
                    j.Apellido,
                    j.Nombre,
                    j.Dni,
                    j.FechaNacimiento,

                    Edad =
                        DateTime.Today.Year -
                        j.FechaNacimiento.Year -
                        (DateTime.Today.DayOfYear <
                         j.FechaNacimiento.DayOfYear
                            ? 1
                            : 0),

                    j.Posicion,
                    j.Equipo,
                    j.Telefono
                })
                .FirstOrDefaultAsync();

            if (ficha == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "La jugadora no existe."
                });
            }

            return Ok(ficha);
        }
        // 1. POST: api/salud/lesion (Kinesiólogos y Médicos registran un parte de baja médica)
        [HttpPost("lesion")]
        public async Task<IActionResult> RegistrarLesion(
    [FromBody] HistorialLesion nuevaLesion)
    
        {
           

            var staff = await ObtenerStaffAutenticadoAsync();

            if (staff == null)
            {
                return Forbid(
                    "No se encontró un profesional autenticado.");
            }

            var rol =
                User.FindFirstValue(ClaimTypes.Role);

            var permisos =
                _permissionService.ObtenerPermisos(
                    rol ?? string.Empty,
                    staff.Especialidad);

            if (!permisos.PuedeRegistrarLesion)
            {
                return Forbid(
                    "No posee permisos para registrar lesiones.");
            }
            if (nuevaLesion == null) return BadRequest("Datos clínicos inválidos.");

            try
            {
                // Verificar si la futbolista existe en el club
                var existeJugadora = await _context.Jugadoras.AnyAsync(j => j.Id == nuevaLesion.IdJugadora);
                if (!existeJugadora) return NotFound("La jugadora especificada no existe.");

                nuevaLesion.FechaInicio = DateTime.Today;

                _context.HistorialLesiones.Add(nuevaLesion);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "¡Parte médico registrado con éxito! Jugadora ingresada al proceso de readaptación." });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje =
                            "Error al guardar el parte médico."
                    });
            }
        }

        // 2. PUT: api/salud/readaptacion/{idLesion} (Kinesiólogos actualizan el % de avance en campo)
        [HttpPut("readaptacion/{idLesion:int}")]
        public async Task<IActionResult> ActualizarProgresoFisico(
     int idLesion,
     [FromQuery] int porcentajeAlta,
     [FromQuery] string? notasEvolucion)
        {
            var staff = await ObtenerStaffAutenticadoAsync();

            if (staff == null)
            {
                return Forbid(
                    "No se encontró un profesional autenticado.");
            }

            var rol =
                User.FindFirstValue(ClaimTypes.Role);

            var permisos =
                _permissionService.ObtenerPermisos(
                    rol ?? string.Empty,
                    staff.Especialidad);

            if (!permisos.PuedeActualizarReadaptacion)
            {
                return Forbid(
                    "No posee permisos para actualizar el proceso de readaptación.");
            }
            if (porcentajeAlta < 0 || porcentajeAlta > 100)
                return BadRequest("El porcentaje de readaptación debe estar comprendido entre 0 y 100.");

            try
            {
                var lesion = await _context.HistorialLesiones.FindAsync(idLesion);
                if (lesion == null) return NotFound("El registro clínico especificado no existe.");

                lesion.EstadoReadaptacion = porcentajeAlta;
                if (!string.IsNullOrEmpty(notasEvolucion)) lesion.Descripcion += $" | Ev: {notasEvolucion}";

                // Si llega al 100%, se asienta automáticamente la fecha de alta real
                if (porcentajeAlta == 100) lesion.FechaAltaReal = DateTime.Today;

                await _context.SaveChangesAsync();
                return Ok(new { mensaje = $"Progreso actualizado. La jugadora se encuentra al {porcentajeAlta}% de su aptitud física." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la evolución.", detalle = ex.Message });
            }
        }

        // 3. POST: api/salud/nutricion (Nutricionista registra control periódico de peso e índices)
        [HttpPost("nutricion")]
        public async Task<IActionResult> RegistrarControlNutricional(
     [FromBody] EvaluacionNutricional control)
        {
            if (control == null)
                return BadRequest("Datos antropométricos insuficientes.");

            var staff = await ObtenerStaffAutenticadoAsync();

            if (staff == null)
            {
                return Forbid(
                    "No se encontró un profesional autenticado.");
            }

            var rol =
                User.FindFirstValue(ClaimTypes.Role);

            var permisos =
                _permissionService.ObtenerPermisos(
                    rol ?? string.Empty,
                    staff.Especialidad);

            if (!permisos.PuedeRegistrarControlNutricional)
            {
                return Forbid(
                    "No posee permisos para registrar controles nutricionales.");
            }
            try
            {
                var existeJugadora = await _context.Jugadoras.AnyAsync(j => j.Id == control.IdJugadora);
                if (!existeJugadora) return NotFound("La jugadora especificada no existe.");

                control.Fecha = DateTime.Today;

                _context.EvaluacionesNutricionales.Add(control);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Evolución nutricional y composición corporal registradas correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al guardar el registro nutricional.", detalle = ex.Message });
            }
        }

        [HttpGet("lesiones")]
        public async Task<IActionResult> ObtenerLesiones()
        {
            var staff = await ObtenerStaffAutenticadoAsync();

            if (staff == null)
                return Forbid();

            if (!TieneEspecialidadPermitida(
                staff,
                "medico",
                "Médico Clínico",
                "kinesiologo",
                "Kinesiólogo",
                "fisioterapeuta"))
            {
                return Forbid();
            }

            var lesiones = await _context.HistorialLesiones
                .AsNoTracking()
                .OrderByDescending(l => l.FechaInicio)
                .ToListAsync();

            return Ok(lesiones);
        }

    }
}
