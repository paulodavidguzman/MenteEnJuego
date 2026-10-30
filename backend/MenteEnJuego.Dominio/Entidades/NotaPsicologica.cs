using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("notas_psicologicas")]
    public class NotaPsicologica
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_jugadora")]
        public int IdJugadora { get; set; }

        [Required]
        [Column("id_medico")]
        public int IdMedico { get; set; } // Psicóloga

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Required]
        [Column("sesion_confidencial_notas")]
        public string SesionConfidencialNotas { get; set; } = null!; // Contenido confidencial de la intervención psicológica

        [ForeignKey(nameof(IdJugadora))]
        public virtual Jugadora Jugadora { get; set; } = null!;

        [ForeignKey(nameof(IdMedico))]
        public virtual StaffProfesional StaffProfesional { get; set; } = null!;
    }
}
