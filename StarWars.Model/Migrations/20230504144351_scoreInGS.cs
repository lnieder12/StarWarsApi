using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarWars.Model.Migrations
{
    /// <inheritdoc />
    public partial class scoreInGS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Damage",
                table: "GameSoldiers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Damage",
                table: "GameSoldiers");
        }
    }
}
