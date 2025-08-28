using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _006_UpdateUserAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "fk_user_auth_users_user_id", table: "user_auth");

            migrationBuilder.DropPrimaryKey(name: "pk_user_auth", table: "user_auth");

            migrationBuilder.RenameTable(name: "user_auth", newName: "user_auths");

            migrationBuilder.RenameIndex(
                name: "ix_user_auth_user_id_provider",
                table: "user_auths",
                newName: "ix_user_auths_user_id_provider"
            );

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_auths",
                table: "user_auths",
                column: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_user_auths_users_user_id",
                table: "user_auths",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_auths_users_user_id",
                table: "user_auths"
            );

            migrationBuilder.DropPrimaryKey(name: "pk_user_auths", table: "user_auths");

            migrationBuilder.RenameTable(name: "user_auths", newName: "user_auth");

            migrationBuilder.RenameIndex(
                name: "ix_user_auths_user_id_provider",
                table: "user_auth",
                newName: "ix_user_auth_user_id_provider"
            );

            migrationBuilder.AddPrimaryKey(name: "pk_user_auth", table: "user_auth", column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_auth_users_user_id",
                table: "user_auth",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
