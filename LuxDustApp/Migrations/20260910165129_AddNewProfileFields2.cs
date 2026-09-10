using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxDustApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProfileFields2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasProfessionalCare",
                table: "Profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReadyForMultiStep",
                table: "Profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SunSensitivity",
                table: "Profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TendencyToEdema",
                table: "Profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TexturePreference",
                table: "Profiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasProfessionalCare",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "ReadyForMultiStep",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "SunSensitivity",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TendencyToEdema",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TexturePreference",
                table: "Profiles");
        }
    }
}
