namespace MenteEnJuego.Servicios.Seguridad
{
    public class LegajoPermissions
    {
        // Datos generales
        public bool PuedeVerDatosPersonales { get; set; }
        public bool PuedeVerDatosContacto { get; set; }
        public bool PuedeVerMedidasCorporalesActuales { get; set; }

        // Timeline
        public bool PuedeVerTimeline { get; set; }
        public bool PuedeVerTimelineWellness { get; set; }
        public bool PuedeVerTimelinePsicologia { get; set; }
        public bool PuedeVerTimelineNutricion { get; set; }
        public bool PuedeVerTimelineLesiones { get; set; }

        // Información consolidada
        public bool PuedeVerResumen { get; set; }
        public bool PuedeVerMatrizHistorica { get; set; }
        public bool PuedeVerIndicadoresWellness { get; set; }
        public bool PuedeVerIndicadoresLesiones { get; set; }
        public bool PuedeVerDetalleNutricional { get; set; }
        public bool PuedeVerRiesgo { get; set; }
        public bool PuedeVerDisponibilidad { get; set; }
        public bool PuedeVerEvolucionRpe { get; set; }

        // Edición
        public bool PuedeEditarPeso { get; set; }
        public bool PuedeEditarPsicologia { get; set; }
        public bool PuedeEditarLesiones { get; set; }
        public bool PuedeEditarReadaptacion { get; set; }
    }
}