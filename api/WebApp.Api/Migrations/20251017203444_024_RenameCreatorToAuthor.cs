using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _024_RenameCreatorToAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_users_creator_id",
                table: "comments"
            );

            migrationBuilder.RenameColumn(
                name: "creator_id",
                table: "comments",
                newName: "author_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_comments_creator_id",
                table: "comments",
                newName: "ix_comments_author_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_comments_users_author_id",
                table: "comments",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "fk_comments_users_author_id", table: "comments");

            migrationBuilder.RenameColumn(
                name: "author_id",
                table: "comments",
                newName: "creator_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_comments_author_id",
                table: "comments",
                newName: "ix_comments_creator_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_comments_users_creator_id",
                table: "comments",
                column: "creator_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
