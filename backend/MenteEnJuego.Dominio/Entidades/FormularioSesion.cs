using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("FormulariosSesion")]
    public class FormularioSesion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SesionDeportivaId { get; set; }

        [Required]
        public int JugadoraId { get; set; }

        [Required]
        [StringLength(30)]
        public string TipoFormulario { get; set; } = null!;
        // Wellness_PRE, Wellness_POST, RPE_POST

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Pendiente";
        // Pendiente, Completado, Vencido, NoAplica

        public DateTime FechaHabilitacion { get; set; } = DateTime.Now;

        public DateTime? FechaRespuesta { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        /* ==========================
           Relaciones
        ========================== */

        [ForeignKey(nameof(SesionDeportivaId))]
        public virtual SesionDeportiva? SesionDeportiva { get; set; } = null!;

        [ForeignKey(nameof(JugadoraId))]
        public virtual Jugadora? Jugadora { get; set; }= null!;
    }
}