using Microsoft.EntityFrameworkCore.Migrations;

namespace Lueben.Templates.Microservice.Data.Sql.Migrations
{
    public partial class ChangeApplicationIdColumnType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__Application",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Application");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Application",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Application",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Application",
                table: "Application",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__Application",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Application");

            migrationBuilder.AddColumn<long>(
                name: "ApplicationId",
                table: "Application",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Application",
                table: "Application",
                column: "ApplicationId");
        }
    }
}
