using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _018_RemoveSeparateProjectStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_projects_project_statuses_default_project_status_id",
                table: "projects"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_project_statuses_project_status_id",
                table: "tasks"
            );

            migrationBuilder.DropTable(name: "project_statuses");

            migrationBuilder.DropSequence(name: "ProjectStatusHiLoSequence");

            migrationBuilder.RenameColumn(
                name: "project_status_id",
                table: "tasks",
                newName: "status_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_tasks_project_status_id",
                table: "tasks",
                newName: "ix_tasks_status_id"
            );

            migrationBuilder.RenameColumn(
                name: "default_project_status_id",
                table: "projects",
                newName: "default_status_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_projects_default_project_status_id",
                table: "projects",
                newName: "ix_projects_default_status_id"
            );

            migrationBuilder.AddColumn<long>(
                name: "project_id",
                table: "statuses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.CreateIndex(
                name: "ix_statuses_project_id",
                table: "statuses",
                column: "project_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_projects_statuses_default_status_id",
                table: "projects",
                column: "default_status_id",
                principalTable: "statuses",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_statuses_projects_project_id",
                table: "statuses",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_statuses_status_id",
                table: "tasks",
                column: "status_id",
                principalTable: "statuses",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_projects_statuses_default_status_id",
                table: "projects"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_statuses_projects_project_id",
                table: "statuses"
            );

            migrationBuilder.DropForeignKey(name: "fk_tasks_statuses_status_id", table: "tasks");

            migrationBuilder.DropIndex(name: "ix_statuses_project_id", table: "statuses");

            migrationBuilder.DropColumn(name: "project_id", table: "statuses");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "tasks",
                newName: "project_status_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_tasks_status_id",
                table: "tasks",
                newName: "ix_tasks_project_status_id"
            );

            migrationBuilder.RenameColumn(
                name: "default_status_id",
                table: "projects",
                newName: "default_project_status_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_projects_default_status_id",
                table: "projects",
                newName: "ix_projects_default_project_status_id"
            );

            migrationBuilder.CreateSequence(name: "ProjectStatusHiLoSequence", incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "project_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    status_id = table.Column<long>(type: "bigint", nullable: false),
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
                        name: "fk_project_statuses_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
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

            migrationBuilder.AddForeignKey(
                name: "fk_projects_project_statuses_default_project_status_id",
                table: "projects",
                column: "default_project_status_id",
                principalTable: "project_statuses",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_project_statuses_project_status_id",
                table: "tasks",
                column: "project_status_id",
                principalTable: "project_statuses",
                principalColumn: "id"
            );
        }
    }
}
