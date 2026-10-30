using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("evaluaciones_diarias")]
    public class EvaluacionDiaria
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_jugadora")]
        public int IdJugadora { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Required]
        [Column("tipo_sesion")]
        [StringLength(20)]
        public string TipoSesion { get; set; } = "Entrenamiento"; // Entrenamiento o Partido

        [Column("momento_evaluacion")]
        [StringLength(10)]
        public string? MomentoEvaluacion { get; set; }

        [Required]
        [Column("estado_asistencia")]
        [StringLength(30)]
        public string EstadoAsistencia { get; set; } = "Completó"; // Completó, Diferenciado, Ausente_Lesion, etc.

        [Column("estres")]
        public byte? Estres { get; set; }

        [Column("humor")]
        public byte? Humor { get; set; }

        [Column("sueño")]
        public byte? Sueño { get; set; }

        [Column("dolor_doms")]
        public byte? DolorDoms { get; set; }

        [Column("dia_ciclo_menstrual")]
        public int? DiaCicloMenstrual { get; set; }

        [Column("recuperacion_tqr")]
        public byte? RecuperacionTqr { get; set; }

        [Column("esfuerzo_rpe")]
        public byte? EsfuerzoRpe { get; set; }

        [Column("minutos_sesion")]
        public int? MinutosSesion { get; set; }
        [Column("sesion_deportiva_id")]
        public int? SesionDeportivaId { get; set; }

        // El backend lee este campo calculado generado automáticamente por la base de datos
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("unidades_arbitrarias")]
        public int? UnidadesArbitrarias { get; private set; }

        // Propiedades de navegación
        [ForeignKey("IdJugadora")]
        public virtual Jugadora? Jugadora { get; set; } = null!;

        // Relaciones uno a muchos y uno a uno
        public virtual ICollection<EvaluacionZonaDolor> ZonasDeDolor { get; set; } = new List<EvaluacionZonaDolor>();
        public virtual MetricasGpsAvanzadas? MetricasGps { get; set; }

        [ForeignKey(nameof(SesionDeportivaId))]
        public virtual SesionDeportiva? SesionDeportiva { get; set; }
    }
}
