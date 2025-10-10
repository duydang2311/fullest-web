using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _022_AddTaskPriorityRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "priority_id",
                table: "tasks",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_tasks_priority_id",
                table: "tasks",
                column: "priority_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_priorities_priority_id",
                table: "tasks",
                column: "priority_id",
                principalTable: "priorities",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tasks_priorities_priority_id",
                table: "tasks"
            );

            migrationBuilder.DropIndex(name: "ix_tasks_priority_id", table: "tasks");

            migrationBuilder.DropColumn(name: "priority_id", table: "tasks");
        }
    }
}
