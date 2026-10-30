using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre_usuario")]
        [StringLength(100)]
        public string NombreUsuario { get; set; } = null!;

        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [Column("rol")]
        [StringLength(20)]
        public string Rol { get; set; } = null!; // jugadora, entrenador, medico, admin
        public bool Activo { get; set; } = true;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("id_jugadora")]
        public int? IdJugadora { get; set; }

        [Column("id_entrenador")]
        public int? IdEntrenador { get; set; }

        [Column("id_medico")]
        public int? IdMedico { get; set; }

        [ForeignKey("IdJugadora")]
        public virtual Jugadora? Jugadora { get; set; }
    }
}
