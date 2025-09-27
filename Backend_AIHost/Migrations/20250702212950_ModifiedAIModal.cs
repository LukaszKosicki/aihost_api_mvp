using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_AIHost.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAIModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AIModels");

            migrationBuilder.RenameColumn(
                name: "DockerImage",
                table: "AIModels",
                newName: "ModelInternalName");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AIModels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SupportsGPU",
                table: "AIModels",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AIModels");

            migrationBuilder.DropColumn(
                name: "SupportsGPU",
                table: "AIModels");

            migrationBuilder.RenameColumn(
                name: "ModelInternalName",
                table: "AIModels",
                newName: "DockerImage");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AIModels",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
