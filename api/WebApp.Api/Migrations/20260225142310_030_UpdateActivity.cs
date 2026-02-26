using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _030_UpdateActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_activities_tasks_task_id",
                table: "activities"
            );

            migrationBuilder.RenameColumn(
                name: "task_id",
                table: "activities",
                newName: "task_entity_id"
            );

            migrationBuilder.RenameColumn(name: "data", table: "activities", newName: "metadata");

            migrationBuilder.RenameIndex(
                name: "ix_activities_task_id",
                table: "activities",
                newName: "ix_activities_task_entity_id"
            );

            migrationBuilder.AddColumn<long>(
                name: "context_id",
                table: "activities",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "context_kind",
                table: "activities",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "resource_id",
                table: "activities",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "resource_kind",
                table: "activities",
                type: "integer",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_activities_context_id",
                table: "activities",
                column: "context_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_activities_context_kind",
                table: "activities",
                column: "context_kind"
            );

            migrationBuilder.CreateIndex(
                name: "ix_activities_resource_id",
                table: "activities",
                column: "resource_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_activities_resource_kind",
                table: "activities",
                column: "resource_kind"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_activities_tasks_task_entity_id",
                table: "activities",
                column: "task_entity_id",
                principalTable: "tasks",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_activities_tasks_task_entity_id",
                table: "activities"
            );

            migrationBuilder.DropIndex(name: "ix_activities_context_id", table: "activities");

            migrationBuilder.DropIndex(name: "ix_activities_context_kind", table: "activities");

            migrationBuilder.DropIndex(name: "ix_activities_resource_id", table: "activities");

            migrationBuilder.DropIndex(name: "ix_activities_resource_kind", table: "activities");

            migrationBuilder.DropColumn(name: "context_id", table: "activities");

            migrationBuilder.DropColumn(name: "context_kind", table: "activities");

            migrationBuilder.DropColumn(name: "resource_id", table: "activities");

            migrationBuilder.DropColumn(name: "resource_kind", table: "activities");

            migrationBuilder.RenameColumn(
                name: "task_entity_id",
                table: "activities",
                newName: "task_id"
            );

            migrationBuilder.RenameColumn(name: "metadata", table: "activities", newName: "data");

            migrationBuilder.RenameIndex(
                name: "ix_activities_task_entity_id",
                table: "activities",
                newName: "ix_activities_task_id"
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
    }
}
