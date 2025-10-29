using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _026_MergeUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "user_profiles");

            migrationBuilder.AddColumn<string>(
                name: "display_name",
                table: "users",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "image_key",
                table: "users",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "image_version",
                table: "users",
                type: "text",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "display_name", table: "users");

            migrationBuilder.DropColumn(name: "image_key", table: "users");

            migrationBuilder.DropColumn(name: "image_version", table: "users");

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    created_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "now()"
                    ),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    image_key = table.Column<string>(type: "text", nullable: true),
                    image_version = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_profiles", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );
        }
    }
}
