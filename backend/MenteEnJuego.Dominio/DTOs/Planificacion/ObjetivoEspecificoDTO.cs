namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class ObjetivoEspecificoDTO
    {
        public int Id { get; set; }

        public int PlanificacionTecnicaId { get; set; }

        public string Descripcion { get; set; } =
            string.Empty;

        public string Prioridad { get; set; } =
            string.Empty;

        public int Orden { get; set; }

        public bool? Cumplido { get; set; }

        public string? ObservacionResultado { get; set; }
    }
}