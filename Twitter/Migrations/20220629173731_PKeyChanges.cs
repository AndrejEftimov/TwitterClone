using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Twitter.Migrations
{
    public partial class PKeyChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ListMember",
                table: "ListMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListFollower",
                table: "ListFollower");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Followings",
                table: "Followings");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ListMember",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ListFollower",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Followings",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListMember",
                table: "ListMember",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListFollower",
                table: "ListFollower",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Followings",
                table: "Followings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ListMember_ListId",
                table: "ListMember",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_ListFollower_ListId",
                table: "ListFollower",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_Followings_FollowedUserId",
                table: "Followings",
                column: "FollowedUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ListMember",
                table: "ListMember");

            migrationBuilder.DropIndex(
                name: "IX_ListMember_ListId",
                table: "ListMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListFollower",
                table: "ListFollower");

            migrationBuilder.DropIndex(
                name: "IX_ListFollower_ListId",
                table: "ListFollower");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Followings",
                table: "Followings");

            migrationBuilder.DropIndex(
                name: "IX_Followings_FollowedUserId",
                table: "Followings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ListMember");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ListFollower");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Followings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListMember",
                table: "ListMember",
                columns: new[] { "ListId", "MemberId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListFollower",
                table: "ListFollower",
                columns: new[] { "ListId", "FollowerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Followings",
                table: "Followings",
                columns: new[] { "FollowedUserId", "FollowerId" });
        }
    }
}
