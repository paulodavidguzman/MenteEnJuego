using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("evaluaciones_zonas_dolor")]
    public class EvaluacionZonaDolor
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_evaluacion")]
        public int IdEvaluacion { get; set; }

        [Required]
        [Column("zona_muscular")]
        [StringLength(50)]
        public string ZonaMuscular { get; set; } = null!; // Psoas, Cuádriceps, Isquiosurales, etc.

        [Column("observaciones_otro")]
        [StringLength(255)]
        public string? ObservacionesOtro { get; set; }

        [Column("intensidad_dolor")]
        public int? IntensidadDolor { get; set; }

        [ForeignKey(nameof(IdEvaluacion))]
        public virtual EvaluacionDiaria? EvaluacionDiaria { get; set; } = null!;
    }
}
