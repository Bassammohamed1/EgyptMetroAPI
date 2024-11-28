using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetroAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SharedWith",
                table: "Stations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isShared",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LineNo",
                table: "Lines",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SharedWith",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "isShared",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "LineNo",
                table: "Lines");
        }
    }
}
