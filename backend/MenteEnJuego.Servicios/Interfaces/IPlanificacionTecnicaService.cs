using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IPlanificacionTecnicaService
    {
        Task<List<PlanificacionTecnicaDTO>>
            ObtenerTodasAsync();

        Task<PlanificacionTecnicaDTO?>
            ObtenerPorIdAsync(int id);

        Task<PlanificacionTecnicaDTO?>
            ObtenerPorSesionAsync(
                int sesionDeportivaId);

        Task<(
            bool Exito,
            string Mensaje,
            PlanificacionTecnicaDTO? Datos)>
            CrearAsync(
                CrearPlanificacionTecnicaDTO solicitud,
                int? usuarioResponsableId);

        Task<(
            bool Exito,
            string Mensaje,
            PlanificacionTecnicaDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarPlanificacionTecnicaDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje,
            PlanificacionTecnicaDTO? Datos)>
            CambiarEstadoAsync(
                int id,
                CambiarEstadoPlanificacionDTO solicitud);
    }
}