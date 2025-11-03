using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllulExpressAPI.Migrations
{
    /// <inheritdoc />
    public partial class tokentable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Phonenum1",
                keyValue: null,
                column: "Phonenum1",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Phonenum1",
                table: "Employees",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Phonenum1",
                keyValue: null,
                column: "Phonenum1",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Phonenum1",
                table: "Drivers",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Phonenum1",
                keyValue: null,
                column: "Phonenum1",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Phonenum1",
                table: "Clients",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ValidTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidTokens", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Phonenum1",
                table: "Employees",
                column: "Phonenum1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_Phonenum1",
                table: "Drivers",
                column: "Phonenum1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Phonenum1",
                table: "Clients",
                column: "Phonenum1",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValidTokens");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Phonenum1",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_Phonenum1",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Phonenum1",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "Phonenum1",
                table: "Employees",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Phonenum1",
                table: "Drivers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Phonenum1",
                table: "Clients",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
