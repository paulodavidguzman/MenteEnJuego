using FluentValidation;
using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Validaciones.Planificacion
{
    public class ActualizarObjetivoEspecificoValidator
        : AbstractValidator<ActualizarObjetivoEspecificoDTO>
    {
        public ActualizarObjetivoEspecificoValidator()
        {
            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .WithMessage(
                    "La descripción del objetivo es obligatoria.")
                .MaximumLength(1000)
                .WithMessage(
                    "La descripción no puede superar los 1000 caracteres.");

            RuleFor(x => x.Prioridad)
                .NotEmpty()
                .Must(EsPrioridadValida)
                .WithMessage(
                    "La prioridad debe ser Alta, Media o Baja.");

            RuleFor(x => x.Orden)
                .GreaterThan(0)
                .WithMessage(
                    "El orden debe ser mayor que cero.");
        }

        private static bool EsPrioridadValida(
            string prioridad)
        {
            return prioridad == "Alta"
                || prioridad == "Media"
                || prioridad == "Baja";
        }
    }
}