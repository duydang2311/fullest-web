using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _027_AddActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(name: "ActivityHiLoSequence", incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "activities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    created_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "now()"
                    ),
                    task_id = table.Column<long>(type: "bigint", nullable: true),
                    actor_id = table.Column<long>(type: "bigint", nullable: true),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    data = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_activities", x => x.id);
                    table.ForeignKey(
                        name: "fk_activities_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id"
                    );
                    table.ForeignKey(
                        name: "fk_activities_users_actor_id",
                        column: x => x.actor_id,
                        principalTable: "users",
                        principalColumn: "id"
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_activities_actor_id",
                table: "activities",
                column: "actor_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_activities_task_id",
                table: "activities",
                column: "task_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "activities");

            migrationBuilder.DropSequence(name: "ActivityHiLoSequence");
        }
    }
}
