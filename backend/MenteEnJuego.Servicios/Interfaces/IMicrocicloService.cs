using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IMicrocicloService
    {
        Task<List<MicrocicloDTO>>
            ObtenerTodosAsync();

        Task<MicrocicloDTO?>
            ObtenerPorIdAsync(int id);

        Task<(
            bool Exito,
            string Mensaje,
            MicrocicloDTO? Datos)>
            CrearAsync(
                CrearMicrocicloDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje,
            MicrocicloDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarMicrocicloDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje,
            MicrocicloDTO? Datos)>
            CambiarEstadoAsync(
                int id,
                CambiarEstadoMicrocicloDTO solicitud);
    }
}