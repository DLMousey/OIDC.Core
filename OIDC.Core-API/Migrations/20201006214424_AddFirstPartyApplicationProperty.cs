using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthServer.Migrations
{
    public partial class AddFirstPartyApplicationProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FirstParty",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstParty",
                table: "Applications");
        }
    }
}
