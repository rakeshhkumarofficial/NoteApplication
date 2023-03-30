using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteApplication.Migrations
{
    /// <inheritdoc />
    public partial class Test4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collaborators_Notes_NoteId",
                table: "Collaborators");

            migrationBuilder.DropIndex(
                name: "IX_Collaborators_NoteId",
                table: "Collaborators");

            migrationBuilder.AlterColumn<Guid>(
                name: "NoteId",
                table: "Collaborators",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "NoteId",
                table: "Collaborators",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Collaborators_NoteId",
                table: "Collaborators",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collaborators_Notes_NoteId",
                table: "Collaborators",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "NoteId");
        }
    }
}
