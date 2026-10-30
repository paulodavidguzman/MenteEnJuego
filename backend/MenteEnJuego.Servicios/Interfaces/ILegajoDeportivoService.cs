using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenteEnJuego.Dominio.DTOs;
using MenteEnJuego.Servicios.Seguridad;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface ILegajoDeportivoService
    {
        Task<LegajoDeportivoDTO?> ObtenerLegajoAsync(
    int idJugadora,
    LegajoPermissions permisos);
    }
}
