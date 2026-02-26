using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _032_RemoveActivityContextAndResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_activities_context_kind", table: "activities");

            migrationBuilder.DropIndex(name: "ix_activities_resource_kind", table: "activities");

            migrationBuilder.DropColumn(name: "context_kind", table: "activities");

            migrationBuilder.DropColumn(name: "resource_kind", table: "activities");

            migrationBuilder.RenameColumn(
                name: "resource_id",
                table: "activities",
                newName: "task_id"
            );

            migrationBuilder.RenameColumn(
                name: "context_id",
                table: "activities",
                newName: "project_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_activities_resource_id",
                table: "activities",
                newName: "ix_activities_task_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_activities_context_id",
                table: "activities",
                newName: "ix_activities_project_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_activities_projects_project_id",
                table: "activities",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_activities_tasks_task_id",
                table: "activities",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_activities_projects_project_id",
                table: "activities"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_activities_tasks_task_id",
                table: "activities"
            );

            migrationBuilder.RenameColumn(
                name: "task_id",
                table: "activities",
                newName: "resource_id"
            );

            migrationBuilder.RenameColumn(
                name: "project_id",
                table: "activities",
                newName: "context_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_activities_task_id",
                table: "activities",
                newName: "ix_activities_resource_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_activities_project_id",
                table: "activities",
                newName: "ix_activities_context_id"
            );

            migrationBuilder.AddColumn<byte>(
                name: "context_kind",
                table: "activities",
                type: "smallint",
                nullable: true
            );

            migrationBuilder.AddColumn<byte>(
                name: "resource_kind",
                table: "activities",
                type: "smallint",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_activities_context_kind",
                table: "activities",
                column: "context_kind"
            );

            migrationBuilder.CreateIndex(
                name: "ix_activities_resource_kind",
                table: "activities",
                column: "resource_kind"
            );
        }
    }
}
