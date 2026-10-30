using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IIndicadorPlanificadoService
    {
        Task<List<IndicadorPlanificadoDTO>>
            ObtenerPorPlanificacionAsync(
                int planificacionTecnicaId);

        Task<IndicadorPlanificadoDTO?>
            ObtenerPorIdAsync(int id);

        Task<(
            bool Exito,
            string Mensaje,
            IndicadorPlanificadoDTO? Datos)>
            CrearAsync(
                CrearIndicadorPlanificadoDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje,
            IndicadorPlanificadoDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarIndicadorPlanificadoDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje)>
            EliminarAsync(int id);
    }
}