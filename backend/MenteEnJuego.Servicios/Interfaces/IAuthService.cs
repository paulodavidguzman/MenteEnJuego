using MenteEnJuego.Dominio.DTOs;
using MenteEnJuego.Dominio.Entidades;
using System.Threading.Tasks;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IAuthService
    {
        Task<Usuario?> RegistrarUsuarioAsync(Usuario nuevoUsuario, string passwordPlana);
        Task<Usuario?> AutenticarUsuarioAsync(LoginDTO credenciales);
    }
}
