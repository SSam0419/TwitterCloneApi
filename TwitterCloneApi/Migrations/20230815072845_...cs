using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    /// <inheritdoc />
    public partial class _ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TweetUser");

            migrationBuilder.CreateTable(
                name: "TweetLikes",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TweetId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TweetLikes", x => new { x.UserId, x.TweetId });
                    table.ForeignKey(
                        name: "FK_TweetLikes_Tweet_TweetId",
                        column: x => x.TweetId,
                        principalTable: "Tweet",
                        principalColumn: "TweetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TweetLikes_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TweetLikes_TweetId",
                table: "TweetLikes",
                column: "TweetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TweetLikes");

            migrationBuilder.CreateTable(
                name: "TweetUser",
                columns: table => new
                {
                    LikesId = table.Column<string>(type: "text", nullable: false),
                    TweetId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TweetUser", x => new { x.LikesId, x.TweetId });
                    table.ForeignKey(
                        name: "FK_TweetUser_Tweet_TweetId",
                        column: x => x.TweetId,
                        principalTable: "Tweet",
                        principalColumn: "TweetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TweetUser_User_LikesId",
                        column: x => x.LikesId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TweetUser_TweetId",
                table: "TweetUser",
                column: "TweetId");
        }
    }
}
