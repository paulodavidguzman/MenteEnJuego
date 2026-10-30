using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IBloqueSesionService
    {
        Task<List<BloqueSesionDTO>>
            ObtenerPorPlanificacionAsync(
                int planificacionTecnicaId);

        Task<BloqueSesionDTO?>
            ObtenerPorIdAsync(int id);

        Task<(
            bool Exito,
            string Mensaje,
            BloqueSesionDTO? Datos)>
            CrearAsync(
                CrearBloqueSesionDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje,
            BloqueSesionDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarBloqueSesionDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje)>
            EliminarAsync(int id);
    }
}