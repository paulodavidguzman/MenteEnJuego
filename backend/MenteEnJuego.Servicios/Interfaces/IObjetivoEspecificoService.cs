using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IObjetivoEspecificoService
    {
        Task<List<ObjetivoEspecificoDTO>>
            ObtenerPorPlanificacionAsync(
                int planificacionTecnicaId);

        Task<ObjetivoEspecificoDTO?>
            ObtenerPorIdAsync(int id);

        Task<(
            bool Exito,
            string Mensaje,
            ObjetivoEspecificoDTO? Datos)>
            CrearAsync(
                CrearObjetivoEspecificoDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje,
            ObjetivoEspecificoDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarObjetivoEspecificoDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje,
            ObjetivoEspecificoDTO? Datos)>
            EvaluarAsync(
                int id,
                EvaluarObjetivoEspecificoDTO solicitud);

        Task<(
            bool Exito,
            string Mensaje)>
            EliminarAsync(int id);
    }
}