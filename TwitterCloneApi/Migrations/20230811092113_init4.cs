using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweet_User_AuthorId",
                table: "Tweet");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Comment_CommentId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_User_UserId",
                table: "User");

            migrationBuilder.DropTable(
                name: "TweetUserLike");

            migrationBuilder.DropIndex(
                name: "IX_User_CommentId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_UserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Comment",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "CommentUser",
                columns: table => new
                {
                    CommentId = table.Column<string>(type: "text", nullable: false),
                    LikesId = table.Column<string>(type: "character varying(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentUser", x => new { x.CommentId, x.LikesId });
                    table.ForeignKey(
                        name: "FK_CommentUser_Comment_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentUser_User_LikesId",
                        column: x => x.LikesId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TweetUser",
                columns: table => new
                {
                    LikesId = table.Column<string>(type: "character varying(30)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "UserUser",
                columns: table => new
                {
                    MyFollowingUsersId = table.Column<string>(type: "character varying(30)", nullable: false),
                    UsersFollowedMeId = table.Column<string>(type: "character varying(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUser", x => new { x.MyFollowingUsersId, x.UsersFollowedMeId });
                    table.ForeignKey(
                        name: "FK_UserUser_User_MyFollowingUsersId",
                        column: x => x.MyFollowingUsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserUser_User_UsersFollowedMeId",
                        column: x => x.UsersFollowedMeId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AuthorId",
                table: "Comment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentUser_LikesId",
                table: "CommentUser",
                column: "LikesId");

            migrationBuilder.CreateIndex(
                name: "IX_TweetUser_TweetId",
                table: "TweetUser",
                column: "TweetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUser_UsersFollowedMeId",
                table: "UserUser",
                column: "UsersFollowedMeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_User_AuthorId",
                table: "Comment",
                column: "AuthorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tweet_User_AuthorId",
                table: "Tweet",
                column: "AuthorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_User_AuthorId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Tweet_User_AuthorId",
                table: "Tweet");

            migrationBuilder.DropTable(
                name: "CommentUser");

            migrationBuilder.DropTable(
                name: "TweetUser");

            migrationBuilder.DropTable(
                name: "UserUser");

            migrationBuilder.DropIndex(
                name: "IX_Comment_AuthorId",
                table: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "CommentId",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "User",
                type: "character varying(30)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Comment",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.CreateTable(
                name: "TweetUserLike",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(30)", nullable: false),
                    TweetId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TweetUserLike", x => new { x.UserId, x.TweetId });
                    table.ForeignKey(
                        name: "FK_TweetUserLike_Tweet_TweetId",
                        column: x => x.TweetId,
                        principalTable: "Tweet",
                        principalColumn: "TweetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TweetUserLike_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_CommentId",
                table: "User",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserId",
                table: "User",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TweetUserLike_TweetId",
                table: "TweetUserLike",
                column: "TweetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tweet_User_AuthorId",
                table: "Tweet",
                column: "AuthorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Comment_CommentId",
                table: "User",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_User_UserId",
                table: "User",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
