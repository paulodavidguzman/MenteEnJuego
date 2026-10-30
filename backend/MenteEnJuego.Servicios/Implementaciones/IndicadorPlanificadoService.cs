using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class IndicadorPlanificadoService
        : IIndicadorPlanificadoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        private readonly IValidator<CrearIndicadorPlanificadoDTO>
            _crearValidator;

        private readonly IValidator<ActualizarIndicadorPlanificadoDTO>
            _actualizarValidator;

        public IndicadorPlanificadoService(
            ApplicationDbContext context,
            IMapper mapper,
            IValidator<CrearIndicadorPlanificadoDTO> crearValidator,
            IValidator<ActualizarIndicadorPlanificadoDTO> actualizarValidator)
        {
            _context = context;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
        }

        public async Task<List<IndicadorPlanificadoDTO>>
    ObtenerPorPlanificacionAsync(
        int planificacionTecnicaId)
        {
            if (planificacionTecnicaId <= 0)
            {
                return new List<IndicadorPlanificadoDTO>();
            }

            var indicadores =
                await _context.IndicadoresPlanificados
                    .AsNoTracking()
                    .Where(x =>
                        x.PlanificacionTecnicaId ==
                        planificacionTecnicaId)
                    .OrderBy(x => x.Orden)
                    .ThenBy(x => x.Nombre)
                    .ToListAsync();

            return _mapper.Map<
                List<IndicadorPlanificadoDTO>>(
                    indicadores);
        }

        public async Task<IndicadorPlanificadoDTO?>
            ObtenerPorIdAsync(int id)
        {
            var indicador =
                await _context.IndicadoresPlanificados
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (indicador == null)
            {
                return null;
            }

            return _mapper.Map<IndicadorPlanificadoDTO>(
                indicador);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            IndicadorPlanificadoDTO? Datos)>
            CrearAsync(
                CrearIndicadorPlanificadoDTO solicitud)
        {
            NormalizarSolicitudCreacion(solicitud);

            var resultadoValidacion =
                await _crearValidator.ValidateAsync(solicitud);

            if (!resultadoValidacion.IsValid)
            {
                return (
                    false,
                    ObtenerErroresValidacion(
                        resultadoValidacion.Errors),
                    null);
            }

            NormalizarSolicitudCreacion(solicitud);

            var planificacion =
                await _context.PlanificacionesTecnicas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id ==
                        solicitud.PlanificacionTecnicaId);

            if (planificacion == null)
            {
                return (
                    false,
                    "No se encontró la planificación técnica indicada.",
                    null);
            }

            if (!EsPlanificacionEditable(
                    planificacion.Estado))
            {
                return (
                    false,
                    "La planificación técnica no puede modificarse porque su estado no es Borrador ni Pendiente.",
                    null);
            }

            var ordenDuplicado =
                await _context.IndicadoresPlanificados
                    .AnyAsync(x =>
                        x.PlanificacionTecnicaId ==
                            solicitud.PlanificacionTecnicaId
                        &&
                        x.Orden == solicitud.Orden);

            if (ordenDuplicado)
            {
                return (
                    false,
                    $"Ya existe un indicador con el orden {solicitud.Orden} dentro de la planificación.",
                    null);
            }

            var indicadorDuplicado =
                await _context.IndicadoresPlanificados
                    .AnyAsync(x =>
                        x.PlanificacionTecnicaId ==
                            solicitud.PlanificacionTecnicaId
                        &&
                        x.Categoria ==
                            solicitud.Categoria
                        &&
                        x.Nombre ==
                            solicitud.Nombre);

            if (indicadorDuplicado)
            {
                return (
                    false,
                    "Ya existe un indicador con el mismo nombre y categoría dentro de la planificación.",
                    null);
            }

            var indicador =
                _mapper.Map<IndicadorPlanificado>(
                    solicitud);

            _context.IndicadoresPlanificados.Add(
                indicador);

            await _context.SaveChangesAsync();

            var indicadorDTO =
                _mapper.Map<IndicadorPlanificadoDTO>(
                    indicador);

            return (
                true,
                "El indicador planificado se creó correctamente.",
                indicadorDTO);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            IndicadorPlanificadoDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarIndicadorPlanificadoDTO solicitud)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador del indicador no es válido.",
                    null);
            }

            NormalizarSolicitudActualizacion(
            solicitud);

            var resultadoValidacion =
                await _actualizarValidator
                    .ValidateAsync(solicitud);

            if (!resultadoValidacion.IsValid)
            {
                return (
                    false,
                    ObtenerErroresValidacion(
                        resultadoValidacion.Errors),
                    null);
            }

            NormalizarSolicitudActualizacion(solicitud);

            var indicador =
                await _context.IndicadoresPlanificados
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (indicador == null)
            {
                return (
                    false,
                    "No se encontró el indicador planificado solicitado.",
                    null);
            }

            var planificacion =
                await _context.PlanificacionesTecnicas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id ==
                        indicador.PlanificacionTecnicaId);

            if (planificacion == null)
            {
                return (
                    false,
                    "No se encontró la planificación técnica asociada al indicador.",
                    null);
            }

            if (!EsPlanificacionEditable(
                    planificacion.Estado))
            {
                return (
                    false,
                    "La planificación técnica no puede modificarse porque su estado no es Borrador ni Pendiente.",
                    null);
            }

            var ordenDuplicado =
                await _context.IndicadoresPlanificados
                    .AnyAsync(x =>
                        x.Id != id
                        &&
                        x.PlanificacionTecnicaId ==
                            indicador.PlanificacionTecnicaId
                        &&
                        x.Orden == solicitud.Orden);

            if (ordenDuplicado)
            {
                return (
                    false,
                    $"Ya existe otro indicador con el orden {solicitud.Orden} dentro de la planificación.",
                    null);
            }

            var indicadorDuplicado =
                await _context.IndicadoresPlanificados
                    .AnyAsync(x =>
                        x.Id != id
                        &&
                        x.PlanificacionTecnicaId ==
                            indicador.PlanificacionTecnicaId
                        &&
                        x.Categoria ==
                            solicitud.Categoria
                        &&
                        x.Nombre ==
                            solicitud.Nombre);

            if (indicadorDuplicado)
            {
                return (
                    false,
                    "Ya existe otro indicador con el mismo nombre y categoría dentro de la planificación.",
                    null);
            }

            _mapper.Map(solicitud, indicador);

            await _context.SaveChangesAsync();

            var indicadorDTO =
                _mapper.Map<IndicadorPlanificadoDTO>(
                    indicador);

            return (
                true,
                "El indicador planificado se actualizó correctamente.",
                indicadorDTO);
        }

        public async Task<(
            bool Exito,
            string Mensaje)>
            EliminarAsync(int id)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador del indicador no es válido.");
            }

            var indicador =
                await _context.IndicadoresPlanificados
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (indicador == null)
            {
                return (
                    false,
                    "No se encontró el indicador planificado solicitado.");
            }

            var planificacion =
                await _context.PlanificacionesTecnicas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id ==
                        indicador.PlanificacionTecnicaId);

            if (planificacion == null)
            {
                return (
                    false,
                    "No se encontró la planificación técnica asociada al indicador.");
            }

            if (!EsPlanificacionEditable(
                    planificacion.Estado))
            {
                return (
                    false,
                    "El indicador no puede eliminarse porque la planificación no está en estado Borrador ni Pendiente.");
            }

            _context.IndicadoresPlanificados.Remove(
                indicador);

            await _context.SaveChangesAsync();

            return (
                true,
                "El indicador planificado se eliminó correctamente.");
        }

        private static bool EsPlanificacionEditable(
            string? estado)
        {
            return string.Equals(
                       estado,
                       "Borrador",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   string.Equals(
                       estado,
                       "Pendiente",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static void NormalizarSolicitudCreacion(
            CrearIndicadorPlanificadoDTO solicitud)
        {
            solicitud.Nombre =
                solicitud.Nombre.Trim();

            solicitud.Categoria =
                solicitud.Categoria.Trim();

            solicitud.FuenteDatos =
                solicitud.FuenteDatos.Trim();

            solicitud.UnidadMedida =
                NormalizarTextoOpcional(
                    solicitud.UnidadMedida);
        }

        private static void NormalizarSolicitudActualizacion(
            ActualizarIndicadorPlanificadoDTO solicitud)
        {
            solicitud.Nombre =
                solicitud.Nombre.Trim();

            solicitud.Categoria =
                solicitud.Categoria.Trim();

            solicitud.FuenteDatos =
                solicitud.FuenteDatos.Trim();

            solicitud.UnidadMedida =
                NormalizarTextoOpcional(
                    solicitud.UnidadMedida);
        }

        private static string? NormalizarTextoOpcional(
            string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        private static string ObtenerErroresValidacion(
            IEnumerable<FluentValidation.Results.ValidationFailure>
                errores)
        {
            return string.Join(
                " ",
                errores.Select(x => x.ErrorMessage));
        }
    }
}