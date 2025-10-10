using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _019_DeletedTimeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_tasks_deleted_time",
                table: "tasks",
                column: "deleted_time"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_tasks_deleted_time", table: "tasks");
        }
    }
}
