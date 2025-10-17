using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _023_AddComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "description", table: "tasks");

            migrationBuilder.CreateSequence(name: "CommentHiLoSequence", incrementBy: 10);

            migrationBuilder.AddColumn<long>(
                name: "initial_comment_id",
                table: "tasks",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    created_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    task_id = table.Column<long>(type: "bigint", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    content_json = table.Column<string>(type: "jsonb", nullable: true),
                    content_preview = table.Column<string>(type: "text", nullable: true),
                    deleted_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_comments_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_comments_users_creator_id",
                        column: x => x.creator_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_tasks_initial_comment_id",
                table: "tasks",
                column: "initial_comment_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_comments_creator_id",
                table: "comments",
                column: "creator_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_comments_deleted_time",
                table: "comments",
                column: "deleted_time"
            );

            migrationBuilder.CreateIndex(
                name: "ix_comments_task_id",
                table: "comments",
                column: "task_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_comments_initial_comment_id",
                table: "tasks",
                column: "initial_comment_id",
                principalTable: "comments",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tasks_comments_initial_comment_id",
                table: "tasks"
            );

            migrationBuilder.DropTable(name: "comments");

            migrationBuilder.DropIndex(name: "ix_tasks_initial_comment_id", table: "tasks");

            migrationBuilder.DropColumn(name: "initial_comment_id", table: "tasks");

            migrationBuilder.DropSequence(name: "CommentHiLoSequence");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "tasks",
                type: "text",
                nullable: true
            );
        }
    }
}
