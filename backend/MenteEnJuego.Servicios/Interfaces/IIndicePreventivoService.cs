using MenteEnJuego.Dominio.DTOs;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IIndicePreventivoService
    {
        Task<IndicePreventivoResultadoDTO> CalcularIndicePreventivoAsync(
            int idJugadora,
            DateTime fecha);
    }
}