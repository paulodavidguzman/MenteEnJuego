using MenteEnJuego.Dominio.DTOs;
using System.Threading.Tasks;

namespace MenteEnJuego.Servicios.Interfaces
{
    public interface ICicloMenstrualService
    {
        FaseCicloMenstrualDTO EvaluarFaseFisiologica(int diaCiclo);
    }
}
