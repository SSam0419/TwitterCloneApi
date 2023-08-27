using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    /// <inheritdoc />
    public partial class addReTweet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.CreateTable(
                name: "ReTweet",
                columns: table => new
                {
                    ReTweetedBy = table.Column<string>(type: "text", nullable: false),
                    TweetId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReTweet", x => new { x.TweetId, x.ReTweetedBy });
                    table.ForeignKey(
                        name: "FK_ReTweet_Tweet_TweetId",
                        column: x => x.TweetId,
                        principalTable: "Tweet",
                        principalColumn: "TweetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReTweet_User_ReTweetedBy",
                        column: x => x.ReTweetedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                }); 

            migrationBuilder.CreateIndex(
                name: "IX_ReTweet_ReTweetedBy",
                table: "ReTweet",
                column: "ReTweetedBy"); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentLikes");

            migrationBuilder.DropTable(
                name: "ReTweet");

            migrationBuilder.DropTable(
                name: "TweetBookmarks");

            migrationBuilder.DropTable(
                name: "TweetLikes");

            migrationBuilder.DropTable(
                name: "UserConfidentials");

            migrationBuilder.DropTable(
                name: "UserFollowings");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Tweet");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
