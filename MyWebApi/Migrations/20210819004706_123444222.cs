using Microsoft.EntityFrameworkCore.Migrations;

namespace MyWebApi.Migrations
{
    public partial class _123444222 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Energies_Meters_MeterId1",
                table: "Energies");

            migrationBuilder.DropForeignKey(
                name: "FK_Powers_Meters_MeterId1",
                table: "Powers");

            migrationBuilder.DropIndex(
                name: "IX_Powers_MeterId1",
                table: "Powers");

            migrationBuilder.DropIndex(
                name: "IX_Energies_MeterId1",
                table: "Energies");

            migrationBuilder.DropColumn(
                name: "MeterId1",
                table: "Powers");

            migrationBuilder.DropColumn(
                name: "MeterId1",
                table: "Energies");

            migrationBuilder.AlterColumn<string>(
                name: "MeterId",
                table: "Powers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "MeterId",
                table: "Energies",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Powers_MeterId",
                table: "Powers",
                column: "MeterId");

            migrationBuilder.CreateIndex(
                name: "IX_Energies_MeterId",
                table: "Energies",
                column: "MeterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Energies_Meters_MeterId",
                table: "Energies",
                column: "MeterId",
                principalTable: "Meters",
                principalColumn: "MeterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Powers_Meters_MeterId",
                table: "Powers",
                column: "MeterId",
                principalTable: "Meters",
                principalColumn: "MeterId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Energies_Meters_MeterId",
                table: "Energies");

            migrationBuilder.DropForeignKey(
                name: "FK_Powers_Meters_MeterId",
                table: "Powers");

            migrationBuilder.DropIndex(
                name: "IX_Powers_MeterId",
                table: "Powers");

            migrationBuilder.DropIndex(
                name: "IX_Energies_MeterId",
                table: "Energies");

            migrationBuilder.AlterColumn<int>(
                name: "MeterId",
                table: "Powers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeterId1",
                table: "Powers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MeterId",
                table: "Energies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeterId1",
                table: "Energies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Powers_MeterId1",
                table: "Powers",
                column: "MeterId1");

            migrationBuilder.CreateIndex(
                name: "IX_Energies_MeterId1",
                table: "Energies",
                column: "MeterId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Energies_Meters_MeterId1",
                table: "Energies",
                column: "MeterId1",
                principalTable: "Meters",
                principalColumn: "MeterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Powers_Meters_MeterId1",
                table: "Powers",
                column: "MeterId1",
                principalTable: "Meters",
                principalColumn: "MeterId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
