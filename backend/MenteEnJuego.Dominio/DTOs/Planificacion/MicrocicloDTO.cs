using System;

namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class MicrocicloDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } =
            string.Empty;

        public int NumeroSemana { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public string? ObjetivoGeneral { get; set; }

        public string TipoMicrociclo { get; set; } =
            string.Empty;

        public string Division { get; set; } =
    string.Empty;

        public string Estado { get; set; } =
            string.Empty;

        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int CantidadSesiones { get; set; }
    }
}