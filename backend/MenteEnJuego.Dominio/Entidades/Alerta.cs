using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("Alertas")]
    public class Alerta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JugadoraId { get; set; }

        public int? SesionDeportivaId { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoAlerta { get; set; } = null!;
        // Riesgo_Lesion, Fatiga_Alta, Wellness_Bajo, etc.

        [Required]
        [StringLength(20)]
        public string Nivel { get; set; } = "Medio";
        // Bajo, Medio, Alto, Critico

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = null!;

        [Required]
        public string Mensaje { get; set; } = null!;

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Activa";
        // Activa, EnRevision, Resuelta, Descartada

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaResolucion { get; set; }

        public int? ResueltaPorUsuarioId { get; set; }

        [StringLength(500)]
        public string? ObservacionResolucion { get; set; }

        /* ==========================
           Relaciones
        ========================== */

        [ForeignKey(nameof(SesionDeportivaId))]
        public virtual SesionDeportiva? SesionDeportiva { get; set; }

        [ForeignKey(nameof(JugadoraId))]
        public virtual Jugadora? Jugadora { get; set; }

        [ForeignKey(nameof(ResueltaPorUsuarioId))]
        public virtual Usuario? UsuarioResolucion { get; set; }
    }
}