using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("historial_lesiones")]
    public class HistorialLesion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_jugadora")]
        public int IdJugadora { get; set; }

        [Required]
        [Column("id_medico")]
        public int IdMedico { get; set; } // El médico o kinesiólogo que atiende

        [Required]
        [Column("tipo_lesion")]
        [StringLength(100)]
        public string TipoLesion { get; set; } = null!; // Ej: 'Desgarro Isquiotibiales'

        [Column("gravedad")]
        [StringLength(20)]
        public string? Gravedad { get; set; } // Leve, Moderada, Grave

        [Column("descripcion")]
        public string? Descripcion { get; set; } // Evolución clínica confidencial

        [Required]
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Column("fecha_alta_estimada")]
        public DateTime? FechaAltaEstimada { get; set; }

        [Column("fecha_alta_real")]
        public DateTime? FechaAltaReal { get; set; }

        [Required]
        [Column("estado_readaptacion")]
        public int EstadoReadaptacion { get; set; } = 100; // 0% (Baja total) a 100% (Alta / Apta)

        [ForeignKey(nameof(IdJugadora))]
        public virtual Jugadora Jugadora { get; set; } = null!;

        [ForeignKey(nameof(IdMedico))]
        public virtual StaffProfesional StaffProfesional { get; set; } = null!;
    }
}
