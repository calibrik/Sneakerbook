using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunningClub.Migrations
{
    /// <inheritdoc />
    public partial class addlengthandisCompletedtorace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Races",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Length",
                table: "Races",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "Mileage",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Races");

            migrationBuilder.AlterColumn<float>(
                name: "Mileage",
                table: "AspNetUsers",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
