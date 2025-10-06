using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using WebApp.Domain.Entities;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _014_AddLabel_AddStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(name: "name", table: "tasks", newName: "title");

            migrationBuilder
                .AlterDatabase()
                .Annotation("Npgsql:Enum:namespace_kind", "none,user")
                .Annotation("Npgsql:Enum:status_category", "active,canceled,completed,none,pending")
                .OldAnnotation("Npgsql:Enum:namespace_kind", "none,user");

            migrationBuilder.CreateSequence(name: "LabelHiLoSequence", incrementBy: 10);

            migrationBuilder.CreateSequence(name: "ProjectStatusHiLoSequence", incrementBy: 10);

            migrationBuilder.CreateSequence(name: "StatusHiLoSequence", incrementBy: 10);

            migrationBuilder.AddColumn<Instant>(
                name: "due_time",
                table: "tasks",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "due_tz",
                table: "tasks",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "project_status_id",
                table: "tasks",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "labels",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    color = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labels", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<StatusCategory>(
                        type: "status_category",
                        nullable: false
                    ),
                    color = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_statuses", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "task_label",
                columns: table => new
                {
                    task_id = table.Column<long>(type: "bigint", nullable: false),
                    label_id = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_task_label", x => new { x.label_id, x.task_id });
                    table.ForeignKey(
                        name: "fk_task_label_labels_label_id",
                        column: x => x.label_id,
                        principalTable: "labels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_task_label_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "project_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    status_id = table.Column<long>(type: "bigint", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_statuses", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_statuses_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_project_statuses_status_status_id",
                        column: x => x.status_id,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_tasks_project_status_id",
                table: "tasks",
                column: "project_status_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_project_statuses_project_id",
                table: "project_statuses",
                column: "project_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_project_statuses_status_id",
                table: "project_statuses",
                column: "status_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_task_label_task_id",
                table: "task_label",
                column: "task_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_project_status_project_status_id",
                table: "tasks",
                column: "project_status_id",
                principalTable: "project_statuses",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tasks_project_status_project_status_id",
                table: "tasks"
            );

            migrationBuilder.DropTable(name: "project_statuses");

            migrationBuilder.DropTable(name: "task_label");

            migrationBuilder.DropTable(name: "statuses");

            migrationBuilder.DropTable(name: "labels");

            migrationBuilder.DropIndex(name: "ix_tasks_project_status_id", table: "tasks");

            migrationBuilder.DropColumn(name: "due_time", table: "tasks");

            migrationBuilder.DropColumn(name: "due_tz", table: "tasks");

            migrationBuilder.DropColumn(name: "project_status_id", table: "tasks");

            migrationBuilder.DropSequence(name: "LabelHiLoSequence");

            migrationBuilder.DropSequence(name: "ProjectStatusHiLoSequence");

            migrationBuilder.DropSequence(name: "StatusHiLoSequence");

            migrationBuilder.RenameColumn(name: "title", table: "tasks", newName: "name");

            migrationBuilder
                .AlterDatabase()
                .Annotation("Npgsql:Enum:namespace_kind", "none,user")
                .OldAnnotation("Npgsql:Enum:namespace_kind", "none,user")
                .OldAnnotation(
                    "Npgsql:Enum:status_category",
                    "active,canceled,completed,none,pending"
                );
        }
    }
}
