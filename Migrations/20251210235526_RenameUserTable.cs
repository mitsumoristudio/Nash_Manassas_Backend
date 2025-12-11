using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nash_Manassas.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_User_UserId",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_User_UserId",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users2",
                table: "Users2",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Users2_UserId",
                table: "Equipment",
                column: "UserId",
                principalTable: "Users2",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Users2_UserId",
                table: "Project",
                column: "UserId",
                principalTable: "Users2",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Users2_UserId",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Users2_UserId",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users2",
                table: "Users2");

            migrationBuilder.RenameTable(
                name: "Users2",
                newName: "User");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_User_UserId",
                table: "Equipment",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_User_UserId",
                table: "Project",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
