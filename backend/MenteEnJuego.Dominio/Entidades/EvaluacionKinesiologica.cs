
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("evaluaciones_kinesiologicas")]
    public class EvaluacionKinesiologica
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdJugadora { get; set; }

        public int? IdProfesional { get; set; }

        [Required]
        public DateTime FechaEvaluacion { get; set; }

        [Required]
        [MaxLength(100)]
        public string RegionCorporal { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Lateralidad { get; set; }

        [Range(0, 10)]
        public int? Dolor { get; set; }

        [MaxLength(500)]
        public string? Movilidad { get; set; }

        [MaxLength(100)]
        public string? LimitacionFuncional { get; set; }

        [MaxLength(100)]
        public string? FaseRehabilitacion { get; set; }

        public string? TratamientoRealizado { get; set; }

        public string? Objetivos { get; set; }

        public DateTime? ProximoControl { get; set; }

        public string? Observaciones { get; set; }

        public Jugadora Jugadora { get; set; } = null!;

        public StaffProfesional? Profesional { get; set; }
    }
}
