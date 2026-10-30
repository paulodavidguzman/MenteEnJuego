namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class PlanificacionTecnicaDTO
    {
        public int Id { get; set; }

        public int SesionDeportivaId { get; set; }

        public int? UsuarioResponsableId { get; set; }

        public string? ObjetivoGeneral { get; set; }

        public int? DuracionPlanificadaMinutos { get; set; }

        public string? IntensidadPlanificada { get; set; }

        public byte? RpeObjetivo { get; set; }

        public int? CargaPlanificada { get; set; }

        public string? MaterialesNecesarios { get; set; }

        public string? Precauciones { get; set; }

        public string? Observaciones { get; set; }

        public string Estado { get; set; } =
            string.Empty;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int CantidadObjetivos { get; set; }

        public int CantidadContenidos { get; set; }

        public int CantidadBloques { get; set; }

        public int CantidadIndicadores { get; set; }

        public int CantidadProfesionales { get; set; }
    }
}