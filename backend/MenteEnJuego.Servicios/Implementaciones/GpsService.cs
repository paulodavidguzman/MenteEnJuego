using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Interfaces;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class GpsService : IGpsService
    {
        private readonly ApplicationDbContext _context;

        public GpsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProcesarArchivoGpsCsvAsync(IFormFile archivoCsv)
        {
            if (archivoCsv == null || archivoCsv.Length == 0) return false;

            // Abrir el archivo de red en un flujo de lectura de memoria
            using (var reader = new StreamReader(archivoCsv.OpenReadStream()))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, // El archivo tiene títulos arriba
                Delimiter = ","        // Separado por comas estándar
            }))
            {
                // Leer los registros del CSV de forma asíncrona
                var registrosRaw = csv.GetRecordsAsync<dynamic>();

                await foreach (var registro in registrosRaw)
                {
                    // Convertir el ID de la evaluación del CSV
                    int idEvaluacion = Convert.ToInt32(registro.id_evaluacion);

                    // Validar si ese día evaluado (cuestionario de la jugadora) existe en la base de datos
                    var existeEvaluacion = await _context.EvaluacionesDiarias.AnyAsync(e => e.Id == idEvaluacion);
                    if (!existeEvaluacion) continue; // Si no existe el cuestionario, ignora la fila por seguridad

                    // Verificar si ya tiene métricas GPS cargadas para no duplicar
                    var yaTieneGps = await _context.MetricasGpsAvanzadas.AnyAsync(g => g.IdEvaluacion == idEvaluacion);
                    if (yaTieneGps) continue;

                    // Mapear los datos de la planilla hacia la entidad física de Entity Framework
                    var metricasFisicas = new MetricasGpsAvanzadas
                    
                    {
                        IdEvaluacion = idEvaluacion,
                        // Forzar la lectura con punto decimal estándar de los chalecos GPS
                        DistanciaTotalKm = decimal.Parse(registro.distancia_total_km.ToString(), CultureInfo.InvariantCulture),
                        VelocidadMaximaKmh = decimal.Parse(registro.velocidad_maxima_kmh.ToString(), CultureInfo.InvariantCulture),
                        DistanciaZonaAltaVelocidadMetros = decimal.Parse(registro.distancia_hsr_metros.ToString(), CultureInfo.InvariantCulture),
                        PlayerLoadIndice = decimal.Parse(registro.player_load.ToString(), CultureInfo.InvariantCulture),

                        // Los números enteros se mantienen igual ya que no sufren desvíos por comas
                        CantidadAceleracionesIntensas = Convert.ToInt32(registro.aceleraciones_intensas),
                        CantidadDesaceleracionesIntensas = Convert.ToInt32(registro.desaceleraciones_intensas),
                        FcPromedioLpm = Convert.ToInt32(registro.fc_promedio),
                        FcMaximaLpm = Convert.ToInt32(registro.fc_maxima)
                    };


                    _context.MetricasGpsAvanzadas.Add(metricasFisicas);
                }

                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
