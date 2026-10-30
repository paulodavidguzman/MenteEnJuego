using FluentValidation;
using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Validaciones.Planificacion
{
    public class ActualizarBloqueSesionValidator
        : AbstractValidator<ActualizarBloqueSesionDTO>
    {
        private static readonly string[] TiposBloqueValidos =
        {
            "Activacion",
            "EntradaEnCalor",
            "Tecnico",
            "Tactico",
            "Fisico",
            "PartePrincipal",
            "JuegoAplicado",
            "VueltaALaCalma",
            "Recuperacion",
            "Evaluacion"
        };

        private static readonly string[] IntensidadesValidas =
        {
            "MuyBaja",
            "Baja",
            "Moderada",
            "Alta",
            "MuyAlta"
        };

        public ActualizarBloqueSesionValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .WithMessage(
                    "El nombre del bloque es obligatorio.")
                .MaximumLength(150)
                .WithMessage(
                    "El nombre del bloque no puede superar los 150 caracteres.");

            RuleFor(x => x.TipoBloque)
                .NotEmpty()
                .WithMessage(
                    "El tipo de bloque es obligatorio.")
                .Must(tipo =>
                    TiposBloqueValidos.Contains(tipo))
                .WithMessage(
                    "El tipo de bloque indicado no es válido.");

            RuleFor(x => x.Orden)
                .GreaterThan(0)
                .WithMessage(
                    "El orden debe ser mayor que cero.");

            RuleFor(x => x.DuracionPlanificadaMinutos)
                .GreaterThan(0)
                .WithMessage(
                    "La duración planificada debe ser mayor que cero.");

            RuleFor(x => x.IntensidadPlanificada)
                .Must(intensidad =>
                    string.IsNullOrWhiteSpace(intensidad)
                    || IntensidadesValidas.Contains(intensidad))
                .WithMessage(
                    "La intensidad planificada indicada no es válida.");

            RuleFor(x => x.Objetivo)
     .MaximumLength(1000)
     .WithMessage(
         "El objetivo no puede superar los 1000 caracteres.")
     .When(x =>
         !string.IsNullOrWhiteSpace(
             x.Objetivo));

            RuleFor(x => x.Descripcion)
    .MaximumLength(2000)
    .WithMessage(
        "La descripción no puede superar los 2000 caracteres.")
    .When(x =>
        !string.IsNullOrWhiteSpace(
            x.Descripcion));

            RuleFor(x => x.PausaPlanificadaMinutos)
                .GreaterThanOrEqualTo(0)
                .WithMessage(
                    "La pausa planificada no puede ser negativa.");
        }
    }
}