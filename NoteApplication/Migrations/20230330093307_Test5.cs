using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApplication.Migrations
{
    /// <inheritdoc />
    public partial class Test5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReciverEmail",
                table: "Collaborators",
                newName: "ReceiverEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiverEmail",
                table: "Collaborators",
                newName: "ReciverEmail");
        }
    }
}
