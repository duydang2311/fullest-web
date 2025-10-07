using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _015_AddDefaultProjectStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "is_default", table: "project_statuses");

            migrationBuilder.AddColumn<long>(
                name: "default_project_status_id",
                table: "projects",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_projects_default_project_status_id",
                table: "projects",
                column: "default_project_status_id",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_projects_project_status_default_project_status_id",
                table: "projects",
                column: "default_project_status_id",
                principalTable: "project_statuses",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_projects_project_status_default_project_status_id",
                table: "projects"
            );

            migrationBuilder.DropIndex(
                name: "ix_projects_default_project_status_id",
                table: "projects"
            );

            migrationBuilder.DropColumn(name: "default_project_status_id", table: "projects");

            migrationBuilder.AddColumn<bool>(
                name: "is_default",
                table: "project_statuses",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );
        }
    }
}
