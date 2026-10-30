using System.ComponentModel.DataAnnotations;

namespace MenteEnJuego.Dominio.DTOs
{
    public class CrearCuentaDTO
    {
        [Required]
        [StringLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage =
                "La contraseña debe tener al menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } = string.Empty;

        public int? IdJugadora { get; set; }

        public int? IdMedico { get; set; }
    }
}