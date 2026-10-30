using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenteEnJuego.Dominio.DTOs
{
    public class LegajoDeportivoDTO
    {
        public FichaGeneralJugadoraDTO FichaGeneral { get; set; } = new();
        public RiesgoDeportivoDTO RiesgoActual { get; set; } = new();
        public List<EventoTimelineDTO> Timeline { get; set; } = new();

        public string DisponibilidadActual { get; set; } = "Disponible";
        public string UltimoEstadoBienestar { get; set; } = "Sin datos";
        public string UltimoEstadoCarga { get; set; } = "Sin datos";
        public ResumenDeportivoDTO ResumenDeportivo { get; set; } = new();
        public MatrizHistoricaLegajoDTO MatrizHistorica { get; set; } = new();
        public List<MiniGraficoRpe> EvolucionRpe { get; set; } = new();
    }
}
