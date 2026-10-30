namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class ActualizarPlanificacionTecnicaDTO
    {
        public string? ObjetivoGeneral { get; set; }

        public int? DuracionPlanificadaMinutos { get; set; }

        public string? IntensidadPlanificada { get; set; }

        public byte? RpeObjetivo { get; set; }

        public string? MaterialesNecesarios { get; set; }

        public string? Precauciones { get; set; }

        public string? Observaciones { get; set; }
    }
}