using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_API.Migrations
{
    /// <inheritdoc />
    public partial class RoleName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role_name",
                table: "Roles",
                newName: "RoleName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "Roles",
                newName: "Role_name");
        }
    }
}
