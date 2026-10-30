using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.Entidades;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MenteEnJuego.Dominio.DTOs;


namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class CuentasAdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CuentasAdminController(ApplicationDbContext context)
        {
            _context = context;
        }
      

        private string CalcularSector(string especialidad)
        {
            string[] rolesTecnicos =
            {
        "Profe",
        "Entrenador",
        "DirectorTecnico",
        "AyudanteCampo",
        "PreparadorFisico",
        "EntrenadorArqueras"
    };

            return rolesTecnicos.Contains(especialidad) ? "Tecnico" : "Salud";
        }

        // 1. GET: api/cuentasadmin (Listar todos los profesionales técnicos y médicos)
        [HttpGet]
        public async Task<IActionResult> ObtenerPersonalClub()
        {
            try
            {
                var personal = await _context.StaffProfesionales
                    .Select(p => new
                    {
                        p.Id,
                        p.Dni,
                        p.Apellido,
                        p.Nombre,
                        p.Especialidad,
                        p.Sector,
                        p.Telefono,
                        p.Email,
                        p.FotoBase64,

                        IdUsuario = _context.Usuarios
                            .Where(u =>
                                u.Email.ToLower() ==
                                (p.Email ?? string.Empty).ToLower())
                            .Select(u => (int?)u.Id)
                            .FirstOrDefault(),

                        CuentaActiva = _context.Usuarios
                            .Where(u =>
                                u.Email.ToLower() ==
                                (p.Email ?? string.Empty).ToLower())
                            .Select(u => (bool?)u.Activo)
                            .FirstOrDefault()
                    })
                    .OrderBy(p => p.Apellido)
                    .ThenBy(p => p.Nombre)
                    .ToListAsync();

                return Ok(personal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al listar el personal.",
                    detalle = ex.Message
                });
            }
        }

        // GET: api/cuentasadmin/existe-dni/{dni}
        [HttpGet("existe-dni/{dni}")]
        public async Task<IActionResult> ExisteDni(string dni)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dni))
                {
                    return BadRequest(new
                    {
                        mensaje = "El DNI es obligatorio."
                    });
                }

                // Dejamos solamente números
                string dniLimpio = new string(
                    dni.Where(char.IsDigit).ToArray()
                );

                if (dniLimpio.Length < 7 || dniLimpio.Length > 8)
                {
                    return BadRequest(new
                    {
                        mensaje = "El DNI debe contener entre 7 y 8 dígitos."
                    });
                }

                bool existe = await _context.StaffProfesionales
                    .AnyAsync(p => p.Dni == dniLimpio);

                return Ok(new
                {
                    existe = existe
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    mensaje = "No fue posible verificar el DNI."
                });
            }
        }



        // 2. POST: api/cuentasadmin/alta-profesional (Alta de Médico, Kine, Nutri, Psico, Profe)
        [HttpPost("alta-profesional")]
        public async Task<IActionResult> AltaPersonal(
    [FromBody] CrearProfesionalDTO solicitud)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var nuevoProfesional =
                solicitud.Profesional;

            if (nuevoProfesional == null)
            {
                return BadRequest(new
                {
                    mensaje =
                        "Los datos del profesional son obligatorios."
                });
            }

            var email =
                nuevoProfesional.Email?
                    .Trim()
                    .ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new
                {
                    mensaje =
                        "El correo electrónico es obligatorio."
                });
            }

            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var existeDni =
                    await _context.StaffProfesionales
                        .AnyAsync(p =>
                            p.Dni == nuevoProfesional.Dni);

                if (existeDni)
                {
                    return Conflict(new
                    {
                        mensaje =
                            "El DNI ya pertenece a un profesional del club."
                    });
                }

                var existeEmail =
                    await _context.StaffProfesionales
                        .AnyAsync(p =>
                            p.Email != null &&
                            p.Email.ToLower() == email)
                    ||
                    await _context.Usuarios
                        .AnyAsync(u =>
                            u.Email.ToLower() == email);

                if (existeEmail)
                {
                    return Conflict(new
                    {
                        mensaje =
                            "El correo electrónico ya se encuentra registrado."
                    });
                }

                nuevoProfesional.Email = email;
                nuevoProfesional.Sector =
                    CalcularSector(
                        nuevoProfesional.Especialidad);

                _context.StaffProfesionales
                    .Add(nuevoProfesional);

                await _context.SaveChangesAsync();

                var usuarioEnlazado =
                    new Usuario
                    {
                        NombreUsuario =
                            $"{nuevoProfesional.Nombre.Substring(0, 1)}" +
                            $"{nuevoProfesional.Apellido}"
                                .ToLowerInvariant(),

                        Email = email,

                        Rol =
                            nuevoProfesional.Sector == "Tecnico"
                                ? "entrenador"
                                : "medico",

                        PasswordHash =
                            BCryptNet.HashPassword(
                                solicitud.PasswordInicial),

                        Activo = true
                    };

                _context.Usuarios.Add(
                    usuarioEnlazado);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return StatusCode(
                    StatusCodes.Status201Created,
                    new
                    {
                        mensaje =
                            $"Cuenta creada correctamente para " +
                            $"{nuevoProfesional.Apellido}.",

                        idProfesional =
                            nuevoProfesional.Id,

                        idUsuario =
                            usuarioEnlazado.Id
                    });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje =
                            "No fue posible crear el profesional y su cuenta."
                    });
            }
        }

        // PUT: api/cuentasadmin/resetear-clave/{idUsuario}
        [HttpPut("resetear-clave/{idUsuario:int}")]
        public async Task<IActionResult> ResetearClave(
            int idUsuario,
            [FromBody] ResetearClaveDTO solicitud)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == idUsuario);

                if (usuario == null)
                {
                    return NotFound(new
                    {
                        mensaje = "La cuenta de usuario no existe."
                    });
                }

                usuario.PasswordHash =
                    BCryptNet.HashPassword(
                        solicitud.NuevaPassword);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje =
                        "La contraseña fue restablecida correctamente."
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje =
                            "No fue posible restablecer la contraseña."
                    });
            }
        }


        // 4. PUT: api/cuentasadmin/modificar-profesional/{id}
        [HttpPut("modificar-profesional/{id}")]
        public async Task<IActionResult> ModificarProfesional(int id, [FromBody] StaffProfesional datos)
        {
            if (datos == null)
                return BadRequest(new { mensaje = "Datos insuficientes." });

            try
            {
                var profesional = await _context.StaffProfesionales.FindAsync(id);

                if (profesional == null)
                    return NotFound(new { mensaje = "El profesional no existe." });

                var emailAnterior = profesional.Email?.Trim().ToLower();
                var emailNuevo = datos.Email?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(emailNuevo))
                    return BadRequest(new { mensaje = "El email es obligatorio." });

                var emailRepetido = await _context.StaffProfesionales
                    .AnyAsync(p => p.Email.ToLower() == emailNuevo && p.Id != id);

                if (emailRepetido)
                    return BadRequest(new { mensaje = "Ese correo ya pertenece a otro profesional." });

                profesional.Nombre = datos.Nombre;
                profesional.Apellido = datos.Apellido;
                profesional.Email = emailNuevo;
                profesional.Especialidad = datos.Especialidad;

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == emailAnterior);

                if (usuario != null)
                {
                    usuario.NombreUsuario = $"{datos.Nombre.Substring(0, 1)}{datos.Apellido}".ToLower();
                    usuario.Email = emailNuevo;

                    usuario.Rol = CalcularSector(datos.Especialidad) == "Tecnico"
                    ? "entrenador"
                    : "medico";
                }
                profesional.Especialidad = datos.Especialidad;
                profesional.Sector = CalcularSector(datos.Especialidad);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Datos del profesional actualizados con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al modificar profesional.", detalle = ex.Message });
            }
        }

        // 4. DELETE (LÓGICO): api/cuentasadmin/suspender-cuenta/{idUsuario}

        // PUT: api/cuentasadmin/suspender-cuenta/{idUsuario}
        [HttpPut("suspender-cuenta/{idUsuario}")]
        public async Task<IActionResult> SuspenderCuentaPersonal(int idUsuario)
        {
            try
            {
                if (idUsuario <= 0)
                {
                    return BadRequest(new
                    {
                        mensaje = "El identificador del usuario no es válido."
                    });
                }

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == idUsuario);

                if (usuario == null)
                {
                    return NotFound(new
                    {
                        mensaje = "La cuenta de usuario no existe."
                    });
                }

                if (!usuario.Activo)
                {
                    return Ok(new
                    {
                        mensaje = "La cuenta ya se encontraba suspendida."
                    });
                }

                usuario.Activo = false;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Cuenta suspendida correctamente. El profesional ya no podrá iniciar sesión.",
                    idUsuario = usuario.Id,
                    activo = usuario.Activo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al suspender la cuenta.",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


        // PUT: api/cuentasadmin/reactivar-cuenta/{idUsuario}
        [HttpPut("reactivar-cuenta/{idUsuario}")]
        public async Task<IActionResult> ReactivarCuentaPersonal(int idUsuario)
        {
            try
            {
                if (idUsuario <= 0)
                {
                    return BadRequest(new
                    {
                        mensaje = "El identificador del usuario no es válido."
                    });
                }

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == idUsuario);

                if (usuario == null)
                {
                    return NotFound(new
                    {
                        mensaje = "La cuenta de usuario no existe."
                    });
                }

                // Si ya está activa, no hacemos cambios innecesarios
                if (usuario.Activo)
                {
                    return Ok(new
                    {
                        mensaje = "La cuenta ya se encontraba activa.",
                        idUsuario = usuario.Id,
                        activo = usuario.Activo
                    });
                }

                // REACTIVACIÓN DE LA CUENTA
                usuario.Activo = true;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Cuenta reactivada correctamente. El profesional podrá volver a iniciar sesión.",
                    idUsuario = usuario.Id,
                    activo = usuario.Activo
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    mensaje = "No fue posible reactivar la cuenta del profesional."
                });
            }
        }
        /*  [HttpDelete("suspender-cuenta/{idUsuario}")]
          public async Task<IActionResult> SuspenderCuentaPersonal(int idUsuario)
          {
              try
              {
                  var usuario = await _context.Usuarios.FindAsync(idUsuario);
                  if (usuario == null) return NotFound("Usuario no encontrado.");

                  // BAJA LÓGICA DE LA CUENTA: Bloquea el login sin romper sus firmas en los partes médicos pasados
                  usuario.Activo = false;

                  await _context.SaveChangesAsync();
                  return Ok(new { mensaje = "Cuenta suspendida con éxito (Baja Lógica aplicada). Acceso denegado a los paneles." });
              }
              catch (Exception ex)
              {
                  return StatusCode(500, new { mensaje = "Error al aplicar la suspensión.", detalle = ex.Message });
              }
          }*/
    }
}
