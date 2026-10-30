using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenteEnJuego.Datos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEvaluacionesKinesiologicas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evaluaciones_kinesiologicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdJugadora = table.Column<int>(type: "int", nullable: false),
                    IdProfesional = table.Column<int>(type: "int", nullable: true),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegionCorporal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Lateralidad = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Dolor = table.Column<int>(type: "int", nullable: true),
                    Movilidad = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LimitacionFuncional = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FaseRehabilitacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TratamientoRealizado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Objetivos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProximoControl = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluaciones_kinesiologicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_evaluaciones_kinesiologicas_jugadoras_IdJugadora",
                        column: x => x.IdJugadora,
                        principalTable: "jugadoras",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_evaluaciones_kinesiologicas_staff_profesional_IdProfesional",
                        column: x => x.IdProfesional,
                        principalTable: "staff_profesional",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_evaluaciones_kinesiologicas_FechaEvaluacion",
                table: "evaluaciones_kinesiologicas",
                column: "FechaEvaluacion");

            migrationBuilder.CreateIndex(
                name: "IX_evaluaciones_kinesiologicas_IdJugadora",
                table: "evaluaciones_kinesiologicas",
                column: "IdJugadora");

            migrationBuilder.CreateIndex(
                name: "IX_evaluaciones_kinesiologicas_IdProfesional",
                table: "evaluaciones_kinesiologicas",
                column: "IdProfesional");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evaluaciones_kinesiologicas");
        }
    }
}
