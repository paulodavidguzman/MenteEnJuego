using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface IGpsService
    {
        Task<bool> ProcesarArchivoGpsCsvAsync(IFormFile archivoCsv);
    }
}
