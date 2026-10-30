using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.Entidades;
using BCryptNet = BCrypt.Net.BCrypt;
using System;
using System.Linq;
using System.Threading.Tasks;
using MenteEnJuego.Dominio.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class JugadorasAdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JugadorasAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/jugadorasadmin
        [HttpGet]
        public async Task<IActionResult> ObtenerPlantelCompleto()
        {
            try
            {
                var listado = await _context.Jugadoras
                    .OrderBy(j => j.Apellido)
                    .ToListAsync();
                return Ok(listado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al recuperar el plantel.", detalle = ex.Message });
            }
        }


        // VALIDAR DNI ANTES DE REGISTRAR UNA JUGADORA
        // GET: api/JugadorasAdmin/existe-dni/44111222
        [HttpGet("existe-dni/{dni}")]
        public async Task<IActionResult> ExisteDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return BadRequest(new
                {
                    mensaje = "Debe ingresar un DNI válido."
                });
            }

            // Trabajamos solamente con números
            var dniLimpio = new string(dni.Where(char.IsDigit).ToArray());

            if (dniLimpio.Length < 7 || dniLimpio.Length > 9)
            {
                return BadRequest(new
                {
                    mensaje = "El DNI debe contener entre 7 y 9 dígitos."
                });
            }

            var jugadora = await _context.Jugadoras
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Dni == dniLimpio);

            if (jugadora != null)
            {
                return Ok(new
                {
                    existe = true,
                    mensaje = "Ya existe una jugadora registrada con este DNI."
                });
            }

            return Ok(new
            {
                existe = false,
                mensaje = "DNI disponible."
            });
        }

        // 2. POST: api/jugadorasadmin (Alta de jugadora con matrícula automática)
        [HttpPost]
        public async Task<IActionResult> AltaJugadora(
     [FromBody] Jugadora nuevaJugadora,
     [FromQuery] string passwordInicial)
        {
            if (nuevaJugadora == null || string.IsNullOrWhiteSpace(passwordInicial))
                return BadRequest(new { mensaje = "Datos inválidos o contraseña faltante." });

            var email = nuevaJugadora.Email.Trim().ToLower();

            if (await _context.Jugadoras.AnyAsync(j => j.Dni == nuevaJugadora.Dni))
                return BadRequest(new { mensaje = "El DNI ya pertenece a una jugadora registrada." });

            if (await _context.Jugadoras.AnyAsync(j => j.Email.ToLower() == email) ||
                await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == email))
                return BadRequest(new { mensaje = "El correo electrónico ya está registrado." });

            nuevaJugadora.Email = email;
            nuevaJugadora.Activo = true;
            nuevaJugadora.Matricula = "TEMPORAL";

            _context.Jugadoras.Add(nuevaJugadora);
            await _context.SaveChangesAsync();

            nuevaJugadora.Matricula = $"CAT-{DateTime.Today.Year}-{nuevaJugadora.Id.ToString().PadLeft(3, '0')}";

            var usuario = new Usuario
            {
                NombreUsuario = $"{nuevaJugadora.Nombre} {nuevaJugadora.Apellido}",
                Email = email,
                Rol = "jugadora",
                IdJugadora = nuevaJugadora.Id,
                PasswordHash = BCryptNet.HashPassword(passwordInicial),
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Jugadora {nuevaJugadora.Apellido}, {nuevaJugadora.Nombre} creada con usuario de acceso." });
        }


        // 3. PUT: api/jugadorasadmin/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ModificarJugadora(int id, [FromBody] Jugadora datosNuevos)
        {
            if (datosNuevos == null)
                return BadRequest(new { mensaje = "Datos insuficientes." });

            try
            {
                var jugadoraBase = await _context.Jugadoras.FindAsync(id);

                if (jugadoraBase == null)
                    return NotFound(new { mensaje = "La jugadora especificada no existe." });

                var emailAnterior = jugadoraBase.Email?.Trim().ToLower();
                var emailNuevo = datosNuevos.Email?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(emailNuevo))
                    return BadRequest(new { mensaje = "El email es obligatorio." });

                var emailRepetidoJugadoras = await _context.Jugadoras
                    .AnyAsync(j => j.Email.ToLower() == emailNuevo && j.Id != id);

                var emailRepetidoUsuarios = await _context.Usuarios
                    .AnyAsync(u => u.Email.ToLower() == emailNuevo && u.Email.ToLower() != emailAnterior);

                if (emailRepetidoJugadoras || emailRepetidoUsuarios)
                    return BadRequest(new { mensaje = "Ese correo ya pertenece a otra persona registrada." });

                jugadoraBase.Nombre = datosNuevos.Nombre;
                jugadoraBase.Apellido = datosNuevos.Apellido;
                jugadoraBase.Email = emailNuevo;
                jugadoraBase.FechaNacimiento = datosNuevos.FechaNacimiento;
                jugadoraBase.FechaIngreso = datosNuevos.FechaIngreso;
                jugadoraBase.FechaBaja = datosNuevos.FechaBaja;
                jugadoraBase.Posicion = datosNuevos.Posicion;
                jugadoraBase.Equipo = datosNuevos.Equipo;
                jugadoraBase.Lateralidad = datosNuevos.Lateralidad;
                jugadoraBase.LugarNacimientoProvincia = datosNuevos.LugarNacimientoProvincia;
                jugadoraBase.Telefono = datosNuevos.Telefono;
                jugadoraBase.Activo = datosNuevos.Activo;

                if (!string.IsNullOrWhiteSpace(datosNuevos.FotoBase64))
                    jugadoraBase.FotoBase64 = datosNuevos.FotoBase64;

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == emailAnterior);

                if (usuario != null)
                {
                    usuario.NombreUsuario = $"{datosNuevos.Nombre} {datosNuevos.Apellido}";
                    usuario.Email = emailNuevo;
                    usuario.Rol = "jugadora";
                    usuario.IdJugadora = jugadoraBase.Id;
                    usuario.Activo = datosNuevos.Activo;
                }

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = $"Los datos de {jugadoraBase.Apellido}, {jugadoraBase.Nombre} fueron actualizados con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la información.", detalle = ex.Message });
            }
        }

        // PUT: api/JugadorasAdmin/{idJugadora}/resetear-clave
        [Authorize(Roles = "admin")]
        [HttpPut("resetear-clave/{idJugadora:int}")]
        public async Task<IActionResult> ResetearClaveJugadora(
     int idJugadora,
     [FromBody] ResetearClaveDTO solicitud)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var jugadora = await _context.Jugadoras
                .FirstOrDefaultAsync(j => j.Id == idJugadora);

            if (jugadora == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la jugadora."
                });
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdJugadora == idJugadora);

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensaje = "La jugadora no tiene una cuenta asociada."
                });
            }

            usuario.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    solicitud.NuevaPassword);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "La contraseña fue restablecida correctamente."
            });
        
    }


        // 4. DELETE (LÓGICO): api/jugadorasadmin/baja-logica/{id}
        // PUT: api/JugadorasAdmin/baja-logica/{id}
        [HttpPut("baja-logica/{id}")]
        public async Task<IActionResult> AplicarBajaLogica(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        mensaje = "El identificador de la jugadora no es válido."
                    });
                }

                var jugadora = await _context.Jugadoras
                    .FirstOrDefaultAsync(j => j.Id == id);

                if (jugadora == null)
                {
                    return NotFound(new
                    {
                        mensaje = "La jugadora no se encuentra."
                    });
                }

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.IdJugadora == jugadora.Id);

                jugadora.Activo = false;
                jugadora.FechaBaja ??= DateTime.Now;

                if (usuario != null)
                {
                    usuario.Activo = false;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje =
                        $"Baja lógica aplicada correctamente. " +
                        $"{jugadora.Apellido}, {jugadora.Nombre} quedó inactiva " +
                        "y su acceso fue suspendido.",

                    idJugadora = jugadora.Id,
                    jugadoraActiva = jugadora.Activo,
                    cuentaEncontrada = usuario != null,
                    cuentaActiva = usuario?.Activo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al procesar la baja de la jugadora.",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        
        // PUT: api/JugadorasAdmin/reincorporar/{id}
        [HttpPut("reincorporar/{id}")]
        public async Task<IActionResult> ReincorporarJugadora(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        mensaje = "El identificador de la jugadora no es válido."
                    });
                }

                var jugadora = await _context.Jugadoras
                    .FirstOrDefaultAsync(j => j.Id == id);

                if (jugadora == null)
                {
                    return NotFound(new
                    {
                        mensaje = "La jugadora no existe."
                    });
                }

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.IdJugadora == id);

                if (jugadora.Activo && (usuario == null || usuario.Activo))
                {
                    return Ok(new
                    {
                        mensaje = "La jugadora ya se encuentra activa."
                    });
                }

                var fechaBajaAnterior = jugadora.FechaBaja;
                var cuentaActivaAnterior = usuario?.Activo;

                jugadora.Activo = true;
                jugadora.FechaBaja = null;

                if (usuario != null)
                {
                    usuario.Activo = true;
                }

                _context.AuditoriasSistema.Add(new AuditoriaSistema
                {
                    UsuarioId = null, // Se reemplazará por el usuario autenticado con JWT
                    Modulo = "ADMINISTRACION",
                    Accion = "REINCORPORAR_JUGADORA",
                    Entidad = "Jugadora",
                    EntidadId = jugadora.Id,
                    ValorAnterior =
                        $"Activo=false; FechaBaja={fechaBajaAnterior:O}; " +
                        $"CuentaActiva={cuentaActivaAnterior}",
                    ValorNuevo =
                        $"Activo=true; FechaBaja=null; " +
                        $"CuentaActiva={usuario != null}",
                    FechaHora = DateTime.Now,
                    Observaciones =
                        "Reincorporación de jugadora conservando el legajo histórico."
                });

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje =
                        $"{jugadora.Apellido}, {jugadora.Nombre} fue reincorporada correctamente.",
                    idJugadora = jugadora.Id,
                    jugadoraActiva = jugadora.Activo,
                    cuentaEncontrada = usuario != null,
                    cuentaActiva = usuario?.Activo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "No fue posible reincorporar a la jugadora.",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
    }
}
