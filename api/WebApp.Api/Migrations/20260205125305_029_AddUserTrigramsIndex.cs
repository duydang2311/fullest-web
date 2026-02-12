using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _029_AddUserTrigramsIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("create extension pg_trgm");

            migrationBuilder.RenameIndex(
                name: "ix_users_name",
                table: "users",
                newName: "ix_users_name_unique"
            );

            migrationBuilder
                .CreateIndex(
                    name: "ix_users_display_name_trgm",
                    table: "users",
                    column: "display_name"
                )
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder
                .CreateIndex(name: "ix_users_name_trgm", table: "users", column: "name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_users_display_name_trgm", table: "users");

            migrationBuilder.DropIndex(name: "ix_users_name_trgm", table: "users");

            migrationBuilder.RenameIndex(
                name: "ix_users_name_unique",
                table: "users",
                newName: "ix_users_name"
            );

            migrationBuilder.Sql("drop extension pg_trgm");
        }
    }
}
