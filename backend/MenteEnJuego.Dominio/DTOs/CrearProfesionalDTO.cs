using MenteEnJuego.Dominio.Entidades;
using System.ComponentModel.DataAnnotations;

namespace MenteEnJuego.Dominio.DTOs
{
    public class CrearProfesionalDTO
    {
        [Required]
        public StaffProfesional Profesional { get; set; } = null!;

        [Required]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage =
                "La contraseña debe tener al menos 8 caracteres.")]
        public string PasswordInicial { get; set; } =
            string.Empty;
    }
}