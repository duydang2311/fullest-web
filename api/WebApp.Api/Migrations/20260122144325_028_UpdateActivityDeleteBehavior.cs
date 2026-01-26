using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _028_UpdateActivityDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_activities_tasks_task_id",
                table: "activities"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_activities_users_actor_id",
                table: "activities"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_activities_tasks_task_id",
                table: "activities",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_activities_users_actor_id",
                table: "activities",
                column: "actor_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_activities_tasks_task_id",
                table: "activities"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_activities_users_actor_id",
                table: "activities"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_activities_tasks_task_id",
                table: "activities",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_activities_users_actor_id",
                table: "activities",
                column: "actor_id",
                principalTable: "users",
                principalColumn: "id"
            );
        }
    }
}
