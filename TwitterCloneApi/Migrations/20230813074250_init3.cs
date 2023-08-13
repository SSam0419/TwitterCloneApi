using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowings_User_FollowingUserId",
                table: "UserFollowings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowings_User_UserId",
                table: "UserFollowings");

            migrationBuilder.DropTable(
                name: "UserFollowed");

            migrationBuilder.RenameColumn(
                name: "FollowingUserId",
                table: "UserFollowings",
                newName: "ToUserId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserFollowings",
                newName: "FromUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowings_FollowingUserId",
                table: "UserFollowings",
                newName: "IX_UserFollowings_ToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowings_User_FromUserId",
                table: "UserFollowings",
                column: "FromUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowings_User_ToUserId",
                table: "UserFollowings",
                column: "ToUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowings_User_FromUserId",
                table: "UserFollowings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowings_User_ToUserId",
                table: "UserFollowings");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "UserFollowings",
                newName: "FollowingUserId");

            migrationBuilder.RenameColumn(
                name: "FromUserId",
                table: "UserFollowings",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowings_ToUserId",
                table: "UserFollowings",
                newName: "IX_UserFollowings_FollowingUserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowed_FollowedUserId",
                table: "UserFollowed",
                column: "FollowedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowings_User_FollowingUserId",
                table: "UserFollowings",
                column: "FollowingUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowings_User_UserId",
                table: "UserFollowings",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
