using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApplication.Migrations
{
    /// <inheritdoc />
    public partial class Test6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReminder",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReminder",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "Notes");
        }
    }
}
