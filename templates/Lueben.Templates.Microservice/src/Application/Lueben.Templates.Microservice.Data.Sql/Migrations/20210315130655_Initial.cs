using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lueben.Templates.Microservice.Data.Sql.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreferredDepotId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Comments = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    RejectionReason = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "getutcdate()"),
                    Updated = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Application", x => x.ApplicationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application");
        }
    }
}
