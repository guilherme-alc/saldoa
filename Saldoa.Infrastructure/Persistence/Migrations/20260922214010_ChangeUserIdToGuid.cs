using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_claims",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_roles",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "auth");

            DropUserForeignKeys(migrationBuilder);

            migrationBuilder.Sql("""
                ALTER TABLE "auth"."users"
                    ALTER COLUMN "Id" TYPE uuid USING "Id"::uuid;

                ALTER TABLE "auth"."user_claims"
                    ALTER COLUMN "UserId" TYPE uuid USING "UserId"::uuid;

                ALTER TABLE "auth"."user_logins"
                    ALTER COLUMN "UserId" TYPE uuid USING "UserId"::uuid;

                ALTER TABLE "auth"."user_tokens"
                    ALTER COLUMN "UserId" TYPE uuid USING "UserId"::uuid;

                ALTER TABLE "auth"."refresh_tokens"
                    ALTER COLUMN "user_id" TYPE uuid USING "user_id"::uuid;

                ALTER TABLE "app"."transactions"
                    ALTER COLUMN "created_by_id" TYPE uuid USING "created_by_id"::uuid;

                ALTER TABLE "app"."workspace"
                    ALTER COLUMN "created_by_user_id" TYPE uuid USING "created_by_user_id"::uuid;

                ALTER TABLE "app"."workspace_invitation"
                    ALTER COLUMN "invited_by_user_id" TYPE uuid USING "invited_by_user_id"::uuid,
                    ALTER COLUMN "accepted_by_user_id" TYPE uuid USING "accepted_by_user_id"::uuid;

                ALTER TABLE "app"."workspace_memberships"
                    ALTER COLUMN "user_id" TYPE uuid USING "user_id"::uuid;
                """);

            AddUserForeignKeys(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropUserForeignKeys(migrationBuilder);

            migrationBuilder.Sql("""
                ALTER TABLE "auth"."users"
                    ALTER COLUMN "Id" TYPE text USING "Id"::text;

                ALTER TABLE "auth"."user_claims"
                    ALTER COLUMN "UserId" TYPE text USING "UserId"::text;

                ALTER TABLE "auth"."user_logins"
                    ALTER COLUMN "UserId" TYPE text USING "UserId"::text;

                ALTER TABLE "auth"."user_tokens"
                    ALTER COLUMN "UserId" TYPE text USING "UserId"::text;

                ALTER TABLE "auth"."refresh_tokens"
                    ALTER COLUMN "user_id" TYPE text USING "user_id"::text;

                ALTER TABLE "app"."transactions"
                    ALTER COLUMN "created_by_id" TYPE text USING "created_by_id"::text;

                ALTER TABLE "app"."workspace"
                    ALTER COLUMN "created_by_user_id" TYPE text USING "created_by_user_id"::text;

                ALTER TABLE "app"."workspace_invitation"
                    ALTER COLUMN "invited_by_user_id" TYPE text USING "invited_by_user_id"::text,
                    ALTER COLUMN "accepted_by_user_id" TYPE text USING "accepted_by_user_id"::text;

                ALTER TABLE "app"."workspace_memberships"
                    ALTER COLUMN "user_id" TYPE text USING "user_id"::text;
                """);

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_claims_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_role_claims_RoleId",
                schema: "auth",
                table: "role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "auth",
                table: "roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_RoleId",
                schema: "auth",
                table: "user_roles",
                column: "RoleId");

            AddUserForeignKeys(migrationBuilder);
        }

        private static void DropUserForeignKeys(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_claims_users_UserId",
                schema: "auth",
                table: "user_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_user_logins_users_UserId",
                schema: "auth",
                table: "user_logins");

            migrationBuilder.DropForeignKey(
                name: "FK_user_tokens_users_UserId",
                schema: "auth",
                table: "user_tokens");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_created_by_user",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_created_by_user",
                schema: "app",
                table: "workspace");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_invitations_accepted_by_user",
                schema: "app",
                table: "workspace_invitation");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_invitations_invited_by_user",
                schema: "app",
                table: "workspace_invitation");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_memberships_user",
                schema: "app",
                table: "workspace_memberships");
        }

        private static void AddUserForeignKeys(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_user_claims_users_UserId",
                schema: "auth",
                table: "user_claims",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_logins_users_UserId",
                schema: "auth",
                table: "user_logins",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_tokens_users_UserId",
                schema: "auth",
                table: "user_tokens",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_created_by_user",
                schema: "app",
                table: "transactions",
                column: "created_by_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_created_by_user",
                schema: "app",
                table: "workspace",
                column: "created_by_user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_invitations_accepted_by_user",
                schema: "app",
                table: "workspace_invitation",
                column: "accepted_by_user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_invitations_invited_by_user",
                schema: "app",
                table: "workspace_invitation",
                column: "invited_by_user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_memberships_user",
                schema: "app",
                table: "workspace_memberships",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
