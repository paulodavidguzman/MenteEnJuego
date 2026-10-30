using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("PizarraTecnica")]
    public class PizarraTecnica
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SesionDeportivaId { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = null!;

        [Required]
        [StringLength(40)]
        public string TipoPizarra { get; set; } = null!;

        [Required]
        public string ContenidoJson { get; set; } = null!;

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public int? CreadoPorUsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Activa";

        [ForeignKey(nameof(SesionDeportivaId))]
        public virtual SesionDeportiva SesionDeportiva { get; set; } = null!;

        [ForeignKey(nameof(CreadoPorUsuarioId))]
        public virtual Usuario? UsuarioCreador { get; set; }
    }
}
