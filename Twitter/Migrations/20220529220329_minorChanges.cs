using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Twitter.Migrations
{
    public partial class minorChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "Users",
                newName: "ProfileImage");

            migrationBuilder.RenameColumn(
                name: "CoverImageUrl",
                table: "Users",
                newName: "CoverImage");

            migrationBuilder.RenameColumn(
                name: "AttachmentUrl",
                table: "Posts",
                newName: "Attachment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImage",
                table: "Users",
                newName: "ProfileImageUrl");

            migrationBuilder.RenameColumn(
                name: "CoverImage",
                table: "Users",
                newName: "CoverImageUrl");

            migrationBuilder.RenameColumn(
                name: "Attachment",
                table: "Posts",
                newName: "AttachmentUrl");
        }
    }
}
