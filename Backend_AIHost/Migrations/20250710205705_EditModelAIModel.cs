using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_AIHost.Migrations
{
    /// <inheritdoc />
    public partial class EditModelAIModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseImage",
                table: "AIModels");

            migrationBuilder.DropColumn(
                name: "DownloadCommand",
                table: "AIModels");

            migrationBuilder.DropColumn(
                name: "StartCommand",
                table: "AIModels");

            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "AIModels",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "AIModels",
                newName: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "BaseImage",
                table: "AIModels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DownloadCommand",
                table: "AIModels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartCommand",
                table: "AIModels",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
