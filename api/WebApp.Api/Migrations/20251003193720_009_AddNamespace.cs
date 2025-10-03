using Microsoft.EntityFrameworkCore.Migrations;
using WebApp.Domain.Entities;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _009_AddNamespace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_projects_users_creator_id",
                table: "projects"
            );

            migrationBuilder.RenameColumn(
                name: "creator_id",
                table: "projects",
                newName: "namespace_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_projects_creator_id_identifier",
                table: "projects",
                newName: "ix_projects_namespace_id_identifier"
            );

            migrationBuilder.AlterDatabase().Annotation("Npgsql:Enum:namespace_kind", "none,user");

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "projects",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "namespaces",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    kind = table.Column<NamespaceKind>(type: "namespace_kind", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_namespaces", x => x.id);
                    table.ForeignKey(
                        name: "fk_namespaces_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id"
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_projects_user_id",
                table: "projects",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_namespaces_user_id",
                table: "namespaces",
                column: "user_id",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_projects_namespaces_namespace_id",
                table: "projects",
                column: "namespace_id",
                principalTable: "namespaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_projects_users_user_id",
                table: "projects",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_projects_namespaces_namespace_id",
                table: "projects"
            );

            migrationBuilder.DropForeignKey(name: "fk_projects_users_user_id", table: "projects");

            migrationBuilder.DropTable(name: "namespaces");

            migrationBuilder.DropIndex(name: "ix_projects_user_id", table: "projects");

            migrationBuilder.DropColumn(name: "user_id", table: "projects");

            migrationBuilder.RenameColumn(
                name: "namespace_id",
                table: "projects",
                newName: "creator_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_projects_namespace_id_identifier",
                table: "projects",
                newName: "ix_projects_creator_id_identifier"
            );

            migrationBuilder
                .AlterDatabase()
                .OldAnnotation("Npgsql:Enum:namespace_kind", "none,user");

            migrationBuilder.AddForeignKey(
                name: "fk_projects_users_creator_id",
                table: "projects",
                column: "creator_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
