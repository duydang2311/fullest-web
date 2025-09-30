using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _007_permissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_members_role_role_id",
                table: "project_members"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_role_permission_role_role_id",
                table: "role_permission"
            );

            migrationBuilder.DropPrimaryKey(name: "pk_role_permission", table: "role_permission");

            migrationBuilder.DropIndex(
                name: "ix_role_permission_role_id_permission",
                table: "role_permission"
            );

            migrationBuilder.DropPrimaryKey(name: "pk_role", table: "role");

            migrationBuilder.DropColumn(name: "permission", table: "role_permission");

            migrationBuilder.RenameTable(name: "role_permission", newName: "role_permissions");

            migrationBuilder.RenameTable(name: "role", newName: "roles");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "role_permissions",
                newName: "permission_id"
            );

            migrationBuilder.CreateSequence(name: "PermissionHiLoSequence", incrementBy: 10);

            migrationBuilder
                .AlterColumn<long>(
                    name: "permission_id",
                    table: "role_permissions",
                    type: "bigint",
                    nullable: false,
                    oldClrType: typeof(long),
                    oldType: "bigint"
                )
                .OldAnnotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.AddColumn<int>(
                name: "rank",
                table: "roles",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddPrimaryKey(
                name: "pk_role_permissions",
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id" }
            );

            migrationBuilder.AddPrimaryKey(name: "pk_roles", table: "roles", column: "id");

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_id",
                table: "role_permissions",
                column: "role_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_permissions_name",
                table: "permissions",
                column: "name",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_project_members_roles_role_id",
                table: "project_members",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_role_permissions_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id",
                principalTable: "permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_role_permissions_roles_role_id",
                table: "role_permissions",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_members_roles_role_id",
                table: "project_members"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_role_permissions_permissions_permission_id",
                table: "role_permissions"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_role_permissions_roles_role_id",
                table: "role_permissions"
            );

            migrationBuilder.DropTable(name: "permissions");

            migrationBuilder.DropPrimaryKey(name: "pk_roles", table: "roles");

            migrationBuilder.DropPrimaryKey(name: "pk_role_permissions", table: "role_permissions");

            migrationBuilder.DropIndex(
                name: "ix_role_permissions_role_id",
                table: "role_permissions"
            );

            migrationBuilder.DropColumn(name: "rank", table: "roles");

            migrationBuilder.DropSequence(name: "PermissionHiLoSequence");

            migrationBuilder.RenameTable(name: "roles", newName: "role");

            migrationBuilder.RenameTable(name: "role_permissions", newName: "role_permission");

            migrationBuilder.RenameColumn(
                name: "permission_id",
                table: "role_permission",
                newName: "id"
            );

            migrationBuilder
                .AlterColumn<long>(
                    name: "id",
                    table: "role_permission",
                    type: "bigint",
                    nullable: false,
                    oldClrType: typeof(long),
                    oldType: "bigint"
                )
                .Annotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.AddColumn<string>(
                name: "permission",
                table: "role_permission",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddPrimaryKey(name: "pk_role", table: "role", column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_role_permission",
                table: "role_permission",
                column: "id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_role_permission_role_id_permission",
                table: "role_permission",
                columns: new[] { "role_id", "permission" },
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_project_members_role_role_id",
                table: "project_members",
                column: "role_id",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_role_permission_role_role_id",
                table: "role_permission",
                column: "role_id",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
