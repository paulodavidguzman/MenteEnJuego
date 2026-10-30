using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class MicrocicloService : IMicrocicloService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearMicrocicloDTO>
            _crearValidator;
        private readonly IValidator<ActualizarMicrocicloDTO>
            _actualizarValidator;
        private readonly IValidator<CambiarEstadoMicrocicloDTO>
            _estadoValidator;

        public MicrocicloService(
            ApplicationDbContext context,
            IMapper mapper,
            IValidator<CrearMicrocicloDTO> crearValidator,
            IValidator<ActualizarMicrocicloDTO> actualizarValidator,
            IValidator<CambiarEstadoMicrocicloDTO> estadoValidator)
        {
            _context = context;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
            _estadoValidator = estadoValidator;
        }

        public async Task<List<MicrocicloDTO>>
            ObtenerTodosAsync()
        {
            var microciclos = await _context.Microciclos
                .AsNoTracking()
                .Include(m => m.SesionesDeportivas)
                .OrderByDescending(m => m.FechaInicio)
                .ThenByDescending(m => m.NumeroSemana)
                .ToListAsync();

            return _mapper.Map<List<MicrocicloDTO>>(
                microciclos);
        }

        public async Task<MicrocicloDTO?>
            ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var microciclo = await _context.Microciclos
                .AsNoTracking()
                .Include(m => m.SesionesDeportivas)
                .FirstOrDefaultAsync(m => m.Id == id);

            return microciclo == null
                ? null
                : _mapper.Map<MicrocicloDTO>(microciclo);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            MicrocicloDTO? Datos)>
            CrearAsync(
                CrearMicrocicloDTO solicitud)
        {
            var validacion =
                await _crearValidator.ValidateAsync(solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var existeSolapamiento =
      await ExisteSolapamientoAsync(
          solicitud.FechaInicio,
          solicitud.FechaFin,
          solicitud.Division);

            if (existeSolapamiento)
            {
                return (
                    false,
                    "Las fechas se superponen con otro microciclo activo de la misma división.",
                    null);
            }

            var microciclo =
                _mapper.Map<Microciclo>(solicitud);

            await _context.Microciclos.AddAsync(
                microciclo);

            await _context.SaveChangesAsync();

            var respuesta =
                _mapper.Map<MicrocicloDTO>(microciclo);

            return (
                true,
                "El microciclo fue creado correctamente.",
                respuesta);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            MicrocicloDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarMicrocicloDTO solicitud)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador del microciclo no es válido.",
                    null);
            }

            var validacion =
                await _actualizarValidator.ValidateAsync(
                    solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var microciclo =
                await _context.Microciclos
                    .Include(m => m.SesionesDeportivas)
                    .FirstOrDefaultAsync(m => m.Id == id);

            if (microciclo == null)
            {
                return (
                    false,
                    "No se encontró el microciclo solicitado.",
                    null);
            }

            if (microciclo.Estado == "Finalizado" ||
                microciclo.Estado == "Cancelado")
            {
                return (
                    false,
                    "Un microciclo finalizado o cancelado no puede modificarse.",
                    null);
            }

            var existeSolapamiento =
     await ExisteSolapamientoAsync(
         solicitud.FechaInicio,
         solicitud.FechaFin,
         solicitud.Division,
         id);

            if (existeSolapamiento)
            {
                return (
                    false,
                    "Las fechas se superponen con otro microciclo activo.",
                    null);
            }

            _mapper.Map(solicitud, microciclo);

            await _context.SaveChangesAsync();

            var respuesta =
                _mapper.Map<MicrocicloDTO>(microciclo);

            return (
                true,
                "El microciclo fue actualizado correctamente.",
                respuesta);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            MicrocicloDTO? Datos)>
            CambiarEstadoAsync(
                int id,
                CambiarEstadoMicrocicloDTO solicitud)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador del microciclo no es válido.",
                    null);
            }

            var validacion =
                await _estadoValidator.ValidateAsync(solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var microciclo =
                await _context.Microciclos
                    .Include(m => m.SesionesDeportivas)
                    .FirstOrDefaultAsync(m => m.Id == id);

            if (microciclo == null)
            {
                return (
                    false,
                    "No se encontró el microciclo solicitado.",
                    null);
            }

            if (!TransicionPermitida(
                    microciclo.Estado,
                    solicitud.NuevoEstado))
            {
                return (
                    false,
                    $"No se permite cambiar el estado de " +
                    $"'{microciclo.Estado}' a " +
                    $"'{solicitud.NuevoEstado}'.",
                    null);
            }

            microciclo.Estado =
                solicitud.NuevoEstado;

            microciclo.FechaModificacion =
                DateTime.Now;

            await _context.SaveChangesAsync();

            var respuesta =
                _mapper.Map<MicrocicloDTO>(microciclo);

            return (
                true,
                "El estado del microciclo fue actualizado correctamente.",
                respuesta);
        }

        private async Task<bool>
     ExisteSolapamientoAsync(
         DateTime fechaInicio,
         DateTime fechaFin,
         string division,
         int? idExcluir = null)
        {
            var divisionNormalizada =
                division.Trim();

            var consulta =
                _context.Microciclos
                    .AsNoTracking()
                    .Where(m =>
                        m.Estado != "Cancelado" &&
                        m.Division == divisionNormalizada);

            if (idExcluir.HasValue)
            {
                consulta =
                    consulta.Where(
                        m => m.Id != idExcluir.Value);
            }

            return await consulta.AnyAsync(m =>
                fechaInicio <= m.FechaFin &&
                fechaFin >= m.FechaInicio);
        }

        private static bool TransicionPermitida(
            string estadoActual,
            string nuevoEstado)
        {
            return estadoActual switch
            {
                "Planificado" =>
                    nuevoEstado == "EnEjecucion" ||
                    nuevoEstado == "Cancelado",

                "EnEjecucion" =>
                    nuevoEstado == "Finalizado" ||
                    nuevoEstado == "Cancelado",

                _ => false
            };
        }

        private static string UnirErrores(
            IEnumerable<FluentValidation.Results.ValidationFailure>
                errores)
        {
            return string.Join(
                " ",
                errores.Select(e => e.ErrorMessage));
        }
    }
}