using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _021_AddPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "PriorityHiLoSequence",
                incrementBy: 10);

            migrationBuilder.AddColumn<long>(
                name: "default_priority_id",
                table: "projects",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "priorities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false),
                    rank = table.Column<string>(type: "text", nullable: false, collation: "C"),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_priorities", x => x.id);
                    table.ForeignKey(
                        name: "fk_priorities_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_projects_default_priority_id",
                table: "projects",
                column: "default_priority_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_priorities_project_id",
                table: "priorities",
                column: "project_id");

            migrationBuilder.AddForeignKey(
                name: "fk_projects_priorities_default_priority_id",
                table: "projects",
                column: "default_priority_id",
                principalTable: "priorities",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_projects_priorities_default_priority_id",
                table: "projects");

            migrationBuilder.DropTable(
                name: "priorities");

            migrationBuilder.DropIndex(
                name: "ix_projects_default_priority_id",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "default_priority_id",
                table: "projects");

            migrationBuilder.DropSequence(
                name: "PriorityHiLoSequence");
        }
    }
}
