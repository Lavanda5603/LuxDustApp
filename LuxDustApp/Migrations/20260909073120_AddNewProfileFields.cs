using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxDustApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllergiesOther",
                table: "Profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DietType",
                table: "Profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProblemsOther",
                table: "Profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StressLevel",
                table: "Profiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllergiesOther",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "DietType",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "ProblemsOther",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "StressLevel",
                table: "Profiles");
        }
    }
}
