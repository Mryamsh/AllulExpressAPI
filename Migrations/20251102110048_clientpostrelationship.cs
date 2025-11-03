using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllulExpressAPI.Migrations
{
    /// <inheritdoc />
    public partial class clientpostrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ClientId",
                table: "Posts",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Clients_ClientId",
                table: "Posts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Clients_ClientId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ClientId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Posts");
        }
    }
}
