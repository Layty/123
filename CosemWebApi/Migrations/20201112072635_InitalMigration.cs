using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CosemWebApi.Migrations
{
    public partial class InitalMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CosemObjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ClassId = table.Column<int>(type: "INTEGER", nullable: false),
                    Obis = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosemObjects", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CosemObjects",
                columns: new[] { "Id", "ClassId", "Name", "Obis" },
                values: new object[] { new Guid("bbdee09c-089b-4d30-bece-44df5923716c"), 1, "Local Time", "1.0.0.9.1.255" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CosemObjects");
        }
    }
}
