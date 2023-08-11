using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    /// <inheritdoc />
    public partial class init5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserUser");

            migrationBuilder.CreateTable(
                name: "UserFollowing",
                columns: table => new
                {
                    FollowingUserId = table.Column<string>(type: "character varying(30)", nullable: false),
                    FollowerUserId = table.Column<string>(type: "character varying(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowing", x => new { x.FollowingUserId, x.FollowerUserId });
                    table.ForeignKey(
                        name: "FK_UserFollowing_User_FollowerUserId",
                        column: x => x.FollowerUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFollowing_User_FollowingUserId",
                        column: x => x.FollowingUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowing_FollowerUserId",
                table: "UserFollowing",
                column: "FollowerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollowing");

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
                name: "IX_UserUser_UsersFollowedMeId",
                table: "UserUser",
                column: "UsersFollowedMeId");
        }
    }
}
