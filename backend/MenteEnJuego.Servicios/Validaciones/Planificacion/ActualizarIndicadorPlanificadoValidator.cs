using FluentValidation;
using MenteEnJuego.Dominio.DTOs.Planificacion;

namespace MenteEnJuego.Servicios.Validaciones.Planificacion
{
    public class ActualizarIndicadorPlanificadoValidator
        : AbstractValidator<ActualizarIndicadorPlanificadoDTO>
    {
        public ActualizarIndicadorPlanificadoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Categoria)
                .NotEmpty()
                .Must(EsCategoriaValida)
                .WithMessage("La categoría no es válida.");

            RuleFor(x => x.UnidadMedida)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.UnidadMedida));

            RuleFor(x => x.FuenteDatos)
                .NotEmpty()
                .Must(EsFuenteValida)
                .WithMessage("La fuente de datos no es válida.");

            RuleFor(x => x.Orden)
                .GreaterThan(0);

            RuleFor(x => x)
                .Must(RangoValido)
                .WithMessage("El valor mínimo no puede ser mayor al valor máximo.");

            RuleFor(x => x.ValorObjetivo)
        .PrecisionScale(
            12,
            3,
            false)
        .When(x =>
            x.ValorObjetivo.HasValue)
        .WithMessage(
            "El valor objetivo admite hasta 9 dígitos enteros y 3 decimales.");

            RuleFor(x => x.ValorMinimo)
                .PrecisionScale(
                    12,
                    3,
                    false)
                .When(x =>
                    x.ValorMinimo.HasValue)
                .WithMessage(
                    "El valor mínimo admite hasta 9 dígitos enteros y 3 decimales.");

            RuleFor(x => x.ValorMaximo)
                .PrecisionScale(
                    12,
                    3,
                    false)
                .When(x =>
                    x.ValorMaximo.HasValue)
                .WithMessage(
                    "El valor máximo admite hasta 9 dígitos enteros y 3 decimales.");

        }

        private static bool EsCategoriaValida(string categoria)
        {
            string[] categorias =
            {
                "GPS",
                "Wellness",
                "Tecnico",
                "Tactico",
                "Fisico",
                "Medico",
                "Asistencia",
                "CargaInterna"
            };

            return categorias.Contains(categoria);
        }

        private static bool EsFuenteValida(string fuente)
        {
            string[] fuentes =
            {
                "GPS",
                "FormularioPre",
                "FormularioPost",
                "RegistroManual",
                "EvaluacionMedica",
                "AnalisisTecnico"
            };

            return fuentes.Contains(fuente);
        }

        private static bool RangoValido(ActualizarIndicadorPlanificadoDTO dto)
        {
            if (!dto.ValorMinimo.HasValue || !dto.ValorMaximo.HasValue)
                return true;

            return dto.ValorMinimo <= dto.ValorMaximo;
        }
    }
}