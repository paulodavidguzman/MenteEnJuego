using System.ComponentModel.DataAnnotations;

namespace MenteEnJuego.Dominio.DTOs
{
    public class ResetearClaveDTO
    {
        [Required]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage =
                "La nueva contraseña debe tener al menos 8 caracteres.")]
        public string NuevaPassword { get; set; } = string.Empty;
    }
}