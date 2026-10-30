namespace MenteEnJuego.Dominio.DTOs
{
    public class ImportacionGpsDTO
    {
        // Campos de cruce obligatorio
        public int IdEvaluacion { get; set; }

        // CATEGORÍA 1: Desplazamiento y Velocidad
        public decimal DistanciaTotalKm { get; set; }
        public decimal VelocidadMaximaKmh { get; set; }
        public decimal DistanciaZonaAltaVelocidadMetros { get; set; } // HSR

        // CATEGORÍA 2: Aceleración y Desaceleración
        public int CantidadAceleracionesIntensas { get; set; }
        public int CantidadDesaceleracionesIntensas { get; set; }
        public decimal PlayerLoadIndice { get; set; } // Carga biomecánica

        // CATEGORÍA 3: Datos Fisiológicos
        public int FcPromedioLpm { get; set; }
        public int FcMaximaLpm { get; set; }
    }
}
