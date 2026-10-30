using MenteEnJuego.Dominio.DTOs;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IRendimientoService
    {
        Task<List<ReporteSemanalDTO>> ObtenerReporteAcumuladoSemanalAsync(DateTime fechaLunes);
    }
}
