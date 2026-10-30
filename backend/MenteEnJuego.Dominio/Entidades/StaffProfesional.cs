using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("staff_profesional")]
    public class StaffProfesional
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

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
        [Column("especialidad")]
        [StringLength(100)]
        public string Especialidad { get; set; } = null!;

        [Required]
        [Column("sector")]
        [StringLength(30)]
        public string Sector { get; set; } = null!;

        [Column("telefono")]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [Column("email")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Column("foto_base64")]
        public string? FotoBase64 { get; set; }

        public ICollection<StaffPlanificacion>
    StaffPlanificaciones
        { get; set; } =
    new List<StaffPlanificacion>();
    }
}