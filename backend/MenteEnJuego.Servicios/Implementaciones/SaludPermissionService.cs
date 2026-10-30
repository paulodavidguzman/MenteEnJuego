using System.Globalization;
using System.Linq;
using System.Text;
using MenteEnJuego.Servicios.Interfaces;
using MenteEnJuego.Servicios.Seguridad;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class SaludPermissionService
        : ISaludPermissionService
    {
        public SaludPermissions ObtenerPermisos(
            string rol,
            string? especialidad)
        {
            var rolNormalizado =
                Normalizar(rol);

            var especialidadNormalizada =
                Normalizar(especialidad);

            return rolNormalizado switch
            {
                "admin" =>
                    CrearPermisosAdministrador(),

                "medico" =>
                    CrearPermisosProfesionalSalud(
                        especialidadNormalizada),

                // Compatibilidad con cuentas donde
                // la especialidad quedó guardada como rol.
                "nutricionista" =>
                    CrearPermisosNutricionista(),

                "psicologa" or "psicologo" =>
                    CrearPermisosPsicologia(),

                "kinesiologo" or
                "fisioterapeuta" =>
                    CrearPermisosKinesiologia(),

                "readaptador" or
                "readaptador deportivo" =>
                    CrearPermisosReadaptacion(),

                _ =>
                    new SaludPermissions()
            };
        }

        private static SaludPermissions
            CrearPermisosAdministrador()
        {
            /*
             * Acceso técnico y operativo.
             * No habilita automáticamente la edición clínica.
             */
            return new SaludPermissions
            {
                PuedeVerFichaSalud = true,
                PuedeRegistrarLesion = false,
                PuedeActualizarReadaptacion = false,
                PuedeRegistrarControlNutricional = false,
                PuedeConsultarDolor = true,
                PuedeConsultarDisponibilidad = true
            };
        }

        private static SaludPermissions
            CrearPermisosProfesionalSalud(
                string especialidad)
        {
            if (EsEspecialidadNutricion(
                    especialidad))
            {
                return CrearPermisosNutricionista();
            }

            if (EsEspecialidadPsicologia(
                    especialidad))
            {
                return CrearPermisosPsicologia();
            }

            if (EsEspecialidadKinesiologia(
                    especialidad))
            {
                return CrearPermisosKinesiologia();
            }

            if (EsEspecialidadReadaptacion(
                    especialidad))
            {
                return CrearPermisosReadaptacion();
            }

            return CrearPermisosMedicoClinico();
        }

        private static SaludPermissions
            CrearPermisosMedicoClinico()
        {
            return new SaludPermissions
            {
                PuedeVerFichaSalud = true,
                PuedeRegistrarLesion = true,
                PuedeActualizarReadaptacion = false,
                PuedeRegistrarControlNutricional = false,
                PuedeConsultarDolor = true,
                PuedeConsultarDisponibilidad = true
            };
        }

        private static SaludPermissions
            CrearPermisosKinesiologia()
        {
            return new SaludPermissions
            {
                PuedeVerFichaSalud = true,
                PuedeRegistrarLesion = true,
                PuedeActualizarReadaptacion = true,
                PuedeRegistrarControlNutricional = false,
                PuedeConsultarDolor = true,
                PuedeConsultarDisponibilidad = true
            };
        }

        private static SaludPermissions
            CrearPermisosReadaptacion()
        {
            return new SaludPermissions
            {
                PuedeVerFichaSalud = true,
                PuedeRegistrarLesion = false,
                PuedeActualizarReadaptacion = true,
                PuedeRegistrarControlNutricional = false,
                PuedeConsultarDolor = true,
                PuedeConsultarDisponibilidad = true
            };
        }

        private static SaludPermissions
            CrearPermisosNutricionista()
        {
            return new SaludPermissions
            {
                PuedeVerFichaSalud = true,
                PuedeRegistrarLesion = false,
                PuedeActualizarReadaptacion = false,
                PuedeRegistrarControlNutricional = true,
                PuedeConsultarDolor = false,
                PuedeConsultarDisponibilidad = true
            };
        }

        private static SaludPermissions
            CrearPermisosPsicologia()
        {
            return new SaludPermissions
            {
                PuedeVerFichaSalud = true,
                PuedeRegistrarLesion = false,
                PuedeActualizarReadaptacion = false,
                PuedeRegistrarControlNutricional = false,
                PuedeConsultarDolor = false,
                PuedeConsultarDisponibilidad = true
            };
        }

        private static bool
            EsEspecialidadNutricion(
                string especialidad)
        {
            return especialidad is
                "nutricionista" or
                "nutricion";
        }

        private static bool
            EsEspecialidadPsicologia(
                string especialidad)
        {
            return especialidad is
                "psicologa" or
                "psicologo" or
                "psicologia";
        }

        private static bool
            EsEspecialidadKinesiologia(
                string especialidad)
        {
            return especialidad is
                "kinesiologo" or
                "kinesiologia" or
                "fisioterapeuta" or
                "fisioterapia";
        }

        private static bool
            EsEspecialidadReadaptacion(
                string especialidad)
        {
            return especialidad is
                "readaptador" or
                "readaptador deportivo" or
                "readaptacion";
        }

        private static string Normalizar(
            string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            var texto = valor
                .Trim()
                .ToLowerInvariant()
                .Normalize(
                    NormalizationForm.FormD);

            var caracteres = texto
                .Where(c =>
                    CharUnicodeInfo
                        .GetUnicodeCategory(c) !=
                    UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(caracteres)
                .Normalize(
                    NormalizationForm.FormC);
        }
    }
}