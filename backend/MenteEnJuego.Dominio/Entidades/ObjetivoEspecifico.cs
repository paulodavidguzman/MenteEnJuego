namespace MenteEnJuego.Dominio.Entidades
{
    public class ObjetivoEspecifico
    {
        public int Id { get; set; }

        public int PlanificacionTecnicaId { get; set; }

        public string Descripcion { get; set; } =
            string.Empty;

        public string Prioridad { get; set; } =
            "Media";

        public int Orden { get; set; }

        /*
         * Permanece nulo mientras la sesión
         * todavía no fue evaluada.
         */
        public bool? Cumplido { get; set; }

        public string? ObservacionResultado { get; set; }

        public PlanificacionTecnica PlanificacionTecnica
        {
            get;
            set;
        } = null!;
    }
}