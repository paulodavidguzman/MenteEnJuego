using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("evaluaciones_nutricionales")]
    public class EvaluacionNutricional
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_jugadora")]
        public int IdJugadora { get; set; }

        [Required]
        [Column("id_medico")]
        public int IdMedico { get; set; } // Nutricionista

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Required]
        [Column("peso_kg")]
        public decimal PesoKg { get; set; }

        [Column("porcentaje_grasa")]
        public decimal? PorcentajeGrasa { get; set; }

        [Column("porcentaje_musculo")]
        public decimal? PorcentajeMusculo { get; set; }

        [Column("notas_nutricion")]
        [StringLength(500)]
        public string? NotasNutricion { get; set; }

        [ForeignKey(nameof(IdJugadora))]
        public virtual Jugadora Jugadora { get; set; } = null!;

        [ForeignKey(nameof(IdMedico))]
        public virtual StaffProfesional StaffProfesional { get; set; } = null!;
    }
}
