using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _016_AddProjectTaskCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(name: "TaskPublicIdHiLoSequence");

            migrationBuilder
                .AlterColumn<long>(
                    name: "public_id",
                    table: "tasks",
                    type: "bigint",
                    nullable: false,
                    oldClrType: typeof(long),
                    oldType: "bigint"
                )
                .Annotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.CreateTable(
                name: "project_task_counters",
                columns: table => new
                {
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    count = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_task_counters", x => x.project_id);
                }
            );

            migrationBuilder.Sql(
                """
                create function create_project_task_counter()
                returns trigger as $$
                begin
                    insert into project_task_counters (project_id, count)
                    values (new.id, 0);
                    return null;
                end
                $$ language plpgsql;

                create trigger tr_projects_create_project_task_counter after insert on projects
                for each row execute function create_project_task_counter();

                create function set_task_public_id()
                returns trigger as $$
                begin
                    update project_task_counters
                    set count = count + 1
                    where project_id = new.project_id
                    returning count
                    into new.public_id;
                    return new;
                end
                $$ language plpgsql;

                create trigger tr_tasks_set_public_id before insert on tasks
                for each row execute function set_task_public_id();
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                drop trigger if exists tr_tasks_set_public_id on tasks;
                drop function if exists set_task_public_id();

                drop trigger if exists tr_projects_create_project_task_counter on projects;
                drop function if exists create_project_task_counter();
                """
            );
            migrationBuilder.DropTable(name: "project_task_counters");

            migrationBuilder.CreateSequence(name: "TaskPublicIdHiLoSequence", incrementBy: 10);

            migrationBuilder
                .AlterColumn<long>(
                    name: "public_id",
                    table: "tasks",
                    type: "bigint",
                    nullable: false,
                    oldClrType: typeof(long),
                    oldType: "bigint"
                )
                .OldAnnotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );
        }
    }
}
