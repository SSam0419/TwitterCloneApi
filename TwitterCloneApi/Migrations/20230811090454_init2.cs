using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweet_User_TweetId",
                table: "Tweet");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Tweet_TweetId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_TweetId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TweetId",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Tweet",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TweetId",
                table: "Tweet",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "TweetId",
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
                name: "IX_Tweet_AuthorId",
                table: "Tweet",
                column: "AuthorId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweet_User_AuthorId",
                table: "Tweet");

            migrationBuilder.DropTable(
                name: "TweetUserLike");

            migrationBuilder.DropIndex(
                name: "IX_Tweet_AuthorId",
                table: "Tweet");

            migrationBuilder.AddColumn<string>(
                name: "TweetId",
                table: "User",
                type: "character varying(30)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Tweet",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)");

            migrationBuilder.AlterColumn<string>(
                name: "TweetId",
                table: "Tweet",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TweetId",
                table: "Comment",
                type: "character varying(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_User_TweetId",
                table: "User",
                column: "TweetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tweet_User_TweetId",
                table: "Tweet",
                column: "TweetId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Tweet_TweetId",
                table: "User",
                column: "TweetId",
                principalTable: "Tweet",
                principalColumn: "TweetId");
        }
    }
}
