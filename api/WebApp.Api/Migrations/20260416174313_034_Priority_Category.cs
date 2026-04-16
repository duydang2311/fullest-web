using Microsoft.EntityFrameworkCore.Migrations;
using WebApp.Domain.Entities;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _034_Priority_Category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AlterDatabase()
                .Annotation("Npgsql:Enum:namespace_kind", "none,user")
                .Annotation("Npgsql:Enum:priority_category", "high,low,medium,urgent")
                .OldAnnotation("Npgsql:Enum:namespace_kind", "none,user");

            migrationBuilder.Sql(
                """
                ALTER TYPE status_category RENAME TO status_category_old;
                CREATE TYPE status_category AS ENUM ('proposed', 'ready', 'active', 'paused', 'review', 'completed', 'canceled', 'archived');
                ALTER TABLE statuses
                    ALTER COLUMN category TYPE status_category
                    USING (
                        CASE category::text
                            WHEN 'none' THEN 'proposed'
                            WHEN 'pending' THEN 'ready'
                            WHEN 'duplicated' THEN 'canceled'
                            ELSE category::text
                        END
                    )::status_category;
                DROP TYPE status_category_old;
                """
            );

            migrationBuilder.AddColumn<PriorityCategory>(
                name: "category",
                table: "priorities",
                type: "priority_category",
                nullable: false,
                defaultValue: PriorityCategory.Low
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "category", table: "priorities");

            migrationBuilder.Sql(
                """
                ALTER TYPE status_category RENAME TO status_category_old;
                CREATE TYPE status_category AS ENUM ('active', 'canceled', 'completed', 'none', 'pending');
                ALTER TABLE statuses
                    ALTER COLUMN category TYPE status_category
                    USING (
                        CASE category::text
                            WHEN 'proposed' THEN 'none'
                            WHEN 'ready' THEN 'pending'
                            ELSE category::text
                        END
                    )::status_category;
                DROP TYPE status_category_old;
                """
            );

            migrationBuilder
                .AlterDatabase()
                .Annotation("Npgsql:Enum:namespace_kind", "none,user")
                .OldAnnotation("Npgsql:Enum:namespace_kind", "none,user")
                .OldAnnotation("Npgsql:Enum:priority_category", "high,low,medium,urgent");
        }
    }
}
