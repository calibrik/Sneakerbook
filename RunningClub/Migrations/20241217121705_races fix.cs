using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunningClub.Migrations
{
    /// <inheritdoc />
    public partial class racesfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Races_AppUser_AppUserId",
                table: "Races");

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "Races",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "Races",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_AppUser_AppUserId",
                table: "Races",
                column: "AppUserId",
                principalTable: "AppUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Races_AppUser_AppUserId",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Races");

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "Races",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_AppUser_AppUserId",
                table: "Races",
                column: "AppUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
