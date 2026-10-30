using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MenteEnJuego.API.Configuracion;
using MenteEnJuego.Dominio.Entidades;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MenteEnJuego.API.Seguridad
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;

        public TokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerarToken(
     Usuario usuario,
     StaffProfesional? profesional = null
 )
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey))
            {
                throw new InvalidOperationException(
                    "La clave JWT no está configurada."
                );
            }

            var claims = new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    usuario.Id.ToString()
                ),

                new(
                    ClaimTypes.NameIdentifier,
                    usuario.Id.ToString()
                ),

                new(
                    ClaimTypes.Name,
                    usuario.NombreUsuario ?? usuario.Email
                ),

                new(
                    ClaimTypes.Email,
                    usuario.Email
                ),

                new(
                    ClaimTypes.Role,
                    usuario.Rol
                ),

                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                )
            };

            if (usuario.IdJugadora.HasValue)
            {
                claims.Add(
                    new Claim(
                        "idJugadora",
                        usuario.IdJugadora.Value.ToString()
                    )
                );
            }

            var idProfesional =
    usuario.IdMedico ?? profesional?.Id;

            if (idProfesional.HasValue)
            {
                claims.Add(
                    new Claim(
                        "idProfesional",
                        idProfesional.Value.ToString()
                    )
                );
            }
            if (profesional != null)
            {
                claims.Add(
                    new Claim(
                        "especialidad",
                        profesional.Especialidad ?? "General"
                    )
                );

                claims.Add(
                    new Claim(
                        "sector",
                        profesional.Sector ?? "General"
                    )
                );
            }
            if (usuario.IdEntrenador.HasValue)
            {
                claims.Add(
                    new Claim(
                        "idEntrenador",
                        usuario.IdEntrenador.Value.ToString()
                    )
                );
            }

            var clave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)
            );

            var credenciales = new SigningCredentials(
                clave,
                SecurityAlgorithms.HmacSha256
            );

            var fechaActual = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: fechaActual,
                expires: fechaActual.AddMinutes(
                    _jwtOptions.ExpirationMinutes
                ),
                signingCredentials: credenciales
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}