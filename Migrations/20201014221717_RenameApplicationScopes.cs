using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthServer.Migrations
{
    public partial class RenameApplicationScopes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationScopes_Scopes_ScopeId",
                table: "ApplicationScopes");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationScopes_UserApplications_UserApplicationId",
                table: "ApplicationScopes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationScopes",
                table: "ApplicationScopes");

            migrationBuilder.RenameTable(
                name: "ApplicationScopes",
                newName: "UserApplicationScopes");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationScopes_ScopeId",
                table: "UserApplicationScopes",
                newName: "IX_UserApplicationScopes_ScopeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserApplicationScopes",
                table: "UserApplicationScopes",
                columns: new[] { "UserApplicationId", "ScopeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplicationScopes_Scopes_ScopeId",
                table: "UserApplicationScopes",
                column: "ScopeId",
                principalTable: "Scopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplicationScopes_UserApplications_UserApplicationId",
                table: "UserApplicationScopes",
                column: "UserApplicationId",
                principalTable: "UserApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApplicationScopes_Scopes_ScopeId",
                table: "UserApplicationScopes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserApplicationScopes_UserApplications_UserApplicationId",
                table: "UserApplicationScopes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserApplicationScopes",
                table: "UserApplicationScopes");

            migrationBuilder.RenameTable(
                name: "UserApplicationScopes",
                newName: "ApplicationScopes");

            migrationBuilder.RenameIndex(
                name: "IX_UserApplicationScopes_ScopeId",
                table: "ApplicationScopes",
                newName: "IX_ApplicationScopes_ScopeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationScopes",
                table: "ApplicationScopes",
                columns: new[] { "UserApplicationId", "ScopeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationScopes_Scopes_ScopeId",
                table: "ApplicationScopes",
                column: "ScopeId",
                principalTable: "Scopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationScopes_UserApplications_UserApplicationId",
                table: "ApplicationScopes",
                column: "UserApplicationId",
                principalTable: "UserApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
