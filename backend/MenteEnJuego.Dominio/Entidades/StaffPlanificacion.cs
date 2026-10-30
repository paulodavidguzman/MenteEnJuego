namespace MenteEnJuego.Dominio.Entidades
{
    public class StaffPlanificacion
    {
        public int Id { get; set; }

        public int PlanificacionTecnicaId { get; set; }

        public int StaffProfesionalId { get; set; }

        public string FuncionEnSesion { get; set; } =
            string.Empty;

        public bool EsResponsablePrincipal { get; set; }

        public string? Observaciones { get; set; }

        public PlanificacionTecnica
            PlanificacionTecnica
        {
            get;
            set;
        } = null!;

        public StaffProfesional
            StaffProfesional
        {
            get;
            set;
        } = null!;
    }
}