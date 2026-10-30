using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.Entidades;
using System;
using System.Threading.Tasks;

namespace MenteEnJuego.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GestionClubController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GestionClubController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. POST: api/gestionclub/alta-jugadora (El Administrador crea una futbolista)
        [HttpPost("alta-jugadora")]
        public async Task<IActionResult> RegistrarJugadora([FromBody] Jugadora nuevaJugadora)
        {
            if (nuevaJugadora == null) return BadRequest("Datos inválidos.");

            try
            {
                // Evitar DNIs duplicados en Talleres
                var existeDni = await _context.Jugadoras.AnyAsync(j => j.Dni == nuevaJugadora.Dni);
                if (existeDni) return BadRequest("El DNI ingresado ya pertenece a una jugadora del club.");

                _context.Jugadoras.Add(nuevaJugadora);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = $"¡Éxito! La jugadora {nuevaJugadora.Apellido}, {nuevaJugadora.Nombre} fue dada de alta con ID {nuevaJugadora.Id}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error en el alta.", detalle = ex.Message });
            }
        }

        // 2. PUT: api/gestionclub/modificar-jugadora/{id} (El Administrador actualiza datos)
        [HttpPut("modificar-jugadora/{id}")]
        public async Task<IActionResult> ActualizarJugadora(int id, [FromBody] Jugadora datosActualizados)
        {
            if (datosActualizados == null) return BadRequest("Datos insuficientes.");

            var jugadoraBase = await _context.Jugadoras.FindAsync(id);
            if (jugadoraBase == null) return NotFound("La jugadora no existe en los registros.");

            // Modificaciones permitidas para el Administrador
            jugadoraBase.FechaNacimiento = datosActualizados.FechaNacimiento;
            jugadoraBase.FechaIngreso = datosActualizados.FechaIngreso;
            jugadoraBase.FechaBaja = datosActualizados.FechaBaja;
            jugadoraBase.Posicion = datosActualizados.Posicion;
            jugadoraBase.Equipo = datosActualizados.Equipo;
            jugadoraBase.Lateralidad = datosActualizados.Lateralidad;
            jugadoraBase.Telefono = datosActualizados.Telefono;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Los datos de la futbolista fueron actualizados con éxito." });
        }

        // 3. POST: api/gestionclub/precargar-datos-prueba (Seed Data instantáneo para tus pruebas)
        [HttpPost("precargar-datos-prueba")]
        public async Task<IActionResult> CargarSeedData()
        {
            // Validar si ya hay datos inyectados para no duplicar
            if (await _context.Jugadoras.AnyAsync())
                return BadRequest(new { mensaje = "La base de datos ya cuenta con registros activos." });

            // Inyectar un domicilio real en Córdoba
            var casaNuevaCordoba = new Domicilio
            {
                Calle = "Chacabuco",
                Numero = "450",
                Barrio = "Nueva Córdoba",
                Localidad = "Córdoba Capital"
            };
            _context.Domicilios.Add(casaNuevaCordoba);
            await _context.SaveChangesAsync();

            // Dar de alta a las jugadoras reales que simulan las celdas de tu Excel
            var j1 = new Jugadora
            {
                Nombre = "Sofía",
                Apellido = "Capdevila",
                Dni = "44111222",
                Matricula = "CAT-001",
                // Reemplazá j.Edad por este cálculo automático en vivo:
                //Edad = DateTime.Today.Year - j1.FechaNacimiento.Year - (DateTime.Today.DayOfYear < j.FechaNacimiento.DayOfYear ? 1 : 0),

                Posicion = "Volante",
                Equipo = "Primera",
                Lateralidad = "diestra",
                LugarNacimientoProvincia = "Córdoba",
                IdDomicilio = casaNuevaCordoba.Id
            };

            var j2 = new Jugadora
            {
                Nombre = "Martina",
                Apellido = "Racca",
                Dni = "45333444",
                Matricula = "CAT-002"
                // Reemplazá j.Edad por este cálculo automático en vivo:
               // Edad = DateTime.Today.Year - j2.FechaNacimiento.Year - (DateTime.Today.DayOfYear < j.FechaNacimiento.DayOfYear ? 1 : 0),
,
                Posicion = "Delantera",
                Equipo = "Primera",
                Lateralidad = "zurda",
                LugarNacimientoProvincia = "Santa Fe",
                IdDomicilio = casaNuevaCordoba.Id
            };

            _context.Jugadoras.AddRange(j1, j2);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Base de datos inicializada de forma profesional. Sofía Capdevila (ID 1) y Martina Racca (ID 2) listas en SQL Server." });
        }
    }
}
