using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameCategoryBudgetToBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "category_budgets",
                newName: "budgets",
                schema: "app");

            migrationBuilder.AddColumn<short>(
                name: "scope",
                schema: "app",
                table: "budgets",
                type: "smallint",
                nullable: false,
                defaultValue: (short)2,
                comment: "1 = General, 2 = Category");

            migrationBuilder.AlterColumn<long>(
                name: "category_id",
                table: "budgets",
                schema: "app",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.Sql("""
                 ALTER TABLE app.budgets
                RENAME CONSTRAINT "PK_category_budgets" TO "PK_budgets";
                
                ALTER TABLE app.budgets
                RENAME CONSTRAINT fk_category_budgets_category TO fk_budget_category;
                
                ALTER TABLE app.budgets
                RENAME CONSTRAINT fk_category_budgets_workspace TO fk_budget_workspace;
                """);

            migrationBuilder.RenameIndex(
                name: "IX_category_budgets_category_id",
                newName: "IX_budgets_category_id",
                schema: "app",
                table: "budgets");

            migrationBuilder.RenameIndex(
                name: "IX_category_budgets_workspace_id_category_id_period_start_peri~",
                newName: "IX_budgets_workspace_id_category_id_period_start_period_end",
                schema: "app",
                table: "budgets");

            migrationBuilder.Sql("""
                CREATE EXTENSION IF NOT EXISTS btree_gist;

                ALTER TABLE app.budgets
                ADD CONSTRAINT ex_budgets_general_no_overlap
                EXCLUDE USING gist
                (
                    workspace_id WITH =,
                    daterange(period_start, period_end, '[]') WITH &&
                )
                WHERE (scope = 1);
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            ALTER TABLE app.budgets
            DROP CONSTRAINT ex_budgets_general_no_overlap;
            """);

            migrationBuilder.RenameTable(
                name: "budgets",
                newName: "category_budgets",
                schema: "app");

            migrationBuilder.DropColumn(
                name: "scope",
                schema: "app",
                table: "category_budgets");


            migrationBuilder.AlterColumn<long>(
                name: "category_id",
                table: "category_budgets",
                schema: "app",
                type: "bigint",
                nullable: false,
                oldNullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.Sql("""
                 ALTER TABLE app.category_budgets
                RENAME CONSTRAINT "PK_budgets" TO "PK_category_budgets";
                
                ALTER TABLE app.category_budgets
                RENAME CONSTRAINT fk_budget_category TO fk_category_budgets_category;
                
                ALTER TABLE app.category_budgets
                RENAME CONSTRAINT fk_budget_workspace TO fk_category_budgets_workspace;
                """);

            migrationBuilder.RenameIndex(
                name: "IX_budgets_category_id",
                newName: "IX_category_budgets_category_id",
                schema: "app",
                table: "category_budgets");

            migrationBuilder.RenameIndex(
                name: "IX_budgets_workspace_id_category_id_period_start_period_end",
                newName: "IX_category_budgets_workspace_id_category_id_period_start_peri~",
                schema: "app",
                table: "category_budgets");
        }
    }
}
