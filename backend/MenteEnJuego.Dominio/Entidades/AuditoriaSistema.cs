using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("AuditoriaSistema")]
    public class AuditoriaSistema
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("UsuarioId")]
        public int? UsuarioId { get; set; }

        [Required]
        [Column("Modulo")]
        [StringLength(80)]
        public string Modulo { get; set; } = null!;

        [Required]
        [Column("Accion")]
        [StringLength(80)]
        public string Accion { get; set; } = null!;

        [Column("Entidad")]
        [StringLength(80)]
        public string? Entidad { get; set; }

        [Column("EntidadId")]
        public int? EntidadId { get; set; }

        [Column("ValorAnterior")]
        public string? ValorAnterior { get; set; }

        [Column("ValorNuevo")]
        public string? ValorNuevo { get; set; }

        [Required]
        [Column("FechaHora", TypeName = "datetime")]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        [Column("Ip")]
        [StringLength(50)]
        public string? Ip { get; set; }

        [Column("Navegador")]
        [StringLength(300)]
        public string? Navegador { get; set; }

        [Column("Observaciones")]
        [StringLength(500)]
        public string? Observaciones { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }
    }
}