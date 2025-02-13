using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunningClub.Migrations
{
    /// <inheritdoc />
    public partial class addedstartdatetorace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Races",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Race_StartDate",
                table: "Races",
                column: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Race_StartDate",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Races");
        }
    }
}
