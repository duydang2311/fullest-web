using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _035_TaskDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tasks_comments_initial_comment_id",
                table: "tasks"
            );

            migrationBuilder.DropIndex(name: "ix_tasks_initial_comment_id", table: "tasks");

            migrationBuilder.DropColumn(name: "initial_comment_id", table: "tasks");

            migrationBuilder.AddColumn<string>(
                name: "description_json",
                table: "tasks",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "description_preview",
                table: "tasks",
                type: "text",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "description_json", table: "tasks");

            migrationBuilder.DropColumn(name: "description_preview", table: "tasks");

            migrationBuilder.AddColumn<long>(
                name: "initial_comment_id",
                table: "tasks",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_tasks_initial_comment_id",
                table: "tasks",
                column: "initial_comment_id",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_comments_initial_comment_id",
                table: "tasks",
                column: "initial_comment_id",
                principalTable: "comments",
                principalColumn: "id"
            );
        }
    }
}
