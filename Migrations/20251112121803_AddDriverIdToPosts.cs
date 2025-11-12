using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllulExpressAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverIdToPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_DriverId",
                table: "Posts",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Drivers_DriverId",
                table: "Posts",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Drivers_DriverId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_DriverId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Posts");
        }
    }
}
