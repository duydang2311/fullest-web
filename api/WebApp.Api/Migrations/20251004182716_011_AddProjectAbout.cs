using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _011_AddProjectAbout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "description",
                table: "projects",
                newName: "summary"
            );

            migrationBuilder.AddColumn<string>(
                name: "about",
                table: "projects",
                type: "text",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "about", table: "projects");

            migrationBuilder.RenameColumn(
                name: "summary",
                table: "projects",
                newName: "description"
            );
        }
    }
}
