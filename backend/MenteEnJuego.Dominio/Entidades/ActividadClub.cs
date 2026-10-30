using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("actividades_club")]
    public class ActividadClub
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("tipo_actividad")]
        public string TipoActividad { get; set; } = null!; // "Entrenamiento" o "Partido"

        [Required]
        [Column("fecha_hora_inicio")]
        public DateTime FechaHoraInicio { get; set; }

        [Required]
        [Column("fecha_hora_fin")]
        public DateTime FechaHoraFin { get; set; }
    }
}
