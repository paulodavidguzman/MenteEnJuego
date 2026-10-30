using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("SesionesDeportivas")]
    public class SesionDeportiva
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string TipoSesion { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = null!;

        public DateTime Fecha { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan? HoraFin { get; set; }

        [Required]
        [StringLength(150)]
        public string Lugar { get; set; } = null!;

        [StringLength(300)]
        public string? Objetivo { get; set; }

        public string? Descripcion { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Programada";

        public int? CreadoPorUsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }

        [ForeignKey(nameof(CreadoPorUsuarioId))]
        public virtual Usuario? UsuarioCreador { get; set; }

        public int? MicrocicloId { get; set; }

        public Microciclo? Microciclo { get; set; }

        public PlanificacionTecnica? PlanificacionTecnica { get; set; }

        public ICollection<ConvocatoriaSesion> Convocatorias { get; set; } = new List<ConvocatoriaSesion>();

        public ICollection<FormularioSesion> Formularios { get; set; } = new List<FormularioSesion>();

        public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();

        public ICollection<PizarraTecnica> PizarrasTecnicas { get; set; } = new List<PizarraTecnica>();
    }
}