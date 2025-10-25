using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _025_AddUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    table.CheckConstraint(
                        "chk_user_profiles_image_key",
                        "(image_key is null and image_version is null) or (image_key is not null and image_version is not null)"
                    );
                    table.CheckConstraint(
                        "chk_user_profiles_image_version",
                        "(image_key is null and image_version is null) or (image_key is not null and image_version is not null)"
                    );
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "user_profiles");
        }
    }
}
