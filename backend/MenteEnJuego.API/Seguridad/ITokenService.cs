using MenteEnJuego.Dominio.Entidades;

namespace MenteEnJuego.API.Seguridad
{
    public interface ITokenService
    {
        string GenerarToken(
            Usuario usuario,
            StaffProfesional? profesional = null
        );
    }
}