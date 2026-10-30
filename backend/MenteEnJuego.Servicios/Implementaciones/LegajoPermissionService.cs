using System.Globalization;
using System.Text;

namespace MenteEnJuego.Servicios.Seguridad
{
    public class LegajoPermissionService
        : ILegajoPermissionService
    {
        public LegajoPermissions ObtenerPermisos(
            string rol,
            string? especialidad)
        {
            var rolNormalizado = Normalizar(rol);
            var especialidadNormalizada =
                Normalizar(especialidad);

            return rolNormalizado switch
            {
                "admin" =>
                    CrearPermisosAdministrador(),

                "jugadora" =>
                    CrearPermisosJugadora(),

                "entrenador" =>
                    CrearPermisosCuerpoTecnico(),

                "medico" =>
                    CrearPermisosProfesionalSalud(
                        especialidadNormalizada
                    ),

                // Compatibilidad por si alguna cuenta histórica
                // conserva la especialidad como rol.
                "nutricionista" =>
                    CrearPermisosNutricionista(),

                "psicologa" or "psicologo" =>
                    CrearPermisosPsicologia(),

                "kinesiologo" or "fisioterapeuta" =>
                    CrearPermisosKinesiologia(),

                _ => new LegajoPermissions()
            };
        }

        private static LegajoPermissions
            CrearPermisosAdministrador()
        {
            /*
             * El administrador gestiona técnicamente el sistema.
             * Puede comprobar el funcionamiento general del Legajo,
             * pero no recibe contenido psicológico confidencial ni
             * detalle nutricional especializado.
             */
            return new LegajoPermissions
            {
                PuedeVerDatosPersonales = true,
                PuedeVerDatosContacto = true,
                PuedeVerMedidasCorporalesActuales = true,

                PuedeVerTimeline = true,
                PuedeVerTimelineWellness = true,
                PuedeVerTimelinePsicologia = false,
                PuedeVerTimelineNutricion = false,
                PuedeVerTimelineLesiones = true,

                PuedeVerResumen = true,
                PuedeVerMatrizHistorica = true,
                PuedeVerIndicadoresWellness = true,
                PuedeVerIndicadoresLesiones = true,
                PuedeVerDetalleNutricional = false,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerEvolucionRpe = true
            };
        }

        private static LegajoPermissions
            CrearPermisosJugadora()
        {
            return new LegajoPermissions
            {
                PuedeVerDatosPersonales = true,
                PuedeVerDatosContacto = true,
                PuedeVerMedidasCorporalesActuales = true,

                // La jugadora accede a su estado actual,
                // pero no a la cronología profesional completa.
                PuedeVerTimeline = false,

                PuedeVerResumen = true,
                PuedeVerMatrizHistorica = true,
                PuedeVerIndicadoresWellness = true,
                PuedeVerIndicadoresLesiones = false,
                PuedeVerDetalleNutricional = false,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerEvolucionRpe = true
            };
        }

        private static LegajoPermissions
            CrearPermisosCuerpoTecnico()
        {
            return new LegajoPermissions
            {
                PuedeVerDatosPersonales = true,
                PuedeVerDatosContacto = true,

                // Incluye altura, peso e IMC actuales.
                PuedeVerMedidasCorporalesActuales = true,

                PuedeVerTimeline = true,
                PuedeVerTimelineWellness = true,
                PuedeVerTimelinePsicologia = false,
                PuedeVerTimelineNutricion = false,
                PuedeVerTimelineLesiones = true,

                PuedeVerResumen = true,
                PuedeVerMatrizHistorica = true,
                PuedeVerIndicadoresWellness = true,
                PuedeVerIndicadoresLesiones = true,
                PuedeVerDetalleNutricional = false,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerEvolucionRpe = true
            };
        }

        private static LegajoPermissions
            CrearPermisosProfesionalSalud(
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

            if (EsEspecialidadKinesiologia(especialidad))
            {
                return CrearPermisosKinesiologia();
            }

            if (EsEspecialidadReadaptacion(especialidad))
            {
                return CrearPermisosReadaptacion();
            }

            // Médico clínico u otra especialidad médica general.
            return CrearPermisosMedicoClinico();
        }

        private static LegajoPermissions
            CrearPermisosMedicoClinico()
        {
            return new LegajoPermissions
            {
                PuedeVerDatosPersonales = true,
                PuedeVerDatosContacto = true,
                PuedeVerMedidasCorporalesActuales = true,

                PuedeVerTimeline = true,
                PuedeVerTimelineWellness = true,
                PuedeVerTimelinePsicologia = false,
                PuedeVerTimelineNutricion = false,
                PuedeVerTimelineLesiones = true,

                PuedeVerResumen = true,
                PuedeVerMatrizHistorica = true,
                PuedeVerIndicadoresWellness = true,
                PuedeVerIndicadoresLesiones = true,
                PuedeVerDetalleNutricional = false,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerEvolucionRpe = true,

                PuedeEditarLesiones = true
            };
        }

        private static LegajoPermissions
            CrearPermisosKinesiologia()
        {
            return new LegajoPermissions
            {
                PuedeVerDatosPersonales = true,
                PuedeVerDatosContacto = true,
                PuedeVerMedidasCorporalesActuales = true,

                PuedeVerTimeline = true,
                PuedeVerTimelineWellness = true,
                PuedeVerTimelinePsicologia = false,
                PuedeVerTimelineNutricion = false,
                PuedeVerTimelineLesiones = true,

                PuedeVerResumen = true,
                PuedeVerMatrizHistorica = true,
                PuedeVerIndicadoresWellness = true,
                PuedeVerIndicadoresLesiones = true,
                PuedeVerDetalleNutricional = false,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerEvolucionRpe = true,

                PuedeEditarLesiones = true,
                PuedeEditarReadaptacion = true
            };
        }

        private static LegajoPermissions
            CrearPermisosReadaptacion()
        {
            return new LegajoPermissions
            {
                PuedeVerDatosPersonales = true,
                PuedeVerDatosContacto = true,
                PuedeVerMedidasCorporalesActuales = true,

                PuedeVerTimeline = true,
                PuedeVerTimelineWellness = true,
                PuedeVerTimelinePsicologia = false,
                PuedeVerTimelineNutricion = false,
                PuedeVerTimelineLesiones = true,

                PuedeVerResumen = true,
                PuedeVerMatrizHistorica = true,
                PuedeVerIndicadoresWellness = true,
                PuedeVerIndicadoresLesiones = true,
                PuedeVerDetalleNutricional = false,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerEvolucionRpe = true,

                PuedeEditarReadaptacion = true
            };
        }

        private static LegajoPermissions
            CrearPermisosNutricionista()
        {
            return new LegajoPermissions
            {
                PuedeVerDatosPersonales = true,
                PuedeVerDatosContacto = true,
                PuedeVerMedidasCorporalesActuales = true,

                PuedeVerTimeline = true,
                PuedeVerTimelineWellness = true,
                PuedeVerTimelinePsicologia = false,
                PuedeVerTimelineNutricion = true,
                PuedeVerTimelineLesiones = false,

                PuedeVerResumen = true,
                PuedeVerMatrizHistorica = true,
                PuedeVerIndicadoresWellness = true,
                PuedeVerIndicadoresLesiones = false,
                PuedeVerDetalleNutricional = true,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerEvolucionRpe = true,

                PuedeEditarPeso = true
            };
        }

        private static LegajoPermissions
            CrearPermisosPsicologia()
        {
            return new LegajoPermissions
            {
                PuedeVerDatosPersonales = true,
                PuedeVerDatosContacto = true,
                PuedeVerMedidasCorporalesActuales = false,

                PuedeVerTimeline = true,
                PuedeVerTimelineWellness = true,
                PuedeVerTimelinePsicologia = true,
                PuedeVerTimelineNutricion = false,
                PuedeVerTimelineLesiones = false,

                PuedeVerResumen = true,
                PuedeVerMatrizHistorica = true,
                PuedeVerIndicadoresWellness = true,
                PuedeVerIndicadoresLesiones = false,
                PuedeVerDetalleNutricional = false,
                PuedeVerRiesgo = true,
                PuedeVerDisponibilidad = true,
                PuedeVerEvolucionRpe = false,

                PuedeEditarPsicologia = true
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
                "fisioterapeuta";
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
                    UnicodeCategory.NonSpacingMark
                )
                .ToArray();

            return new string(caracteres)
                .Normalize(NormalizationForm.FormC);
        }
    }
}