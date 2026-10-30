using FluentValidation;
using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Validaciones.Planificacion
{
    public class CrearStaffPlanificacionValidator
        : AbstractValidator<CrearStaffPlanificacionDTO>
    {
        public CrearStaffPlanificacionValidator()
        {
            RuleFor(x => x.PlanificacionTecnicaId)
                .GreaterThan(0)
                .WithMessage(
                    "La planificación técnica es obligatoria.");

            RuleFor(x => x.StaffProfesionalId)
                .GreaterThan(0)
                .WithMessage(
                    "El profesional del staff es obligatorio.");

            RuleFor(x => x.FuncionEnSesion)
                .NotEmpty()
                .WithMessage(
                    "La función del profesional en la sesión es obligatoria.")
                .MaximumLength(100)
                .WithMessage(
                    "La función en la sesión no puede superar los 100 caracteres.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(1000)
                .WithMessage(
                    "Las observaciones no pueden superar los 1000 caracteres.")
                .When(x =>
                    !string.IsNullOrWhiteSpace(
                        x.Observaciones));
        }
    }
}