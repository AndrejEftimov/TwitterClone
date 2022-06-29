using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Twitter.Migrations
{
    public partial class AddHeartsToReply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Hearts",
                table: "Hearts");

            migrationBuilder.AddColumn<int>(
                name: "HeartCount",
                table: "Replies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Hearts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ReplyId",
                table: "Hearts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hearts",
                table: "Hearts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Hearts_PostId",
                table: "Hearts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Hearts_ReplyId",
                table: "Hearts",
                column: "ReplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hearts_Replies_ReplyId",
                table: "Hearts",
                column: "ReplyId",
                principalTable: "Replies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hearts_Replies_ReplyId",
                table: "Hearts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hearts",
                table: "Hearts");

            migrationBuilder.DropIndex(
                name: "IX_Hearts_PostId",
                table: "Hearts");

            migrationBuilder.DropIndex(
                name: "IX_Hearts_ReplyId",
                table: "Hearts");

            migrationBuilder.DropColumn(
                name: "HeartCount",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Hearts");

            migrationBuilder.DropColumn(
                name: "ReplyId",
                table: "Hearts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hearts",
                table: "Hearts",
                columns: new[] { "PostId", "UserId" });
        }
    }
}
