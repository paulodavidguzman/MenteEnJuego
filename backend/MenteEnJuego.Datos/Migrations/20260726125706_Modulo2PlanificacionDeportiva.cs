using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenteEnJuego.Datos.Migrations
{
    /// <inheritdoc />
    public partial class Modulo2PlanificacionDeportiva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MicrocicloId",
                table: "SesionesDeportivas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Microciclos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NumeroSemana = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "date", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "date", nullable: false),
                    ObjetivoGeneral = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TipoMicrociclo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Planificado"),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Microciclos", x => x.Id);
                    table.CheckConstraint("CK_Microciclos_Estado", "[Estado] IN ('Planificado','EnEjecucion','Finalizado','Cancelado')");
                    table.CheckConstraint("CK_Microciclos_Fechas", "[FechaFin] >= [FechaInicio]");
                    table.CheckConstraint("CK_Microciclos_NumeroSemana", "[NumeroSemana] > 0");
                    table.CheckConstraint("CK_Microciclos_Tipo", "[TipoMicrociclo] IN ('Carga','Competitivo','Precompetitivo','Recuperacion','Regenerativo','Evaluacion','Descarga')");
                });

            migrationBuilder.CreateTable(
                name: "PlanificacionesTecnicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SesionDeportivaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioResponsableId = table.Column<int>(type: "int", nullable: true),
                    ObjetivoGeneral = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DuracionPlanificadaMinutos = table.Column<int>(type: "int", nullable: true),
                    IntensidadPlanificada = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RpeObjetivo = table.Column<byte>(type: "tinyint", nullable: true),
                    CargaPlanificada = table.Column<int>(type: "int", nullable: true, computedColumnSql: "[DuracionPlanificadaMinutos] * CONVERT(int, [RpeObjetivo])", stored: true),
                    MaterialesNecesarios = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    Precauciones = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Borrador"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanificacionesTecnicas", x => x.Id);
                    table.CheckConstraint("CK_PlanificacionesTecnicas_Duracion", "[DuracionPlanificadaMinutos] IS NULL OR [DuracionPlanificadaMinutos] > 0");
                    table.CheckConstraint("CK_PlanificacionesTecnicas_Estado", "[Estado] IN ('Borrador','Pendiente','Aprobada','EnEjecucion','Ejecutada','Cancelada')");
                    table.CheckConstraint("CK_PlanificacionesTecnicas_Intensidad", "[IntensidadPlanificada] IS NULL OR [IntensidadPlanificada] IN ('MuyBaja','Baja','Moderada','Alta','MuyAlta')");
                    table.CheckConstraint("CK_PlanificacionesTecnicas_Rpe", "[RpeObjetivo] IS NULL OR [RpeObjetivo] BETWEEN 1 AND 10");
                    table.ForeignKey(
                        name: "FK_PlanificacionesTecnicas_Sesion",
                        column: x => x.SesionDeportivaId,
                        principalTable: "SesionesDeportivas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlanificacionesTecnicas_Usuario",
                        column: x => x.UsuarioResponsableId,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "BloquesSesion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanificacionTecnicaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TipoBloque = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    DuracionPlanificadaMinutos = table.Column<int>(type: "int", nullable: false),
                    IntensidadPlanificada = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Objetivo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PausaPlanificadaMinutos = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloquesSesion", x => x.Id);
                    table.CheckConstraint("CK_BloquesSesion_Duracion", "[DuracionPlanificadaMinutos] > 0");
                    table.CheckConstraint("CK_BloquesSesion_Intensidad", "[IntensidadPlanificada] IS NULL OR [IntensidadPlanificada] IN ('MuyBaja','Baja','Moderada','Alta','MuyAlta')");
                    table.CheckConstraint("CK_BloquesSesion_Orden", "[Orden] > 0");
                    table.CheckConstraint("CK_BloquesSesion_Pausa", "[PausaPlanificadaMinutos] >= 0");
                    table.CheckConstraint("CK_BloquesSesion_Tipo", "[TipoBloque] IN ('Activacion','EntradaEnCalor','Tecnico','Tactico','Fisico','PartePrincipal','JuegoAplicado','VueltaALaCalma','Recuperacion','Evaluacion')");
                    table.ForeignKey(
                        name: "FK_BloquesSesion_Planificacion",
                        column: x => x.PlanificacionTecnicaId,
                        principalTable: "PlanificacionesTecnicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContenidosPlanificados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanificacionTecnicaId = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Prioridad = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Media"),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContenidosPlanificados", x => x.Id);
                    table.CheckConstraint("CK_ContenidosPlanificados_Categoria", "[Categoria] IN ('Tecnico','Tactico','Fisico','Preventivo','Psicologico','Regenerativo','Arqueras')");
                    table.CheckConstraint("CK_ContenidosPlanificados_Orden", "[Orden] > 0");
                    table.CheckConstraint("CK_ContenidosPlanificados_Prioridad", "[Prioridad] IN ('Alta','Media','Baja')");
                    table.ForeignKey(
                        name: "FK_ContenidosPlanificados_Planificacion",
                        column: x => x.PlanificacionTecnicaId,
                        principalTable: "PlanificacionesTecnicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndicadoresPlanificados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanificacionTecnicaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ValorObjetivo = table.Column<decimal>(type: "decimal(12,3)", nullable: true),
                    ValorMinimo = table.Column<decimal>(type: "decimal(12,3)", nullable: true),
                    ValorMaximo = table.Column<decimal>(type: "decimal(12,3)", nullable: true),
                    FuenteDatos = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EsObligatorio = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicadoresPlanificados", x => x.Id);
                    table.CheckConstraint("CK_IndicadoresPlanificados_Categoria", "[Categoria] IN ('GPS','Wellness','Tecnico','Tactico','Fisico','Medico','Asistencia','CargaInterna')");
                    table.CheckConstraint("CK_IndicadoresPlanificados_FuenteDatos", "[FuenteDatos] IN ('GPS','FormularioPre','FormularioPost','RegistroManual','EvaluacionMedica','AnalisisTecnico')");
                    table.CheckConstraint("CK_IndicadoresPlanificados_Orden", "[Orden] > 0");
                    table.CheckConstraint("CK_IndicadoresPlanificados_Rango", "[ValorMinimo] IS NULL OR [ValorMaximo] IS NULL OR [ValorMinimo] <= [ValorMaximo]");
                    table.ForeignKey(
                        name: "FK_IndicadoresPlanificados_Planificacion",
                        column: x => x.PlanificacionTecnicaId,
                        principalTable: "PlanificacionesTecnicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjetivosEspecificos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanificacionTecnicaId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Prioridad = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Media"),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Cumplido = table.Column<bool>(type: "bit", nullable: true),
                    ObservacionResultado = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjetivosEspecificos", x => x.Id);
                    table.CheckConstraint("CK_ObjetivosEspecificos_Orden", "[Orden] > 0");
                    table.CheckConstraint("CK_ObjetivosEspecificos_Prioridad", "[Prioridad] IN ('Alta','Media','Baja')");
                    table.ForeignKey(
                        name: "FK_ObjetivosEspecificos_Planificacion",
                        column: x => x.PlanificacionTecnicaId,
                        principalTable: "PlanificacionesTecnicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffPlanificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanificacionTecnicaId = table.Column<int>(type: "int", nullable: false),
                    StaffProfesionalId = table.Column<int>(type: "int", nullable: false),
                    FuncionEnSesion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EsResponsablePrincipal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffPlanificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffPlanificaciones_Planificacion",
                        column: x => x.PlanificacionTecnicaId,
                        principalTable: "PlanificacionesTecnicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffPlanificaciones_Staff",
                        column: x => x.StaffProfesionalId,
                        principalTable: "staff_profesional",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SesionesDeportivas_MicrocicloId",
                table: "SesionesDeportivas",
                column: "MicrocicloId");

            migrationBuilder.CreateIndex(
                name: "UX_BloquesSesion_Planificacion_Orden",
                table: "BloquesSesion",
                columns: new[] { "PlanificacionTecnicaId", "Orden" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ContenidosPlanificados_Planificacion_Categoria_Nombre",
                table: "ContenidosPlanificados",
                columns: new[] { "PlanificacionTecnicaId", "Categoria", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndicadoresPlanificados_Planificacion_Categoria",
                table: "IndicadoresPlanificados",
                columns: new[] { "PlanificacionTecnicaId", "Categoria" });

            migrationBuilder.CreateIndex(
                name: "UX_IndicadoresPlanificados_Planificacion_Orden",
                table: "IndicadoresPlanificados",
                columns: new[] { "PlanificacionTecnicaId", "Orden" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Microciclos_Estado",
                table: "Microciclos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Microciclos_Fechas",
                table: "Microciclos",
                columns: new[] { "FechaInicio", "FechaFin" });

            migrationBuilder.CreateIndex(
                name: "UX_ObjetivosEspecificos_Planificacion_Orden",
                table: "ObjetivosEspecificos",
                columns: new[] { "PlanificacionTecnicaId", "Orden" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanificacionesTecnicas_Estado",
                table: "PlanificacionesTecnicas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_PlanificacionesTecnicas_UsuarioResponsable",
                table: "PlanificacionesTecnicas",
                column: "UsuarioResponsableId");

            migrationBuilder.CreateIndex(
                name: "UX_PlanificacionesTecnicas_SesionDeportivaId",
                table: "PlanificacionesTecnicas",
                column: "SesionDeportivaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffPlanificaciones_StaffProfesionalId",
                table: "StaffPlanificaciones",
                column: "StaffProfesionalId");

            migrationBuilder.CreateIndex(
                name: "UX_StaffPlanificaciones_Planificacion_Staff",
                table: "StaffPlanificaciones",
                columns: new[] { "PlanificacionTecnicaId", "StaffProfesionalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_StaffPlanificaciones_ResponsablePrincipal",
                table: "StaffPlanificaciones",
                column: "PlanificacionTecnicaId",
                unique: true,
                filter: "[EsResponsablePrincipal] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_SesionesDeportivas_Microciclos",
                table: "SesionesDeportivas",
                column: "MicrocicloId",
                principalTable: "Microciclos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SesionesDeportivas_Microciclos",
                table: "SesionesDeportivas");

            migrationBuilder.DropTable(
                name: "BloquesSesion");

            migrationBuilder.DropTable(
                name: "ContenidosPlanificados");

            migrationBuilder.DropTable(
                name: "IndicadoresPlanificados");

            migrationBuilder.DropTable(
                name: "Microciclos");

            migrationBuilder.DropTable(
                name: "ObjetivosEspecificos");

            migrationBuilder.DropTable(
                name: "StaffPlanificaciones");

            migrationBuilder.DropTable(
                name: "PlanificacionesTecnicas");

            migrationBuilder.DropIndex(
                name: "IX_SesionesDeportivas_MicrocicloId",
                table: "SesionesDeportivas");

            migrationBuilder.DropColumn(
                name: "MicrocicloId",
                table: "SesionesDeportivas");
        }
    }
}
