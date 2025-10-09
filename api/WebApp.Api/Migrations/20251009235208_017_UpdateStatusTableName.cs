using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _017_UpdateStatusTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_statuses_status_status_id",
                table: "project_statuses");

            migrationBuilder.DropForeignKey(
                name: "fk_projects_project_status_default_project_status_id",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_project_status_project_status_id",
                table: "tasks");

            migrationBuilder.AddColumn<string>(
                name: "rank",
                table: "statuses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "fk_project_statuses_statuses_status_id",
                table: "project_statuses",
                column: "status_id",
                principalTable: "statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_projects_project_statuses_default_project_status_id",
                table: "projects",
                column: "default_project_status_id",
                principalTable: "project_statuses",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_project_statuses_project_status_id",
                table: "tasks",
                column: "project_status_id",
                principalTable: "project_statuses",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_statuses_statuses_status_id",
                table: "project_statuses");

            migrationBuilder.DropForeignKey(
                name: "fk_projects_project_statuses_default_project_status_id",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_project_statuses_project_status_id",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "rank",
                table: "statuses");

            migrationBuilder.AddForeignKey(
                name: "fk_project_statuses_status_status_id",
                table: "project_statuses",
                column: "status_id",
                principalTable: "statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_projects_project_status_default_project_status_id",
                table: "projects",
                column: "default_project_status_id",
                principalTable: "project_statuses",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_project_status_project_status_id",
                table: "tasks",
                column: "project_status_id",
                principalTable: "project_statuses",
                principalColumn: "id");
        }
    }
}
