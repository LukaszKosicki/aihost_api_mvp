using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_AIHost.Migrations
{
    /// <inheritdoc />
    public partial class AddImageIdandModelNametoUserImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "UserImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModelName",
                table: "UserImages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "UserImages");

            migrationBuilder.DropColumn(
                name: "ModelName",
                table: "UserImages");
        }
    }
}
