using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunningClub.Migrations
{
    /// <inheritdoc />
    public partial class renaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Races",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Clubs",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Races",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Races",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Clubs",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Races",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
