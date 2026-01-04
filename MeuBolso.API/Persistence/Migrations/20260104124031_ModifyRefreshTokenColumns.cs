using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuBolso.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyRefreshTokenColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_revoked",
                schema: "auth",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "token",
                schema: "auth",
                table: "refresh_tokens",
                newName: "token_hash");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_token",
                schema: "auth",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_token_hash");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "auth",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "replaced_by_token_hash",
                schema: "auth",
                table: "refresh_tokens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "revoked_at",
                schema: "auth",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "auth",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "replaced_by_token_hash",
                schema: "auth",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "revoked_at",
                schema: "auth",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "token_hash",
                schema: "auth",
                table: "refresh_tokens",
                newName: "token");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_token_hash",
                schema: "auth",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_token");

            migrationBuilder.AddColumn<bool>(
                name: "is_revoked",
                schema: "auth",
                table: "refresh_tokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
