using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt; // Alias de la librería instalada
using System.Threading.Tasks;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. REGISTRO SEGURO: Encripta la contraseña antes de guardarla en SSMS
        public async Task<Usuario?> RegistrarUsuarioAsync(Usuario nuevoUsuario, string passwordPlana)
        {
            // Validar si el email ya está registrado en Talleres
            var existeEmail = await _context.Usuarios.AnyAsync(u => u.Email == nuevoUsuario.Email);
            if (existeEmail) return null;

            // Aplicar el Hash con sal de BCrypt (Vuelve la contraseña ilegible)
            nuevoUsuario.PasswordHash = BCryptNet.HashPassword(passwordPlana);

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return nuevoUsuario;
        }

        // 2. INICIO DE SESIÓN VERIFICADO: Compara la clave plana ingresada con el hash de la BD
        public async Task<Usuario?> AutenticarUsuarioAsync(LoginDTO credenciales)
        {
            var emailNormalizado = credenciales.Email.Trim().ToLower();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == emailNormalizado &&
                    u.Activo == true
                );

            if (usuario == null) return null;

            bool passwordCorrecta = BCryptNet.Verify(
                credenciales.Password,
                usuario.PasswordHash
            );
          
            if (!passwordCorrecta) return null;

            return usuario;
        
        
        }

    }

}
