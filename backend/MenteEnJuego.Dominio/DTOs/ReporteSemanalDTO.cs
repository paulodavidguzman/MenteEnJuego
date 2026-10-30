namespace MenteEnJuego.Dominio.DTOs
{
    public class ReporteSemanalDTO
    {
        public int JugadoraId { get; set; }
        public string ApellidoYNombre { get; set; } = null!;

        // Cargas diarias en Unidades Arbitrarias (RPE * Minutos)
        public List<int> CargasDiariasLunesADomingo { get; set; } = new();

        public double CargaMediaSemanal { get; set; }  // Promedio (Celda % UA)
        public double DesviacionEstandar { get; set; } // Celda SD
        public double IndiceMonotonia { get; set; }    // Celda IM (Media / SD)
        public double IndiceFatiga { get; set; }       // Celda IF (Suma * IM)

        // Alertas automáticas según los umbrales del club
        public string AlertaMonotonia { get; set; } = null!; // Seguro, Alerta, Peligro
        public string AlertaFatiga { get; set; } = null!;
    }
}
