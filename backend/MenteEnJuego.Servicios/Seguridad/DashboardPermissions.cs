using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenteEnJuego.Servicios.Seguridad
{
    public class DashboardPermissions
    {
        // Dashboards
        public bool PuedeVerDashboardTecnico { get; set; }

        public bool PuedeVerCentroOperaciones { get; set; }

        public bool PuedeVerSesiones { get; set; }

        // GPS

        public bool PuedeVerComparativaGps { get; set; }

        public bool PuedeVerHeatmapGps { get; set; }

        // Wellness

        public bool PuedeVerWellness { get; set; }

        public bool PuedeVerRiesgo { get; set; }

        public bool PuedeVerDisponibilidad { get; set; }

        public bool PuedeVerDolor { get; set; }

        // Individual

        public bool PuedeConsultarJugadoras { get; set; }

        public bool PuedeConsultarGraficosIndividuales { get; set; }

        public bool PuedeConsultarDolorIndividual { get; set; }
    }
}
