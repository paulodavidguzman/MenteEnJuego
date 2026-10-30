using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MenteEnJuego.Dominio.Entidades
{
    public class Microciclo
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
        [Required]
        [StringLength(100)]
        public string Division { get; set; } =
            string.Empty;

        public string Estado { get; set; } =
            "Planificado";

        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; } =
            DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        public ICollection<SesionDeportiva>
            SesionesDeportivas
        { get; set; } =
            new List<SesionDeportiva>();
    }
}