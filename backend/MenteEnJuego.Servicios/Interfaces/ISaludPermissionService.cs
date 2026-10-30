using MenteEnJuego.Servicios.Seguridad;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface ISaludPermissionService
    {
        SaludPermissions ObtenerPermisos(
            string rol,
            string? especialidad);
    }
}