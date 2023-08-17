using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    /// <inheritdoc />
    public partial class addcommentlikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentUser");

            migrationBuilder.CreateTable(
                name: "CommentLikes",
                columns: table => new
                {
                    CommentId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentLikes", x => new { x.UserId, x.CommentId });
                    table.ForeignKey(
                        name: "FK_CommentLikes_Comment_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentLikes_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentLikes_CommentId",
                table: "CommentLikes",
                column: "CommentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentLikes");

            migrationBuilder.CreateTable(
                name: "CommentUser",
                columns: table => new
                {
                    CommentId = table.Column<string>(type: "text", nullable: false),
                    LikesId = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_CommentUser_LikesId",
                table: "CommentUser",
                column: "LikesId");
        }
    }
}
