using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class _004_SeparateHiLo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(name: "EntityFrameworkHiLoSequence");

            migrationBuilder.CreateSequence(name: "UserAuthHiLoSequence", incrementBy: 10);

            migrationBuilder.CreateSequence(name: "UserHiLoSequence", incrementBy: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(name: "UserAuthHiLoSequence");

            migrationBuilder.DropSequence(name: "UserHiLoSequence");

            migrationBuilder.CreateSequence(name: "EntityFrameworkHiLoSequence", incrementBy: 10);
        }
    }
}
