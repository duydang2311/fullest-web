using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _002_HiLoSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(name: "EntityFrameworkHiLoSequence", incrementBy: 10);

            migrationBuilder
                .AlterColumn<long>(
                    name: "id",
                    table: "users",
                    type: "bigint",
                    nullable: false,
                    oldClrType: typeof(long),
                    oldType: "bigint"
                )
                .OldAnnotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.Sql(
                """
                ALTER TABLE user_credentials ALTER COLUMN hash TYPE bytea USING hash::bytea;
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(name: "EntityFrameworkHiLoSequence");

            migrationBuilder
                .AlterColumn<long>(
                    name: "id",
                    table: "users",
                    type: "bigint",
                    nullable: false,
                    oldClrType: typeof(long),
                    oldType: "bigint"
                )
                .Annotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.Sql(
                """
                ALTER TABLE user_credentials ALTER COLUMN hash TYPE text USING encode(hash, 'escape');
                """
            );
        }
    }
}
