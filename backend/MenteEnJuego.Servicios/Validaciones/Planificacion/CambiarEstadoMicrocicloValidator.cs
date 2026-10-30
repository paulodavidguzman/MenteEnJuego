using FluentValidation;
using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Validaciones.Planificacion
{
    public class CambiarEstadoMicrocicloValidator
        : AbstractValidator<CambiarEstadoMicrocicloDTO>
    {
        public CambiarEstadoMicrocicloValidator()
        {
            RuleFor(x => x.NuevoEstado)
                .NotEmpty()
                .Must(EstadoValido)
                .WithMessage(
                    "Estado inválido.");
        }

        private bool EstadoValido(string estado)
        {
            return estado == "Planificado"
                || estado == "EnEjecucion"
                || estado == "Finalizado"
                || estado == "Cancelado";
        }
    }
}