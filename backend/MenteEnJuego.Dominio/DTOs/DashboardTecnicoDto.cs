using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenteEnJuego.Dominio.DTOs
{
    public class DashboardTecnicoDto
    {
        public int TotalJugadoras { get; set; }

        public int SesionesProgramadas { get; set; }

        public int FormulariosPendientes { get; set; }

        public int AlertasActivas { get; set; }

        public decimal RecuperacionPromedio { get; set; }

        public decimal FatigaPromedio { get; set; }

        public decimal RiesgoLesion { get; set; }

        public decimal DisponibilidadPlantel { get; set; }

        public List<SerieDto> EvolucionCarga { get; set; }
            = new();

        public List<SerieDto> EvolucionWellness { get; set; }
            = new();

        public List<ZonaDolorDto> MapaDolor { get; set; }
            = new();
    }


    public class SerieDto
    {
        public string Fecha { get; set; } = "";

        public decimal Valor { get; set; }
    }


    public class ZonaDolorDto
    {
        public string Zona { get; set; } = "";

        public decimal Intensidad { get; set; }
    }
}
