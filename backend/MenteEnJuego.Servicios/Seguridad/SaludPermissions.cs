namespace MenteEnJuego.Servicios.Seguridad
{
    public class SaludPermissions
    {
        public bool PuedeVerFichaSalud { get; set; }

        public bool PuedeRegistrarLesion { get; set; }

        public bool PuedeActualizarReadaptacion { get; set; }

        public bool PuedeRegistrarControlNutricional { get; set; }

        public bool PuedeConsultarDolor { get; set; }

        public bool PuedeConsultarDisponibilidad { get; set; }
    }
}