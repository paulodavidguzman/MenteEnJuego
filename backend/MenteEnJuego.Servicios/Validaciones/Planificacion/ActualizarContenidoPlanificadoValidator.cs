using FluentValidation;
using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Validaciones.Planificacion
{
    public class ActualizarContenidoPlanificadoValidator
        : AbstractValidator<ActualizarContenidoPlanificadoDTO>
    {
        private static readonly string[] CategoriasValidas =
        {
          "Tecnico",
    "Tactico",
    "Fisico",
    "Preventivo",
    "Psicologico",
    "Regenerativo",
    "Arqueras"
        };

        private static readonly string[] PrioridadesValidas =
        {
            "Alta",
            "Media",
            "Baja"
        };

        public ActualizarContenidoPlanificadoValidator()
        {
            RuleFor(x => x.Categoria)
                .NotEmpty()
                .Must(c => CategoriasValidas.Contains(c))
                .WithMessage("La categoría indicada no es válida.");

            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Descripcion)
                .MaximumLength(1000)
.WithMessage(
    "La descripción no puede superar los 1000 caracteres.");

            RuleFor(x => x.Prioridad)
                .Must(p => PrioridadesValidas.Contains(p))
                .WithMessage("La prioridad indicada no es válida.");

            RuleFor(x => x.Orden)
                .GreaterThan(0);
        }
    }
}