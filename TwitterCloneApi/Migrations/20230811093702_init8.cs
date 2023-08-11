using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    /// <inheritdoc />
    public partial class init8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_User_UserId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_UserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "User");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "User",
                type: "character varying(30)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserId",
                table: "User",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_User_UserId",
                table: "User",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
