using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OIDC.CoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedScopes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("0a1f9070-f943-4337-8de2-4e4c21219215"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e43b033c-ed04-4620-9abe-2d9139ab14c1"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("036beea1-5ab6-4f0b-85fa-e3e9f14ab057"), "admin" },
                    { new Guid("41337165-9c3a-4c6a-8d89-9a4b26659168"), "user" }
                });

            migrationBuilder.InsertData(
                table: "Scopes",
                columns: new[] { "Id", "Dangerous", "Description", "Icon", "Label", "Name" },
                values: new object[,]
                {
                    { new Guid("59d4dcbc-8eac-4a9b-ad6a-3bf07164f888"), false, "Allows modification of details on your profile, not including your password or email address", "true", "Update Profile", "profile.write" },
                    { new Guid("f0a49419-cd3f-4dfa-be49-18c44bff7d5f"), false, "Allows listing applications you have published", "true", "List applications you have published", "applications.read" },
                    { new Guid("f19575cb-2364-4d29-816b-af9847685efb"), false, "Allows read-only access to your profile information", "account", "Read Profile", "profile.read" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("036beea1-5ab6-4f0b-85fa-e3e9f14ab057"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("41337165-9c3a-4c6a-8d89-9a4b26659168"));

            migrationBuilder.DeleteData(
                table: "Scopes",
                keyColumn: "Id",
                keyValue: new Guid("59d4dcbc-8eac-4a9b-ad6a-3bf07164f888"));

            migrationBuilder.DeleteData(
                table: "Scopes",
                keyColumn: "Id",
                keyValue: new Guid("f0a49419-cd3f-4dfa-be49-18c44bff7d5f"));

            migrationBuilder.DeleteData(
                table: "Scopes",
                keyColumn: "Id",
                keyValue: new Guid("f19575cb-2364-4d29-816b-af9847685efb"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0a1f9070-f943-4337-8de2-4e4c21219215"), "user" },
                    { new Guid("e43b033c-ed04-4620-9abe-2d9139ab14c1"), "admin" }
                });
        }
    }
}
