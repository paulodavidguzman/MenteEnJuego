using FluentValidation;
using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Validaciones.Planificacion
{
    public class CrearPlanificacionTecnicaValidator
        : AbstractValidator<CrearPlanificacionTecnicaDTO>
    {
        public CrearPlanificacionTecnicaValidator()
        {
            RuleFor(x => x.SesionDeportivaId)
                .GreaterThan(0);

            RuleFor(x => x.ObjetivoGeneral)
                .MaximumLength(1000);

            RuleFor(x => x.DuracionPlanificadaMinutos)
                .GreaterThan(0)
                .When(x => x.DuracionPlanificadaMinutos.HasValue);

            RuleFor(x => x.IntensidadPlanificada)
                .Must(IntensidadValida)
                .When(x =>
                    !string.IsNullOrWhiteSpace(
                        x.IntensidadPlanificada));

            RuleFor(x => x.RpeObjetivo)
                .InclusiveBetween((byte)1, (byte)10)
                .When(x => x.RpeObjetivo.HasValue);

            RuleFor(x => x.MaterialesNecesarios)
                .MaximumLength(1500);

            RuleFor(x => x.Precauciones)
                .MaximumLength(1500);

            RuleFor(x => x.Observaciones)
                .MaximumLength(2000);
        }

        private bool IntensidadValida(
            string? intensidad)
        {
            return intensidad == "MuyBaja"
                || intensidad == "Baja"
                || intensidad == "Moderada"
                || intensidad == "Alta"
                || intensidad == "MuyAlta";
        }
    }
}