using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("Notificaciones")]
    public class Notificacion
    {
        [Key]
        public int Id { get; set; }

        public int? UsuarioId { get; set; }

        public int? JugadoraId { get; set; }

        public int? SesionDeportivaId { get; set; }

        [Required]
        [StringLength(30)]
        public string Canal { get; set; } = null!;
        // Sistema, Email, WhatsApp

        [Required]
        [StringLength(50)]
        public string TipoNotificacion { get; set; } = null!;
        // Recordatorio_PRE, Recordatorio_POST, Formulario_Pendiente, etc.

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = null!;

        [Required]
        public string Mensaje { get; set; } = null!;

        public DateTime? FechaProgramada { get; set; }

        public DateTime? FechaEnvio { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Pendiente";
        // Pendiente, Enviada, Leida, Fallida, Cancelada

        public int Intentos { get; set; } = 0;

        [StringLength(500)]
        public string? ErrorEnvio { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        /* ==========================
           Relaciones
        ========================== */

        [ForeignKey(nameof(SesionDeportivaId))]
        public virtual SesionDeportiva? SesionDeportiva { get; set; }

        [ForeignKey(nameof(JugadoraId))]
        public virtual Jugadora? Jugadora { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }
    }
}