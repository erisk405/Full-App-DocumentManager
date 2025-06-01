using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_API.Migrations
{
    /// <inheritdoc />
    public partial class Permissionattribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Delete",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Read",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Write",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Delete",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Read",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Write",
                table: "Roles");
        }
    }
}
