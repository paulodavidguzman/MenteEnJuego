namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class ActualizarBloqueSesionDTO
    {
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