using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("jugadoras")]
    public class Jugadora
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("matricula")]
        [StringLength(20)]
        public string? Matricula { get; set; }

        [Required]
        [Column("dni")]
        [StringLength(15)]
        public string Dni { get; set; } = null!;

        [Required]
        [Column("apellido")]
        [StringLength(100)]
        public string Apellido { get; set; } = null!;

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [Column("fecha_nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Column("altura_cm")]
        public decimal? AlturaCm { get; set; }

        [Required]
        [Column("fecha_ingreso")]
        public DateTime FechaIngreso { get; set; } = DateTime.Today;

        [Column("fecha_baja")]
        public DateTime? FechaBaja { get; set; } // Admite NULL porque al dar el alta no está de baja


        [Required]
        [Column("nacionalidad")]
        [StringLength(50)]
        public string Nacionalidad { get; set; } = "Argentina";

        [Required]
        [Column("lugar_nacimiento_provincia")]
        [StringLength(100)]
        public string LugarNacimientoProvincia { get; set; } = null!;

        [Required]
        [Column("lugar_nacimiento_pais")]
        [StringLength(100)]
        public string LugarNacimientoPais { get; set; } = "Argentina";

        [Column("posicion")]
        [StringLength(50)]
        public string? Posicion { get; set; }

        [Column("equipo")]
        [StringLength(100)]
        public string? Equipo { get; set; }

        [Required]
        [Column("lateralidad")]
        [StringLength(20)]
        public string Lateralidad { get; set; } = "diestra"; // "diestra" o "zurda"

        [Required]
        [Column("fecha_alta")]
        public DateTime FechaAlta { get; set; } = DateTime.Now; // Log de auditoría de sistema

        [Column("telefono")]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [Column("activo")]
        public bool Activo { get; set; } = true; // true = Activa, false = Baja Lógica (Inactiva)


        [Column("id_domicilio")]
        public int? IdDomicilio { get; set; }

        [Column("foto_base64")] // Debe coincidir con la minúscula exacta de SQL Server
        public string? FotoBase64 { get; set; }


        // Propiedad de navegación (Relación con su casa actual en Córdoba)
        [ForeignKey(nameof(IdDomicilio))]
        public virtual Domicilio? DomicilioActual { get; set; }
    }
}
