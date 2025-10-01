using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nash_Manassas.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_ImageFile_ImageFileId",
                table: "Project");

            migrationBuilder.DropTable(
                name: "ImageFile");

            migrationBuilder.DropIndex(
                name: "IX_Project_ImageFileId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ImageFileId",
                table: "Project");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageFileId",
                table: "Project",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImageFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    UploadedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageFile", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Project_ImageFileId",
                table: "Project",
                column: "ImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ImageFile_ImageFileId",
                table: "Project",
                column: "ImageFileId",
                principalTable: "ImageFile",
                principalColumn: "Id");
        }
    }
}
