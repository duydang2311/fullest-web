using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _005_AddUserSession_Role_Project : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(name: "ProjectHiLoSequence", incrementBy: 10);

            migrationBuilder.CreateSequence(name: "ProjectMemberHiLoSequence", incrementBy: 10);

            migrationBuilder.CreateSequence(name: "RoleHiLoSequence", incrementBy: 10);

            migrationBuilder.CreateSequence(name: "UserSessionHiLoSequence", incrementBy: 10);

            migrationBuilder.AddColumn<Instant>(
                name: "created_time",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()"
            );

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    created_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "now()"
                    ),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    identifier = table.Column<string>(
                        type: "character varying(32)",
                        maxLength: 32,
                        nullable: false
                    ),
                    deleted_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_projects_users_creator_id",
                        column: x => x.creator_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    created_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "now()"
                    ),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    token = table.Column<byte[]>(type: "bytea", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "project_members",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    created_time = table.Column<Instant>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "now()"
                    ),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_members_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_project_members_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_project_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    permission = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permission", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_permission_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_project_members_project_id_user_id",
                table: "project_members",
                columns: new[] { "project_id", "user_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_project_members_role_id",
                table: "project_members",
                column: "role_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_project_members_user_id",
                table: "project_members",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_projects_creator_id_identifier",
                table: "projects",
                columns: new[] { "creator_id", "identifier" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_projects_deleted_time",
                table: "projects",
                column: "deleted_time"
            );

            migrationBuilder.CreateIndex(
                name: "ix_role_permission_role_id_permission",
                table: "role_permission",
                columns: new[] { "role_id", "permission" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_user_id_token",
                table: "user_sessions",
                columns: new[] { "user_id", "token" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "project_members");

            migrationBuilder.DropTable(name: "role_permission");

            migrationBuilder.DropTable(name: "user_sessions");

            migrationBuilder.DropTable(name: "projects");

            migrationBuilder.DropTable(name: "role");

            migrationBuilder.DropColumn(name: "created_time", table: "users");

            migrationBuilder.DropSequence(name: "ProjectHiLoSequence");

            migrationBuilder.DropSequence(name: "ProjectMemberHiLoSequence");

            migrationBuilder.DropSequence(name: "RoleHiLoSequence");

            migrationBuilder.DropSequence(name: "UserSessionHiLoSequence");
        }
    }
}
