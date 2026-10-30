namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class BloqueSesionDTO
    {
        public int Id { get; set; }

        public int PlanificacionTecnicaId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string TipoBloque { get; set; } = string.Empty;

        public int Orden { get; set; }

        public int DuracionPlanificadaMinutos { get; set; }

        public string? IntensidadPlanificada { get; set; }

        public string? Objetivo { get; set; }

        public string? Descripcion { get; set; }

        public int PausaPlanificadaMinutos { get; set; }
    }
}