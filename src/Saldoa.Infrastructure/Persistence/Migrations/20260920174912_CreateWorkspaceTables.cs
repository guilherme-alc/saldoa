using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateWorkspaceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "type",
                schema: "app",
                table: "transactions",
                type: "smallint",
                nullable: false,
                comment: "1 = Expense, 2 = Income",
                oldClrType: typeof(short),
                oldType: "smallint",
                oldComment: "1 = Deposit, 2 = Withdraw");

            migrationBuilder.CreateTable(
                name: "workspace",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    created_by_user_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace", x => x.id);
                    table.ForeignKey(
                        name: "fk_workspace_created_by_user",
                        column: x => x.created_by_user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "workspace_invitation",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    invited_by_username = table.Column<string>(type: "text", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<short>(type: "smallint", nullable: false, comment: "1 = Member, 2 = Owner, 3 = ReadOnly"),
                    workspace_invitation_status = table.Column<short>(type: "smallint", nullable: false, comment: "1 = Pending, 2 = Accepted, 3 = Declined, 4 = Expired, 5 = Revoked"),
                    accepted_by_user_id = table.Column<string>(type: "text", nullable: true),
                    invited_by_user_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    accepted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_invitation", x => x.id);
                    table.ForeignKey(
                        name: "fk_workspace_invitations_accepted_by_user",
                        column: x => x.accepted_by_user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_workspace_invitations_invited_by_user",
                        column: x => x.invited_by_user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workspace_invitations_workspace",
                        column: x => x.workspace_id,
                        principalSchema: "app",
                        principalTable: "workspace",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workspace_memberships",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<short>(type: "smallint", nullable: false, comment: "1 = Member, 2 = Owner, 3 = ReadOnly"),
                    joined_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_memberships", x => x.id);
                    table.ForeignKey(
                        name: "fk_workspace_memberships_user",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_workspace_memberships_workspace",
                        column: x => x.workspace_id,
                        principalSchema: "app",
                        principalTable: "workspace",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_workspace_created_by_user_id",
                schema: "app",
                table: "workspace",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_invitation_accepted_by_user_id",
                schema: "app",
                table: "workspace_invitation",
                column: "accepted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_invitation_invited_by_user_id",
                schema: "app",
                table: "workspace_invitation",
                column: "invited_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ux_workspace_invitations_pending_email",
                schema: "app",
                table: "workspace_invitation",
                columns: new[] { "workspace_id", "email" },
                unique: true,
                filter: "workspace_invitation_status = 1");

            migrationBuilder.CreateIndex(
                name: "ux_workspace_invitations_token_hash",
                schema: "app",
                table: "workspace_invitation",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_workspace_memberships_user_id",
                schema: "app",
                table: "workspace_memberships",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_workspace_member_user",
                schema: "app",
                table: "workspace_memberships",
                columns: new[] { "workspace_id", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workspace_invitation",
                schema: "app");

            migrationBuilder.DropTable(
                name: "workspace_memberships",
                schema: "app");

            migrationBuilder.DropTable(
                name: "workspace",
                schema: "app");

            migrationBuilder.AlterColumn<short>(
                name: "type",
                schema: "app",
                table: "transactions",
                type: "smallint",
                nullable: false,
                comment: "1 = Deposit, 2 = Withdraw",
                oldClrType: typeof(short),
                oldType: "smallint",
                oldComment: "1 = Expense, 2 = Income");
        }
    }
}
