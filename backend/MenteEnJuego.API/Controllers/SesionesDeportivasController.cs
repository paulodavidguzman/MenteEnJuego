using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;

namespace MenteEnJuego.API.Controllers
  
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SesionesDeportivasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SesionesDeportivasController(
            ApplicationDbContext context
        )
        {
            _context = context;
        }


        // ============================
        // LISTAR
        // ============================

        [HttpGet]
        [Authorize(Roles = "admin,entrenador,medico")]
        public async Task<ActionResult>
            ObtenerSesiones()
        {
            var sesiones =
                await _context
                .SesionesDeportivas
                 .AsNoTracking()
                .OrderByDescending(x => x.Fecha)
                .ToListAsync();

            return Ok(sesiones);
        }


        // ============================
        // LISTAR POR MICROCICLO
        // GET: api/SesionesDeportivas/microciclo/5
        // ============================

        [HttpGet("microciclo/{microcicloId:int}")]
        [Authorize(Roles = "admin,entrenador,medico")]
        public async Task<ActionResult> ObtenerPorMicrociclo(
            int microcicloId)
        {
            if (microcicloId <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador del microciclo no es válido."
                });
            }

            var existeMicrociclo =
                await _context.Microciclos
                    .AsNoTracking()
                    .AnyAsync(m => m.Id == microcicloId);

            if (!existeMicrociclo)
            {
                return NotFound(new
                {
                    mensaje =
                        "El microciclo solicitado no existe."
                });
            }

            var sesiones =
                await _context.SesionesDeportivas
                    .AsNoTracking()
                    .Where(s =>
                        s.MicrocicloId == microcicloId)
                    .OrderBy(s => s.Fecha)
                    .ThenBy(s => s.HoraInicio)
                    .Select(s => new
                    {
                        s.Id,
                        s.MicrocicloId,
                        s.TipoSesion,
                        s.Titulo,
                        s.Fecha,
                        s.HoraInicio,
                        s.HoraFin,
                        s.Lugar,
                        s.Objetivo,
                        s.Descripcion,
                        s.Estado
                    })
                    .ToListAsync();

            return Ok(sesiones);
        }

        // ============================
        // OBTENER UNA
        // ============================

        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin,entrenador,medico")]
        public async Task<ActionResult> Obtener(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la sesión no es válido."
                });
            }

            var sesion = await _context
                .SesionesDeportivas
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.TipoSesion,
                    x.Titulo,
                    x.Fecha,
                    x.HoraInicio,
                    x.HoraFin,
                    x.Lugar,
                    x.Objetivo,
                    x.Descripcion,
                    x.Estado,

                    Convocatorias = x.Convocatorias.Select(c => new
                    {
                        c.Id,
                        c.JugadoraId,
                        c.EstadoConvocatoria,
                        c.EstadoAsistencia,
                        c.Observaciones
                    }),

                    Formularios = x.Formularios.Select(f => new
                    {
                        f.Id,
                        f.JugadoraId,
                        f.TipoFormulario,
                        f.Estado,
                        f.FechaHabilitacion,
                        f.FechaRespuesta,
                        f.Observaciones
                    }),

                    Notificaciones = x.Notificaciones.Select(n => new
                    {
                        n.Id,
                        n.JugadoraId,
                        n.Canal,
                        n.TipoNotificacion,
                        n.Titulo,
                        n.Mensaje,
                        n.Estado,
                        n.FechaProgramada
                    }),

                    Alertas = x.Alertas.Select(a => new
                    {
                        a.Id,
                        a.JugadoraId,
                        a.TipoAlerta,
                        a.Nivel,
                        a.Titulo,
                        a.Mensaje,
                        a.Estado
                    })
                })
                .FirstOrDefaultAsync();

            if (sesion == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "La sesión deportiva no existe."
                });
            }

            return Ok(sesion);
        }

        // ============================
        // CREAR
        // ============================

        [HttpPost]
        [Authorize(Roles = "admin,entrenador")]
        public async Task<ActionResult> Crear(
     [FromBody] SesionDeportiva sesion)
        {
            if (sesion == null)
            {
                return BadRequest(new
                {
                    mensaje =
                        "Los datos de la sesión son obligatorios."
                });
            }

            if (sesion.Fecha == default)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La fecha de la sesión no es válida."
                });
            }

            if (sesion.HoraFin <= sesion.HoraInicio)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La hora de finalización debe ser posterior a la hora de inicio."
                });
            }

            sesion.Id = 0;
            sesion.Estado = "Programada";

            _context.SesionesDeportivas.Add(sesion);

            await _context.SaveChangesAsync();

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    mensaje =
                        "La sesión deportiva fue creada correctamente.",
                    sesion
                });
        }

        // ============================
        // EDITAR SESIÓN
        // PUT: api/SesionesDeportivas/5
        // ============================

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin,entrenador")]
        public async Task<ActionResult> Editar(
            int id,
           [FromBody] EditarSesionDeportivaDTO solicitud)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la sesión no es válido."
                });
            }

            if (solicitud == null)
            {
                return BadRequest(new
                {
                    mensaje =
                        "Los datos de la sesión son obligatorios."
                });
            }

            var sesion =
                await _context.SesionesDeportivas
                    .FirstOrDefaultAsync(s => s.Id == id);

            if (sesion == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "La sesión deportiva no existe."
                });
            }

            // Solo permitimos editar sesiones programadas
            if (!string.Equals(
                    sesion.Estado,
                    "Programada",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new
                {
                    mensaje =
                        "Solo las sesiones programadas pueden modificarse."
                });
            }

            if (solicitud.Fecha == default)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La fecha de la sesión no es válida."
                });
            }

            if (solicitud.HoraFin <= solicitud.HoraInicio)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La hora de finalización debe ser posterior a la hora de inicio."
                });
            }

            if (string.IsNullOrWhiteSpace(
                    solicitud.TipoSesion))
            {
                return BadRequest(new
                {
                    mensaje =
                        "El tipo de sesión es obligatorio."
                });
            }

            if (string.IsNullOrWhiteSpace(
                    solicitud.Titulo))
            {
                return BadRequest(new
                {
                    mensaje =
                        "El título es obligatorio."
                });
            }

            if (string.IsNullOrWhiteSpace(
                    solicitud.Lugar))
            {
                return BadRequest(new
                {
                    mensaje =
                        "El lugar es obligatorio."
                });
            }


            // ==========================
            // ACTUALIZAR
            // ==========================

            sesion.TipoSesion =
                solicitud.TipoSesion.Trim();

            sesion.Titulo =
                solicitud.Titulo.Trim();

            sesion.Fecha =
                solicitud.Fecha;

            sesion.HoraInicio =
                solicitud.HoraInicio;

            sesion.HoraFin =
                solicitud.HoraFin;

            sesion.Lugar =
                solicitud.Lugar.Trim();

            sesion.Objetivo =
                string.IsNullOrWhiteSpace(
                    solicitud.Objetivo)
                    ? null
                    : solicitud.Objetivo.Trim();

            sesion.Descripcion =
                string.IsNullOrWhiteSpace(
                    solicitud.Descripcion)
                    ? null
                    : solicitud.Descripcion.Trim();

            // Conservamos la relación existente.
            // No permitimos mover la sesión a otro microciclo desde este PUT.

            sesion.FechaActualizacion =
                DateTime.Now;

            await _context.SaveChangesAsync();


            return Ok(new
            {
                mensaje =
                    "La sesión deportiva fue modificada correctamente.",

                sesion = new
                {
                    sesion.Id,
                    sesion.MicrocicloId,
                    sesion.TipoSesion,
                    sesion.Titulo,
                    sesion.Fecha,
                    sesion.HoraInicio,
                    sesion.HoraFin,
                    sesion.Lugar,
                    sesion.Objetivo,
                    sesion.Descripcion,
                    sesion.Estado,
                    sesion.FechaActualizacion
                }
            });
        }

        // ============================
        // INICIAR SESIÓN
        // PUT: api/SesionesDeportivas/iniciar/5
        // ============================

        [Authorize(Roles = "admin,entrenador")]
        [HttpPut("iniciar/{id:int}")]
        public async Task<ActionResult> Iniciar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la sesión no es válido."
                });
            }

            var sesion =
                await _context.SesionesDeportivas
                    .FirstOrDefaultAsync(s => s.Id == id);

            if (sesion == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "La sesión deportiva no existe."
                });
            }

            if (sesion.Estado == "Cancelada")
            {
                return Conflict(new
                {
                    mensaje =
                        "Una sesión cancelada no puede iniciarse."
                });
            }

            if (sesion.Estado == "Finalizada")
            {
                return Conflict(new
                {
                    mensaje =
                        "Una sesión finalizada no puede iniciarse nuevamente."
                });
            }

            if (sesion.Estado == "EnCurso")
            {
                return Ok(new
                {
                    mensaje =
                        "La sesión ya se encuentra en curso.",
                    idSesion = sesion.Id,
                    estado = sesion.Estado
                });
            }

            if (sesion.Estado != "Programada")
            {
                return Conflict(new
                {
                    mensaje =
                        $"La sesión no puede iniciarse desde el estado {sesion.Estado}."
                });
            }

            sesion.Estado = "EnCurso";
            sesion.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje =
                    "La sesión deportiva fue iniciada correctamente.",
                idSesion = sesion.Id,
                estado = sesion.Estado
            });
        }


        // ============================
        // FINALIZAR SESIÓN
        // PUT: api/SesionesDeportivas/finalizar/5
        // ============================

        [Authorize(Roles = "admin,entrenador")]
        [HttpPut("finalizar/{id:int}")]
        public async Task<ActionResult> Finalizar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la sesión no es válido."
                });
            }

            var sesion =
                await _context.SesionesDeportivas
                    .FirstOrDefaultAsync(s => s.Id == id);

            if (sesion == null)
            {
                return NotFound(new
                {
                    mensaje =
                        "La sesión deportiva no existe."
                });
            }

            if (sesion.Estado == "Cancelada")
            {
                return Conflict(new
                {
                    mensaje =
                        "Una sesión cancelada no puede finalizarse."
                });
            }

            if (sesion.Estado == "Finalizada")
            {
                return Ok(new
                {
                    mensaje =
                        "La sesión ya se encuentra finalizada.",
                    idSesion = sesion.Id,
                    estado = sesion.Estado
                });
            }

            if (sesion.Estado == "Programada")
            {
                return Conflict(new
                {
                    mensaje =
                        "La sesión debe iniciarse antes de poder finalizarse."
                });
            }

            if (sesion.Estado != "EnCurso")
            {
                return Conflict(new
                {
                    mensaje =
                        $"La sesión no puede finalizarse desde el estado {sesion.Estado}."
                });
            }

            sesion.Estado = "Finalizada";
            sesion.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje =
                    "La sesión deportiva fue finalizada correctamente.",
                idSesion = sesion.Id,
                estado = sesion.Estado
            });
        }

        // ============================
        // ELIMINAR
        // ============================

        [Authorize(Roles = "admin,entrenador")]
        [HttpPut("cancelar/{id:int}")]
        public async Task<ActionResult> Cancelar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El identificador de la sesión no es válido."
                });
            }

            var sesion = await _context.SesionesDeportivas
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sesion == null)
            {
                return NotFound(new
                {
                    mensaje = "La sesión deportiva no existe."
                });
            }

            if (sesion.Estado == "Finalizada")
            {
                return Conflict(new
                {
                    mensaje =
                        "Una sesión finalizada no puede cancelarse."
                });
            }

            if (sesion.Estado == "Cancelada")
            {
                return Ok(new
                {
                    mensaje = "La sesión ya estaba cancelada."
                });
            }

            sesion.Estado = "Cancelada";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje =
                    "La sesión fue cancelada correctamente y se conservó su historial.",
                idSesion = sesion.Id,
                estado = sesion.Estado
            });
        }
    }
}