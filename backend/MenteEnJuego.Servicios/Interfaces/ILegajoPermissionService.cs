namespace MenteEnJuego.Servicios.Seguridad
{
    public interface ILegajoPermissionService
    {
        LegajoPermissions ObtenerPermisos(
            string rol,
            string? especialidad
        );
    }
}