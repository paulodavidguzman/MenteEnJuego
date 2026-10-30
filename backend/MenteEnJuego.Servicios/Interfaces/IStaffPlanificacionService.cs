using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IStaffPlanificacionService
    {
        Task<List<StaffPlanificacionDTO>>
            ObtenerPorPlanificacionAsync(
                int planificacionTecnicaId);

        Task<StaffPlanificacionDTO?>
            ObtenerPorIdAsync(int id);

        Task<(
            bool Exito,
            string Mensaje,
            StaffPlanificacionDTO? Datos)>
            CrearAsync(
                CrearStaffPlanificacionDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje,
            StaffPlanificacionDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarStaffPlanificacionDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje)>
            EliminarAsync(int id);
    }
}