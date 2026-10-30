using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MenteEnJuego.Servicios.Seguridad;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IDashboardPermissionService
    {
        DashboardPermissions ObtenerPermisos(
            string rol,
            string? especialidad);
    }
}
