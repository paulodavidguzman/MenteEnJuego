using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenteEnJuego.Datos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDivisionAMicrociclo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Microciclos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Division",
                table: "Microciclos");
        }
    }
}
