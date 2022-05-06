using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthServer.Migrations
{
    public partial class AccessTokenRevokedProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserApplications",
                table: "UserApplications");

            migrationBuilder.DropIndex(
                name: "IX_UserApplications_UserId",
                table: "UserApplications");

            migrationBuilder.AddColumn<bool>(
                name: "Revoked",
                table: "AccessTokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                table: "AccessTokens",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserApplications",
                table: "UserApplications",
                columns: new[] { "UserId", "ApplicationId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserApplications",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "Revoked",
                table: "AccessTokens");

            migrationBuilder.DropColumn(
                name: "RevokedAt",
                table: "AccessTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserApplications",
                table: "UserApplications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_UserId",
                table: "UserApplications",
                column: "UserId");
        }
    }
}
