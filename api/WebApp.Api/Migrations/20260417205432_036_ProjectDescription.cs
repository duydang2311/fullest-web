using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _036_ProjectDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "about",
                table: "projects",
                newName: "description_preview"
            );

            migrationBuilder.AddColumn<string>(
                name: "description_json",
                table: "projects",
                type: "text",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "description_json", table: "projects");

            migrationBuilder.RenameColumn(
                name: "description_preview",
                table: "projects",
                newName: "about"
            );
        }
    }
}
