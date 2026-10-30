using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenteEnJuego.Datos.Migrations
{
    /// <inheritdoc />
    public partial class SepararEvaluacionesPrePost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "unidades_arbitrarias",
            table: "evaluaciones_diarias");

            migrationBuilder.AlterColumn<byte>(
                name: "sueño",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "recuperacion_tqr",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "minutos_sesion",
                table: "evaluaciones_diarias",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "humor",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "estres",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "esfuerzo_rpe",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "dolor_doms",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "dia_ciclo_menstrual",
                table: "evaluaciones_diarias",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
            migrationBuilder.AddColumn<int>(
             name: "unidades_arbitrarias",
            table: "evaluaciones_diarias",
                type: "int",
    nullable: true,
    computedColumnSql: "[esfuerzo_rpe] * [minutos_sesion]",
    stored: false);



        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
    name: "unidades_arbitrarias",
    table: "evaluaciones_diarias");

            migrationBuilder.AlterColumn<byte>(
                name: "sueño",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "recuperacion_tqr",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "minutos_sesion",
                table: "evaluaciones_diarias",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "humor",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "estres",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "esfuerzo_rpe",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "dolor_doms",
                table: "evaluaciones_diarias",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "dia_ciclo_menstrual",
                table: "evaluaciones_diarias",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
    name: "unidades_arbitrarias",
    table: "evaluaciones_diarias",
    type: "int",
    nullable: true,
    computedColumnSql: "[esfuerzo_rpe] * [minutos_sesion]",
    stored: false);


        }
    }
}
