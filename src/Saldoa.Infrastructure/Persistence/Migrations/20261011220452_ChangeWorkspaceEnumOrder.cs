using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeWorkspaceEnumOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE app.workspace_memberships
                SET role = CASE role WHEN 1 THEN 2 WHEN 2 THEN 3 WHEN 3 THEN 1 ELSE role END;
                UPDATE app.workspace_invitation
                SET role = CASE role WHEN 1 THEN 2 WHEN 2 THEN 3 WHEN 3 THEN 1 ELSE role END;
                """);

            migrationBuilder.AlterColumn<short>(
                name: "role",
                schema: "app",
                table: "workspace_memberships",
                type: "smallint",
                nullable: false,
                comment: "1 = ReadOnly, 2 = Member, 3 = Owner",
                oldClrType: typeof(short),
                oldType: "smallint",
                oldComment: "1 = Member, 2 = Owner, 3 = ReadOnly");

            migrationBuilder.AlterColumn<short>(
                name: "role",
                schema: "app",
                table: "workspace_invitation",
                type: "smallint",
                nullable: false,
                comment: "1 = ReadOnly, 2 = Member, 3 = Owner",
                oldClrType: typeof(short),
                oldType: "smallint",
                oldComment: "1 = Member, 2 = Owner, 3 = ReadOnly");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE app.workspace_memberships
                SET role = CASE role WHEN 1 THEN 3 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE role END;
                UPDATE app.workspace_invitation
                SET role = CASE role WHEN 1 THEN 3 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE role END;
                """);

            migrationBuilder.AlterColumn<short>(
                name: "role",
                schema: "app",
                table: "workspace_memberships",
                type: "smallint",
                nullable: false,
                comment: "1 = Member, 2 = Owner, 3 = ReadOnly",
                oldClrType: typeof(short),
                oldType: "smallint",
                oldComment: "1 = ReadOnly, 2 = Member, 3 = Owner");

            migrationBuilder.AlterColumn<short>(
                name: "role",
                schema: "app",
                table: "workspace_invitation",
                type: "smallint",
                nullable: false,
                comment: "1 = Member, 2 = Owner, 3 = ReadOnly",
                oldClrType: typeof(short),
                oldType: "smallint",
                oldComment: "1 = ReadOnly, 2 = Member, 3 = Owner");
        }
    }
}
