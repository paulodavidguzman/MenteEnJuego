using FluentValidation;
using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Validaciones.Planificacion
{
    public class CambiarEstadoPlanificacionValidator
        : AbstractValidator<CambiarEstadoPlanificacionDTO>
    {
        public CambiarEstadoPlanificacionValidator()
        {
            RuleFor(x => x.NuevoEstado)
                .NotEmpty()
                .Must(EstadoValido)
                .WithMessage(
                    "Estado inválido.");
        }

        private bool EstadoValido(
            string estado)
        {
            return estado == "Borrador"
                || estado == "Pendiente"
                || estado == "Aprobada"
                || estado == "EnEjecucion"
                || estado == "Ejecutada"
                || estado == "Cancelada";
        }
    }
}