using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IContenidoPlanificadoService
    {
        Task<List<ContenidoPlanificadoDTO>>
            ObtenerPorPlanificacionAsync(
                int planificacionTecnicaId);

        Task<ContenidoPlanificadoDTO?>
            ObtenerPorIdAsync(int id);

        Task<(
            bool Exito,
            string Mensaje,
            ContenidoPlanificadoDTO? Datos)>
            CrearAsync(
                CrearContenidoPlanificadoDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje,
            ContenidoPlanificadoDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarContenidoPlanificadoDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje)>
            EliminarAsync(int id);
    }
}