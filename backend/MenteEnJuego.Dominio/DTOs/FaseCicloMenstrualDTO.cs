namespace MenteEnJuego.Dominio.DTOs
{
    public class FaseCicloMenstrualDTO
    {
        public int DiaCiclo { get; set; }
        public string FaseNombre { get; set; } = null!;        // Fase Folicular, Ovulación, Lútea, etc.
        public string ColorSemaforo { get; set; } = null!;     // Rojo, Verde, Amarillo, Naranja, Azul
        public string ImpactoBiologico { get; set; } = null!;  // Ej: "+FATIGA / -Oxigenación"
        public string RecomendacionPlanificacion { get; set; } = null!; // Ej: "Desarrollo de Fuerza"
        public string AlertaMedicaPreventiva { get; set; } = null!;     // Alertas críticas como laxitud en LCA
    }
}
