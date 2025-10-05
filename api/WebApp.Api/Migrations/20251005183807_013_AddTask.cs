using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _013_AddTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(name: "TaskHiLoSequence", incrementBy: 10);

            migrationBuilder.CreateSequence(name: "TaskPublicIdHiLoSequence", incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    created_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    author_id = table.Column<long>(type: "bigint", nullable: false),
                    public_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    deleted_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tasks", x => x.id);
                    table.ForeignKey(
                        name: "fk_tasks_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_tasks_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "task_assignees",
                columns: table => new
                {
                    task_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_task_assignees", x => new { x.task_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_task_assignees_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_task_assignees_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_task_assignees_user_id",
                table: "task_assignees",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_tasks_author_id",
                table: "tasks",
                column: "author_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_tasks_project_id",
                table: "tasks",
                column: "project_id"
            );

            migrationBuilder.Sql(
                """
                create function update_task_updated_time()
                returns trigger as $$
                begin
                    new.updated_time := now();
                    return new;
                end $$ language plpgsql;

                create function update_task_updated_time_on_assignee_change()
                returns trigger as $$
                begin
                    update tasks set updated_time = now() where id = new.task_id;
                    return null;
                end $$ language plpgsql;

                create trigger tr_tasks_updated_time
                before update on tasks
                for each row execute function update_task_updated_time();

                create trigger tr_task_assignees_tasks_updated_time
                after insert or delete on task_assignees
                for each row execute function update_task_updated_time_on_assignee_change();
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                drop trigger tr_tasks_updated_time on tasks;
                drop trigger tr_task_assignees_tasks_updated_time on task_assignees;
                drop function update_task_updated_time();
                drop function update_task_updated_time_on_assignee_change();
                """
            );
            migrationBuilder.DropTable(name: "task_assignees");

            migrationBuilder.DropTable(name: "tasks");

            migrationBuilder.DropSequence(name: "TaskHiLoSequence");

            migrationBuilder.DropSequence(name: "TaskPublicIdHiLoSequence");
        }
    }
}
