using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunningClub.Migrations
{
    /// <inheritdoc />
    public partial class addedimageidtoraceandclub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Races",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Clubs");
        }
    }
}
