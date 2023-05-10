using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarWars.Model.Migrations
{
    /// <inheritdoc />
    public partial class roundGameFkFinale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Games_GameId",
                table: "Rounds");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Games_GameId",
                table: "Rounds",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Games_GameId",
                table: "Rounds");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "Rounds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Games_GameId",
                table: "Rounds",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
