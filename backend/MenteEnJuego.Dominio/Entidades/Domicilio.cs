using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("domicilios")]
    public class Domicilio
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("calle")]
        [StringLength(100)]
        public string Calle { get; set; } = null!;

        [Column("numero")]
        [StringLength(10)]
        public string? Numero { get; set; }

        [Column("barrio")]
        [StringLength(100)]
        public string? Barrio { get; set; }

        [Required]
        [Column("localidad")]
        [StringLength(100)]
        public string Localidad { get; set; } = null!;

        [Column("provincia")]
        [StringLength(100)]
        public string Provincia { get; set; } = "Córdoba";

        [Column("codigo_postal")]
        [StringLength(10)]
        public string? CodigoPostal { get; set; }
    }
}
