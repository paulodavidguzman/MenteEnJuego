using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenteEnJuego.Dominio.DTOs
{
    public class ResumenDeportivoDTO
    {
        public int TotalEvaluacionesWellness { get; set; }
        public int TotalLesiones { get; set; }
        public int TotalControlesNutricionales { get; set; }
        public int TotalEventosTimeline { get; set; }

        public DateTime? UltimaEvaluacionWellness { get; set; }
        public DateTime? UltimoControlNutricional { get; set; }
        public DateTime? UltimaLesion { get; set; }

        public int TotalDiasBajaMedica { get; set; }
        public string EstadoHistoricoGeneral { get; set; } = "Sin historial suficiente";
    }
}