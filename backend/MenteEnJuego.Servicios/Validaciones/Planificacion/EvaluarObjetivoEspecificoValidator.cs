using FluentValidation;
using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Validaciones.Planificacion
{
    public class EvaluarObjetivoEspecificoValidator
        : AbstractValidator<EvaluarObjetivoEspecificoDTO>
    {
        public EvaluarObjetivoEspecificoValidator()
        {
            RuleFor(x => x.ObservacionResultado)
                .MaximumLength(1500)
                .WithMessage(
                    "La observación del resultado no puede superar los 1500 caracteres.");
        }
    }
}