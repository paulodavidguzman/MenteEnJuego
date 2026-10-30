using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MenteEnJuego.Dominio.DTOs;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.API.Seguridad;
using MenteEnJuego.Datos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;

        public AuthController(
            IAuthService authService,
            ITokenService tokenService,
            ApplicationDbContext context)
        {
            _authService = authService;
            _tokenService = tokenService;
            _context = context;
        }

        // POST: api/Auth/crear-cuenta
        [Authorize(Roles = "admin")]
        [HttpPost("crear-cuenta")]
        public async Task<IActionResult> Registrar(
            [FromBody] CrearCuentaDTO solicitud)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var rolNormalizado =
                solicitud.Rol.Trim().ToLowerInvariant();

            var rolesPermitidos = new[]
            {
        "admin",
        "jugadora",
        "entrenador",
        "medico"
    };

            if (!rolesPermitidos.Contains(rolNormalizado))
            {
                return BadRequest(new
                {
                    mensaje =
                        "El rol indicado no es válido."
                });
            }

            var nuevoUsuario = new Usuario
            {
                NombreUsuario =
                    solicitud.NombreUsuario.Trim(),

                Email =
                    solicitud.Email.Trim().ToLowerInvariant(),

                Rol = rolNormalizado,

                IdJugadora =
                    solicitud.IdJugadora,

                IdMedico =
                    solicitud.IdMedico,

                Activo = true
            };

            var resultado =
                await _authService.RegistrarUsuarioAsync(
                    nuevoUsuario,
                    solicitud.Password);

            if (resultado == null)
            {
                return Conflict(new
                {
                    mensaje =
                        "El correo electrónico ya se encuentra registrado."
                });
            }

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    mensaje =
                        "Cuenta de usuario creada correctamente.",

                    idUsuario =
                        resultado.Id
                });
        }



        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDTO credenciales)
        {
            if (credenciales == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe ingresar las credenciales."
                });
            }

            var usuarioValidado =
                await _authService.AutenticarUsuarioAsync(
                    credenciales
                );

            if (usuarioValidado == null)
            {
                return Unauthorized(new
                {
                    mensaje =
                        "Credenciales incorrectas o cuenta inactiva. " +
                        "Verifique su correo y contraseña."
                });
            }

            var emailNormalizado =
                usuarioValidado.Email.Trim().ToLower();

            var profesional =
                await _context.StaffProfesionales
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p =>
                        p.Email != null &&
                        p.Email.ToLower() == emailNormalizado
                    );

            var token = _tokenService.GenerarToken(
                usuarioValidado,
                profesional
            );

            return Ok(new
            {
                mensaje = "¡Acceso concedido!",

                token,
                tipoToken = "Bearer",

                usuarioId = usuarioValidado.Id,
                nombreUsuario = usuarioValidado.NombreUsuario,
                email = usuarioValidado.Email,
                rol = usuarioValidado.Rol,
                idJugadora = usuarioValidado.IdJugadora,

                idProfesional =
                    usuarioValidado.IdMedico ??
                    profesional?.Id,

                especialidad =
                    profesional?.Especialidad ??
                    "General",

                sector =
                    profesional?.Sector ??
                    "General"
            });
        }

        // GET: api/auth/verificar-token
        [Authorize]
        [HttpGet("verificar-token")]
        public IActionResult VerificarToken()
        {
            var idUsuario =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var email =
                User.FindFirstValue(ClaimTypes.Email);

            var rol =
                User.FindFirstValue(ClaimTypes.Role);

            var idJugadora =
                User.FindFirstValue("idJugadora");

            var idProfesional =
                User.FindFirstValue("idProfesional");

            var especialidad =
                User.FindFirstValue("especialidad");

            var sector =
                User.FindFirstValue("sector");

            return Ok(new
            {
                autenticado =
                    User.Identity?.IsAuthenticated ?? false,

                idUsuario,
                email,
                rol,
                idJugadora,
                idProfesional,
                especialidad,
                sector
            });
        }



    }
}
