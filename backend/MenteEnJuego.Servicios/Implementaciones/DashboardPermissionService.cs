using System;

using System.Globalization;
using System.Linq;
using System.Text;

using System.Globalization;
using MenteEnJuego.Servicios.Interfaces;
using MenteEnJuego.Servicios.Seguridad;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class DashboardPermissionService
        : IDashboardPermissionService
    {
        public DashboardPermissions ObtenerPermisos(
    string rol,
    string? especialidad)
        {
            var rolNormalizado = Normalizar(rol);
            var especialidadNormalizada = Normalizar(especialidad);

            return rolNormalizado switch
            {
                "admin" =>
                    CrearPermisosAdministrador(),

                "jugadora" =>
                    CrearPermisosJugadora(),

                "entrenador" or
                "director tecnico" or
                "asistente de campo" or
                "entrenador de arqueras" or
                "preparador fisico" =>
                    CrearPermisosEntrenador(),

                "medico" =>
                    CrearPermisosProfesional(
                        especialidadNormalizada),

                // Compatibilidad con cuentas donde la especialidad
                // fue guardada directamente como rol.
                "nutricionista" =>
                    CrearPermisosNutricionista(),

                "psicologa" or "psicologo" =>
                    CrearPermisosPsicologia(),

                "kinesiologo" or
                "fisioterapeuta" or
                "readaptador" or
                "readaptador deportivo" =>
                    CrearPermisosKinesiologia(),

                _ => new DashboardPermissions()
            };
        }



        private static DashboardPermissions
    CrearPermisosAdministrador()
        {
            return new DashboardPermissions
            {
                PuedeVerDashboardTecnico = true,
                PuedeVerCentroOperaciones = true,
                PuedeVerSesiones = true,

                PuedeVerComparativaGps = true,
                PuedeVerHeatmapGps = true,

                PuedeVerWellness = true,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerDolor = true,

                PuedeConsultarJugadoras = true,
                PuedeConsultarGraficosIndividuales = true,
                PuedeConsultarDolorIndividual = true
            };
        }

        private static DashboardPermissions
    CrearPermisosJugadora()
        {
            return new DashboardPermissions
            {
                PuedeVerDashboardTecnico = false,
                PuedeVerCentroOperaciones = false,
                PuedeVerSesiones = false,

                PuedeVerComparativaGps = false,
                PuedeVerHeatmapGps = false,

                PuedeVerWellness = true,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerDolor = true,

                PuedeConsultarJugadoras = false,

                // Estos permisos siempre estarán sujetos
                // a la validación de su propio idJugadora.
                PuedeConsultarGraficosIndividuales = true,
                PuedeConsultarDolorIndividual = true
            };
        }

        private static DashboardPermissions
       CrearPermisosEntrenador()
        {
            return new DashboardPermissions
            {
                PuedeVerDashboardTecnico = true,
                PuedeVerCentroOperaciones = false,
                PuedeVerSesiones = true,

                PuedeVerComparativaGps = true,
                PuedeVerHeatmapGps = true,

                PuedeVerWellness = true,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerDolor = true,

                PuedeConsultarJugadoras = true,
                PuedeConsultarGraficosIndividuales = true,
                PuedeConsultarDolorIndividual = true
            };
        }

        private static DashboardPermissions
    CrearPermisosProfesional(
        string especialidad)
        {
            if (EsEspecialidadPsicologia(especialidad))
            {
                return CrearPermisosPsicologia();
            }

            if (EsEspecialidadNutricion(especialidad))
            {
                return CrearPermisosNutricionista();
            }

            if (EsEspecialidadKinesiologia(especialidad) ||
                EsEspecialidadReadaptacion(especialidad))
            {
                return CrearPermisosKinesiologia();
            }

            return CrearPermisosMedicoClinico();
        }
        private static DashboardPermissions
   CrearPermisosMedicoClinico()
        {
            return new DashboardPermissions
            {
                PuedeVerDashboardTecnico = true,
                PuedeVerCentroOperaciones = false,
                PuedeVerSesiones = true,

                PuedeVerComparativaGps = false,
                PuedeVerHeatmapGps = false,

                PuedeVerWellness = true,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerDolor = true,

                PuedeConsultarJugadoras = true,
                PuedeConsultarGraficosIndividuales = true,
                PuedeConsultarDolorIndividual = true
            };
        }
        private static DashboardPermissions
     CrearPermisosNutricionista()
        {
            return new DashboardPermissions
            {
                PuedeVerDashboardTecnico = true,
                PuedeVerCentroOperaciones = false,
                PuedeVerSesiones = true,

                PuedeVerComparativaGps = false,
                PuedeVerHeatmapGps = false,

                PuedeVerWellness = true,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerDolor = false,

                PuedeConsultarJugadoras = true,
                PuedeConsultarGraficosIndividuales = true,
                PuedeConsultarDolorIndividual = false
            };
        }

        private static DashboardPermissions
     CrearPermisosPsicologia()
        {
            return new DashboardPermissions
            {
                PuedeVerDashboardTecnico = true,
                PuedeVerCentroOperaciones = false,
                PuedeVerSesiones = true,

                PuedeVerComparativaGps = false,
                PuedeVerHeatmapGps = false,

                PuedeVerWellness = true,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerDolor = false,

                PuedeConsultarJugadoras = true,
                PuedeConsultarGraficosIndividuales = true,
                PuedeConsultarDolorIndividual = false
            };
        }
        private static DashboardPermissions
    CrearPermisosKinesiologia()
        {
            return new DashboardPermissions
            {
                PuedeVerDashboardTecnico = true,
                PuedeVerCentroOperaciones = false,
                PuedeVerSesiones = true,

                PuedeVerComparativaGps = false,
                PuedeVerHeatmapGps = false,

                PuedeVerWellness = true,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerDolor = true,

                PuedeConsultarJugadoras = true,
                PuedeConsultarGraficosIndividuales = true,
                PuedeConsultarDolorIndividual = true
            };
        }

        private static bool EsEspecialidadPsicologia(
    string especialidad)
        {
            return especialidad is
                "psicologa" or
                "psicologo" or
                "psicologia";
        }

        private static bool EsEspecialidadNutricion(
            string especialidad)
        {
            return especialidad is
                "nutricionista" or
                "nutricion";
        }

        private static bool EsEspecialidadKinesiologia(
            string especialidad)
        {
            return especialidad is
                "kinesiologo" or
                "kinesiologia" or
                "fisioterapeuta" or
                "fisioterapia";
        }

        private static bool EsEspecialidadReadaptacion(
            string especialidad)
        {
            return especialidad is
                "readaptador" or
                "readaptador deportivo" or
                "readaptacion";
        }

        private static string Normalizar(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            var texto = valor
                .Trim()
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD);

            var caracteres = texto
                .Where(c =>
                    CharUnicodeInfo.GetUnicodeCategory(c) !=
                    UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(caracteres)
                .Normalize(NormalizationForm.FormC);
        }
    }


}
