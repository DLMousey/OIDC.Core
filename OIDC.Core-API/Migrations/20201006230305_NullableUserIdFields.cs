using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthServer.Migrations
{
    public partial class NullableUserIdFields : Migration
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

            migrationBuilder.AlterColumn<Guid>(
                name: "RefreshTokenId",
                table: "UserApplications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthorisationCodeId",
                table: "UserApplications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "AccessTokenId",
                table: "UserApplications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<Guid>(
                name: "RefreshTokenId",
                table: "UserApplications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthorisationCodeId",
                table: "UserApplications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AccessTokenId",
                table: "UserApplications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_AccessTokens_AccessTokenId",
                table: "UserApplications",
                column: "AccessTokenId",
                principalTable: "AccessTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_AuthorisationCodes_AuthorisationCodeId",
                table: "UserApplications",
                column: "AuthorisationCodeId",
                principalTable: "AuthorisationCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_RefreshTokens_RefreshTokenId",
                table: "UserApplications",
                column: "RefreshTokenId",
                principalTable: "RefreshTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
