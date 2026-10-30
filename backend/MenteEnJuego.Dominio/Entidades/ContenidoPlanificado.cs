namespace MenteEnJuego.Dominio.Entidades
{
    public class ContenidoPlanificado
    {
        public int Id { get; set; }

        public int PlanificacionTecnicaId { get; set; }

        public string Categoria { get; set; } =
            string.Empty;

        public string Nombre { get; set; } =
            string.Empty;

        public string? Descripcion { get; set; }

        public string Prioridad { get; set; } =
            "Media";

        public int Orden { get; set; }

        public PlanificacionTecnica
            PlanificacionTecnica
        {
            get;
            set;
        } = null!;
    }
}