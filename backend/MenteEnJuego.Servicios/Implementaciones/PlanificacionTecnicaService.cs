using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class PlanificacionTecnicaService
        : IPlanificacionTecnicaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        private readonly
            IValidator<CrearPlanificacionTecnicaDTO>
            _crearValidator;

        private readonly
            IValidator<ActualizarPlanificacionTecnicaDTO>
            _actualizarValidator;

        private readonly
            IValidator<CambiarEstadoPlanificacionDTO>
            _estadoValidator;

        public PlanificacionTecnicaService(
            ApplicationDbContext context,
            IMapper mapper,
            IValidator<CrearPlanificacionTecnicaDTO>
                crearValidator,
            IValidator<ActualizarPlanificacionTecnicaDTO>
                actualizarValidator,
            IValidator<CambiarEstadoPlanificacionDTO>
                estadoValidator)
        {
            _context = context;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
            _estadoValidator = estadoValidator;
        }

        // ==================================================
        // OBTENER TODAS
        // ==================================================

        public async Task<List<PlanificacionTecnicaDTO>>
            ObtenerTodasAsync()
        {
            var planificaciones =
                await ConsultaCompleta()
                    .AsNoTracking()
                    .OrderByDescending(
                        p => p.FechaCreacion)
                    .ToListAsync();

            return _mapper.Map<
                List<PlanificacionTecnicaDTO>>(
                    planificaciones);
        }

        // ==================================================
        // OBTENER POR ID
        // ==================================================

        public async Task<PlanificacionTecnicaDTO?>
            ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var planificacion =
                await ConsultaCompleta()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        p => p.Id == id);

            return planificacion == null
                ? null
                : _mapper.Map<
                    PlanificacionTecnicaDTO>(
                        planificacion);
        }

        // ==================================================
        // OBTENER POR SESIÓN DEPORTIVA
        // ==================================================

        public async Task<PlanificacionTecnicaDTO?>
            ObtenerPorSesionAsync(
                int sesionDeportivaId)
        {
            if (sesionDeportivaId <= 0)
            {
                return null;
            }

            var planificacion =
                await ConsultaCompleta()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        p =>
                            p.SesionDeportivaId ==
                            sesionDeportivaId);

            return planificacion == null
                ? null
                : _mapper.Map<
                    PlanificacionTecnicaDTO>(
                        planificacion);
        }

        // ==================================================
        // CREAR
        // ==================================================

        public async Task<(
            bool Exito,
            string Mensaje,
            PlanificacionTecnicaDTO? Datos)>
            CrearAsync(
                CrearPlanificacionTecnicaDTO solicitud,
                int? usuarioResponsableId)
        {
            var validacion =
                await _crearValidator.ValidateAsync(
                    solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var sesion =
                await _context.SesionesDeportivas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        s =>
                            s.Id ==
                            solicitud.SesionDeportivaId);

            if (sesion == null)
            {
                return (
                    false,
                    "No se encontró la sesión deportiva indicada.",
                    null);
            }

            if (sesion.Estado == "Cancelada" ||
                sesion.Estado == "Finalizada")
            {
                return (
                    false,
                    "No se puede crear una planificación para una sesión cancelada o finalizada.",
                    null);
            }

            var yaExiste =
                await _context.PlanificacionesTecnicas
                    .AsNoTracking()
                    .AnyAsync(
                        p =>
                            p.SesionDeportivaId ==
                            solicitud.SesionDeportivaId);

            if (yaExiste)
            {
                return (
                    false,
                    "La sesión deportiva ya posee una planificación técnica.",
                    null);
            }

            if (usuarioResponsableId.HasValue)
            {
                var usuarioResponsableValido =
                    await _context.Usuarios
                        .AsNoTracking()
                        .AnyAsync(u =>
                            u.Id == usuarioResponsableId.Value
                            &&
                            u.Activo
                            &&
                            (
                                u.Rol == "admin"
                                ||
                                u.Rol == "entrenador"
                            ));

                if (!usuarioResponsableValido)
                {
                    return (
                        false,
                        "El usuario responsable no existe, está inactivo o no posee un rol autorizado.",
                        null);
                }
            }

            var planificacion =
                _mapper.Map<PlanificacionTecnica>(
                    solicitud);

            planificacion.UsuarioResponsableId =
                usuarioResponsableId;

            planificacion.Estado = "Borrador";

            await _context.PlanificacionesTecnicas
                .AddAsync(planificacion);

            await _context.SaveChangesAsync();

            /*
             * CargaPlanificada es una columna calculada
             * por SQL Server. Recargamos la entidad para
             * obtener su valor definitivo.
             */
            await _context.Entry(planificacion)
                .ReloadAsync();

            var respuesta =
                _mapper.Map<PlanificacionTecnicaDTO>(
                    planificacion);

            return (
                true,
                "La planificación técnica fue creada correctamente.",
                respuesta);
        }

        // ==================================================
        // ACTUALIZAR
        // ==================================================

        public async Task<(
            bool Exito,
            string Mensaje,
            PlanificacionTecnicaDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarPlanificacionTecnicaDTO solicitud)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador de la planificación no es válido.",
                    null);
            }

            var validacion =
                await _actualizarValidator
                    .ValidateAsync(solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var planificacion =
                await ConsultaCompleta()
                    .FirstOrDefaultAsync(
                        p => p.Id == id);

            if (planificacion == null)
            {
                return (
                    false,
                    "No se encontró la planificación técnica solicitada.",
                    null);
            }

            if (planificacion.Estado != "Borrador" &&
                planificacion.Estado != "Pendiente")
            {
                return (
                    false,
                    "La planificación solamente puede modificarse cuando está en estado Borrador o Pendiente.",
                    null);
            }

            if (planificacion.SesionDeportiva.Estado ==
                    "Cancelada" ||
                planificacion.SesionDeportiva.Estado ==
                    "Finalizada")
            {
                return (
                    false,
                    "No se puede modificar la planificación porque la sesión está cancelada o finalizada.",
                    null);
            }

            _mapper.Map(
                solicitud,
                planificacion);

            await _context.SaveChangesAsync();

            await _context.Entry(planificacion)
                .ReloadAsync();

            /*
             * ReloadAsync recarga la entidad principal,
             * pero no sus colecciones. Para devolver los
             * contadores correctamente hacemos una consulta
             * completa posterior.
             */
            var actualizada =
                await ConsultaCompleta()
                    .AsNoTracking()
                    .FirstAsync(
                        p => p.Id == id);

            var respuesta =
                _mapper.Map<PlanificacionTecnicaDTO>(
                    actualizada);

            return (
                true,
                "La planificación técnica fue actualizada correctamente.",
                respuesta);
        }

        // ==================================================
        // CAMBIAR ESTADO
        // ==================================================

        public async Task<(
            bool Exito,
            string Mensaje,
            PlanificacionTecnicaDTO? Datos)>
            CambiarEstadoAsync(
                int id,
                CambiarEstadoPlanificacionDTO solicitud)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador de la planificación no es válido.",
                    null);
            }

            var validacion =
                await _estadoValidator
                    .ValidateAsync(solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var planificacion =
                await ConsultaCompleta()
                    .FirstOrDefaultAsync(
                        p => p.Id == id);

            if (planificacion == null)
            {
                return (
                    false,
                    "No se encontró la planificación técnica solicitada.",
                    null);
            }

            if (!TransicionPermitida(
                    planificacion.Estado,
                    solicitud.NuevoEstado))
            {
                return (
                    false,
                    $"No se permite cambiar el estado de " +
                    $"'{planificacion.Estado}' a " +
                    $"'{solicitud.NuevoEstado}'.",
                    null);
            }

            if (solicitud.NuevoEstado != "Cancelada" &&
                (planificacion.SesionDeportiva.Estado ==
                    "Cancelada" ||
                 planificacion.SesionDeportiva.Estado ==
                    "Finalizada"))
            {
                return (
                    false,
                    "No se puede avanzar con la planificación porque la sesión está cancelada o finalizada.",
                    null);
            }

            var validacionEstado =
                ValidarRequisitosDelNuevoEstado(
                    planificacion,
                    solicitud.NuevoEstado);

            if (!validacionEstado.Exito)
            {
                return (
                    false,
                    validacionEstado.Mensaje,
                    null);
            }

            planificacion.Estado =
                solicitud.NuevoEstado;

            planificacion.FechaModificacion =
                DateTime.Now;

            await _context.SaveChangesAsync();

            await _context.Entry(planificacion)
                .ReloadAsync();

            var actualizada =
                await ConsultaCompleta()
                    .AsNoTracking()
                    .FirstAsync(
                        p => p.Id == id);

            var respuesta =
                _mapper.Map<PlanificacionTecnicaDTO>(
                    actualizada);

            return (
                true,
                "El estado de la planificación técnica fue actualizado correctamente.",
                respuesta);
        }

        // ==================================================
        // CONSULTA COMPLETA
        // ==================================================

        private IQueryable<PlanificacionTecnica>
      ConsultaCompleta()
        {
            return _context.PlanificacionesTecnicas
                .AsSplitQuery()
                .Include(p => p.SesionDeportiva)
                .Include(p => p.UsuarioResponsable)
                .Include(p => p.ObjetivosEspecificos)
                .Include(p => p.ContenidosPlanificados)
                .Include(p => p.BloquesSesion)
                .Include(p => p.IndicadoresPlanificados)
                .Include(p => p.StaffPlanificaciones);
        }

        // ==================================================
        // TRANSICIONES DE ESTADO
        // ==================================================



        private static bool TransicionPermitida(
            string estadoActual,
            string nuevoEstado)
        {
            return estadoActual switch
            {
                "Borrador" =>
                    nuevoEstado == "Pendiente" ||
                    nuevoEstado == "Cancelada",

                "Pendiente" =>
                    nuevoEstado == "Borrador" ||
                    nuevoEstado == "Aprobada" ||
                    nuevoEstado == "Cancelada",

                "Aprobada" =>
                    nuevoEstado == "EnEjecucion" ||
                    nuevoEstado == "Cancelada",

                "EnEjecucion" =>
                    nuevoEstado == "Ejecutada" ||
                    nuevoEstado == "Cancelada",

                _ => false
            };
        }

        // ==================================================
        // REQUISITOS DE CADA ESTADO
        // ==================================================

        private static (
    bool Exito,
    string Mensaje)
    ValidarDatosGeneralesCompletos(
        PlanificacionTecnica planificacion,
        string nuevoEstado)
        {
            if (string.IsNullOrWhiteSpace(
                    planificacion.ObjetivoGeneral))
            {
                return (
                    false,
                    $"Para pasar a {nuevoEstado} debe informar el objetivo general.");
            }

            if (!planificacion
                    .DuracionPlanificadaMinutos
                    .HasValue)
            {
                return (
                    false,
                    $"Para pasar a {nuevoEstado} debe informar la duración planificada.");
            }

            if (string.IsNullOrWhiteSpace(
                    planificacion.IntensidadPlanificada))
            {
                return (
                    false,
                    $"Para pasar a {nuevoEstado} debe informar la intensidad planificada.");
            }

            if (!planificacion.RpeObjetivo.HasValue)
            {
                return (
                    false,
                    $"Para pasar a {nuevoEstado} debe informar el RPE objetivo.");
            }

            return (
                true,
                string.Empty);
        }

        private static (
    bool Exito,
    string Mensaje)
    ValidarRequisitosDelNuevoEstado(
        PlanificacionTecnica planificacion,
        string nuevoEstado)
        {
            /*
             * Tanto Pendiente como Aprobada requieren
             * que los datos generales estén completos.
             *
             * Esto impide aprobar una planificación a la
             * que se le borraron datos después de pasar
             * previamente a Pendiente.
             */
            if (nuevoEstado == "Pendiente" ||
                nuevoEstado == "Aprobada")
            {
                var validacionDatosGenerales =
                    ValidarDatosGeneralesCompletos(
                        planificacion,
                        nuevoEstado);

                if (!validacionDatosGenerales.Exito)
                {
                    return validacionDatosGenerales;
                }
            }

            if (nuevoEstado == "Aprobada")
            {
                if (planificacion
                        .ObjetivosEspecificos.Count == 0)
                {
                    return (
                        false,
                        "Para aprobar la planificación debe existir al menos un objetivo específico.");
                }

                if (planificacion.BloquesSesion.Count == 0)
                {
                    return (
                        false,
                        "Para aprobar la planificación debe existir al menos un bloque de sesión.");
                }

                if (!planificacion
                        .StaffPlanificaciones
                        .Any(s =>
                            s.EsResponsablePrincipal))
                {
                    return (
                        false,
                        "Para aprobar la planificación debe asignarse un responsable principal.");
                }

                var duracionBloques =
                    planificacion.BloquesSesion.Sum(
                        b =>
                            b.DuracionPlanificadaMinutos
                            +
                            b.PausaPlanificadaMinutos);

                /*
                 * En este punto la duración general ya fue
                 * validada como obligatoria, por lo que
                 * Value puede utilizarse de forma segura.
                 */
                if (duracionBloques >
                    planificacion
                        .DuracionPlanificadaMinutos!.Value)
                {
                    return (
                        false,
                        "La suma de las duraciones y pausas de los bloques supera la duración total planificada.");
                }
            }

            return (
                true,
                string.Empty);
        }

        // ==================================================
        // ERRORES DE VALIDACIÓN
        // ==================================================

        private static string UnirErrores(
            IEnumerable<
                FluentValidation.Results.ValidationFailure>
                errores)
        {
            return string.Join(
                " ",
                errores.Select(
                    error => error.ErrorMessage));
        }
    }
}