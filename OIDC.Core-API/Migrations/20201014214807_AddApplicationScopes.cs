using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthServer.Migrations
{
    public partial class AddApplicationScopes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApplications_AccessTokens_AccessTokenId",
                table: "UserApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_UserApplications_AuthorisationCodes_AuthorisationCodeId",
                table: "UserApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_UserApplications_RefreshTokens_RefreshTokenId",
                table: "UserApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserApplications",
                table: "UserApplications");

            migrationBuilder.DropIndex(
                name: "IX_UserApplications_AccessTokenId",
                table: "UserApplications");

            migrationBuilder.DropIndex(
                name: "IX_UserApplications_AuthorisationCodeId",
                table: "UserApplications");

            migrationBuilder.DropIndex(
                name: "IX_UserApplications_RefreshTokenId",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "AccessTokenId",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "AuthorisationCodeId",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "RefreshTokenId",
                table: "UserApplications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserApplications",
                table: "UserApplications",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApplicationScopes",
                columns: table => new
                {
                    UserApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationScopes", x => new { x.UserApplicationId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_ApplicationScopes_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationScopes_UserApplications_UserApplicationId",
                        column: x => x.UserApplicationId,
                        principalTable: "UserApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_UserId",
                table: "UserApplications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationScopes_ScopeId",
                table: "ApplicationScopes",
                column: "ScopeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationScopes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserApplications",
                table: "UserApplications");

            migrationBuilder.DropIndex(
                name: "IX_UserApplications_UserId",
                table: "UserApplications");

            migrationBuilder.AddColumn<Guid>(
                name: "AccessTokenId",
                table: "UserApplications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorisationCodeId",
                table: "UserApplications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserApplications",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "RefreshTokenId",
                table: "UserApplications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserApplications",
                table: "UserApplications",
                columns: new[] { "UserId", "ApplicationId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_AccessTokenId",
                table: "UserApplications",
                column: "AccessTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_AuthorisationCodeId",
                table: "UserApplications",
                column: "AuthorisationCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_RefreshTokenId",
                table: "UserApplications",
                column: "RefreshTokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_AccessTokens_AccessTokenId",
                table: "UserApplications",
                column: "AccessTokenId",
                principalTable: "AccessTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_AuthorisationCodes_AuthorisationCodeId",
                table: "UserApplications",
                column: "AuthorisationCodeId",
                principalTable: "AuthorisationCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_RefreshTokens_RefreshTokenId",
                table: "UserApplications",
                column: "RefreshTokenId",
                principalTable: "RefreshTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
