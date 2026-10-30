namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class ActualizarStaffPlanificacionDTO
    {
        public string FuncionEnSesion { get; set; } = string.Empty;

        public bool EsResponsablePrincipal { get; set; }

        public string? Observaciones { get; set; }
    }
}