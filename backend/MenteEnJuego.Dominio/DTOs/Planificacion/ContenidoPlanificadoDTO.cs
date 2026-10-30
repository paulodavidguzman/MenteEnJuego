namespace MenteEnJuego.Dominio.DTOs.Planificacion { 
    public class ContenidoPlanificadoDTO
    {
        public int Id { get; set; }

        public int PlanificacionTecnicaId { get; set; }

        public string Categoria { get; set; } =
            string.Empty;

        public string Nombre { get; set; } =
            string.Empty;

        public string? Descripcion { get; set; }

        public string Prioridad { get; set; } =
            string.Empty;

        public int Orden { get; set; }
    }
}