using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_AIHost.Migrations
{
    /// <inheritdoc />
    public partial class AddNoNameMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserContainers_VPSId",
                table: "UserContainers",
                column: "VPSId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserContainers_VPSes_VPSId",
                table: "UserContainers",
                column: "VPSId",
                principalTable: "VPSes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserContainers_VPSes_VPSId",
                table: "UserContainers");

            migrationBuilder.DropIndex(
                name: "IX_UserContainers_VPSId",
                table: "UserContainers");
        }
    }
}
