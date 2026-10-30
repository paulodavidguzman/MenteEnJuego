namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class StaffPlanificacionDTO
    {
        public int Id { get; set; }

        public int PlanificacionTecnicaId { get; set; }

        public int StaffProfesionalId { get; set; }

        public string FuncionEnSesion { get; set; } = string.Empty;

        public bool EsResponsablePrincipal { get; set; }

        public string? Observaciones { get; set; }
    }
}