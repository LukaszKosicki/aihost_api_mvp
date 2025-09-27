using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_AIHost.Migrations
{
    /// <inheritdoc />
    public partial class AddContainerIdInUserContainer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContainerId",
                table: "UserContainers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContainerId",
                table: "UserContainers");
        }
    }
}
