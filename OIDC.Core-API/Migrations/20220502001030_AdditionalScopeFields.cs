using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuthServer.Migrations
{
    public partial class AdditionalScopeFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Dangerous",
                table: "Scopes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "Scopes",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dangerous",
                table: "Scopes");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "Scopes");
        }
    }
}
