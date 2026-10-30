using FluentValidation;
using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Validaciones.Planificacion
{
    public class ActualizarMicrocicloValidator
        : AbstractValidator<ActualizarMicrocicloDTO>
    {
        public ActualizarMicrocicloValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.NumeroSemana)
                .GreaterThan(0);

            RuleFor(x => x.FechaFin)
                .GreaterThanOrEqualTo(x => x.FechaInicio);

            RuleFor(x => x.TipoMicrociclo)
                .NotEmpty()
                .Must(EsTipoValido);

            RuleFor(x => x.ObjetivoGeneral)
                .MaximumLength(1000);

            RuleFor(x => x.Observaciones)
                .MaximumLength(2000);

            RuleFor(x => x.Division)
    .NotEmpty()
    .WithMessage(
        "La división es obligatoria.")
    .MaximumLength(100);
        }

        private bool EsTipoValido(string tipo)
        {
            return tipo == "Carga"
                || tipo == "Competitivo"
                || tipo == "Precompetitivo"
                || tipo == "Recuperacion"
                || tipo == "Regenerativo"
                || tipo == "Evaluacion"
                || tipo == "Descarga";
        }
    }
}