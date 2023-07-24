using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    public partial class AddForeignKeyToComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Tweet_TweetId",
                table: "Comment");

            migrationBuilder.AlterColumn<string>(
                name: "TweetId",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Tweet_TweetId",
                table: "Comment",
                column: "TweetId",
                principalTable: "Tweet",
                principalColumn: "TweetId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Tweet_TweetId",
                table: "Comment");

            migrationBuilder.AlterColumn<string>(
                name: "TweetId",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Tweet_TweetId",
                table: "Comment",
                column: "TweetId",
                principalTable: "Tweet",
                principalColumn: "TweetId");
        }
    }
}
