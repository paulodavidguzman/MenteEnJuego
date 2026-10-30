using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("ConvocatoriasSesion")]
    public class ConvocatoriaSesion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SesionDeportivaId { get; set; }

        [Required]
        public int JugadoraId { get; set; }

        [Required]
        [StringLength(30)]
        public string EstadoConvocatoria { get; set; } = "Convocada";

        [StringLength(30)]
        public string? EstadoAsistencia { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        /* ==========================
           Relaciones
        ========================== */

        [ForeignKey(nameof(SesionDeportivaId))]
        public virtual SesionDeportiva? SesionDeportiva { get; set; } = null!;

        [ForeignKey(nameof(JugadoraId))]
        public virtual Jugadora? Jugadora { get; set; } = null!;
    }
}