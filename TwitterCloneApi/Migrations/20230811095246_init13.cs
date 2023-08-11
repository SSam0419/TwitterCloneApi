using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    /// <inheritdoc />
    public partial class init13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_User__UserId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User__UserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "_UserId",
                table: "User");

            migrationBuilder.CreateTable(
                name: "UserFollowed",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(30)", nullable: false),
                    FollowedUserId = table.Column<string>(type: "character varying(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowed", x => new { x.UserId, x.FollowedUserId });
                    table.ForeignKey(
                        name: "FK_UserFollowed_User_FollowedUserId",
                        column: x => x.FollowedUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollowed_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFollowings",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(30)", nullable: false),
                    FollowingUserId = table.Column<string>(type: "character varying(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowings", x => new { x.UserId, x.FollowingUserId });
                    table.ForeignKey(
                        name: "FK_UserFollowings_User_FollowingUserId",
                        column: x => x.FollowingUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollowings_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowed_FollowedUserId",
                table: "UserFollowed",
                column: "FollowedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowings_FollowingUserId",
                table: "UserFollowings",
                column: "FollowingUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollowed");

            migrationBuilder.DropTable(
                name: "UserFollowings");

            migrationBuilder.AddColumn<string>(
                name: "_UserId",
                table: "User",
                type: "character varying(30)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_User__UserId",
                table: "User",
                column: "_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_User__UserId",
                table: "User",
                column: "_UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
