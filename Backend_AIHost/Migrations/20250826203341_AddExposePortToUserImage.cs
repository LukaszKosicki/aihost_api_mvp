using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_AIHost.Migrations
{
    /// <inheritdoc />
    public partial class AddExposePortToUserImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExposePort",
                table: "UserImages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExposePort",
                table: "UserImages");
        }
    }
}
