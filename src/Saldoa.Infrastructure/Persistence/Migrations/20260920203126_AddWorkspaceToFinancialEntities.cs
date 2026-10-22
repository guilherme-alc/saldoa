using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkspaceToFinancialEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_categories_user",
                schema: "app",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "fk_category_budgets_user",
                schema: "app",
                table: "category_budgets");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_user",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_user_id_paid_or_received_at",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_category_budgets_user_id_category_id_period_start_period_end",
                schema: "app",
                table: "category_budgets");

            migrationBuilder.DropIndex(
                name: "ux_categories_user_normalized_name",
                schema: "app",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "app",
                table: "transactions",
                newName: "created_by_id");

            migrationBuilder.AddColumn<Guid>(
                name: "workspace_id",
                schema: "app",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "workspace_id",
                schema: "app",
                table: "category_budgets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "workspace_id",
                schema: "app",
                table: "categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                CREATE TEMP TABLE legacy_user_workspace_map
                (
                    user_id text PRIMARY KEY,
                    workspace_id uuid NOT NULL
                ) ON COMMIT DROP;

                INSERT INTO legacy_user_workspace_map (user_id, workspace_id)
                SELECT legacy_users.user_id,
                       COALESCE(
                           (
                               SELECT workspace.id
                               FROM app.workspace AS workspace
                               WHERE workspace.created_by_user_id = legacy_users.user_id
                               ORDER BY workspace.created_at, workspace.id
                               LIMIT 1
                           ),
                           gen_random_uuid()
                       )
                FROM
                (
                    SELECT user_id FROM app.categories
                    UNION
                    SELECT user_id FROM app.category_budgets
                    UNION
                    SELECT created_by_id FROM app.transactions
                ) AS legacy_users;

                INSERT INTO app.workspace (id, name, created_by_user_id, created_at)
                SELECT mapping.workspace_id,
                       'Workspace pessoal',
                       mapping.user_id,
                       CURRENT_TIMESTAMP
                FROM legacy_user_workspace_map AS mapping
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM app.workspace AS workspace
                    WHERE workspace.id = mapping.workspace_id
                );

                INSERT INTO app.workspace_memberships (id, workspace_id, user_id, role, joined_at)
                SELECT gen_random_uuid(),
                       mapping.workspace_id,
                       mapping.user_id,
                       2,
                       CURRENT_TIMESTAMP
                FROM legacy_user_workspace_map AS mapping
                ON CONFLICT (workspace_id, user_id) DO NOTHING;

                UPDATE app.categories AS category
                SET workspace_id = mapping.workspace_id
                FROM legacy_user_workspace_map AS mapping
                WHERE category.user_id = mapping.user_id;

                UPDATE app.category_budgets AS budget
                SET workspace_id = mapping.workspace_id
                FROM legacy_user_workspace_map AS mapping
                WHERE budget.user_id = mapping.user_id;

                UPDATE app.transactions AS financial_transaction
                SET workspace_id = mapping.workspace_id
                FROM legacy_user_workspace_map AS mapping
                WHERE financial_transaction.created_by_id = mapping.user_id;
                """);

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "app",
                table: "category_budgets");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "app",
                table: "categories");

            migrationBuilder.AlterColumn<Guid>(
                name: "workspace_id",
                schema: "app",
                table: "transactions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "workspace_id",
                schema: "app",
                table: "category_budgets",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "workspace_id",
                schema: "app",
                table: "categories",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_created_by_id",
                schema: "app",
                table: "transactions",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_workspace_id_paid_or_received_at",
                schema: "app",
                table: "transactions",
                columns: new[] { "workspace_id", "paid_or_received_at" });

            migrationBuilder.CreateIndex(
                name: "IX_category_budgets_workspace_id_category_id_period_start_peri~",
                schema: "app",
                table: "category_budgets",
                columns: new[] { "workspace_id", "category_id", "period_start", "period_end" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_categories_workspace_normalized_name",
                schema: "app",
                table: "categories",
                columns: new[] { "workspace_id", "normalized_name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_categories_workspace",
                schema: "app",
                table: "categories",
                column: "workspace_id",
                principalSchema: "app",
                principalTable: "workspace",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_category_budgets_workspace",
                schema: "app",
                table: "category_budgets",
                column: "workspace_id",
                principalSchema: "app",
                principalTable: "workspace",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_workspace",
                schema: "app",
                table: "transactions",
                column: "workspace_id",
                principalSchema: "app",
                principalTable: "workspace",
                principalColumn: "id",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_categories_workspace",
                schema: "app",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "fk_category_budgets_workspace",
                schema: "app",
                table: "category_budgets");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_workspace",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_created_by_user",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_created_by_id",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_workspace_id_paid_or_received_at",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_category_budgets_workspace_id_category_id_period_start_peri~",
                schema: "app",
                table: "category_budgets");

            migrationBuilder.DropIndex(
                name: "ux_categories_workspace_normalized_name",
                schema: "app",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                schema: "app",
                table: "category_budgets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                schema: "app",
                table: "categories",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE app.categories AS category
                SET user_id = workspace.created_by_user_id
                FROM app.workspace AS workspace
                WHERE category.workspace_id = workspace.id;

                UPDATE app.category_budgets AS budget
                SET user_id = workspace.created_by_user_id
                FROM app.workspace AS workspace
                WHERE budget.workspace_id = workspace.id;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                schema: "app",
                table: "category_budgets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                schema: "app",
                table: "categories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "workspace_id",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "workspace_id",
                schema: "app",
                table: "category_budgets");

            migrationBuilder.DropColumn(
                name: "workspace_id",
                schema: "app",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                schema: "app",
                table: "transactions",
                newName: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id_paid_or_received_at",
                schema: "app",
                table: "transactions",
                columns: new[] { "user_id", "paid_or_received_at" });

            migrationBuilder.CreateIndex(
                name: "IX_category_budgets_user_id_category_id_period_start_period_end",
                schema: "app",
                table: "category_budgets",
                columns: new[] { "user_id", "category_id", "period_start", "period_end" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_categories_user_normalized_name",
                schema: "app",
                table: "categories",
                columns: new[] { "user_id", "normalized_name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_categories_user",
                schema: "app",
                table: "categories",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_category_budgets_user",
                schema: "app",
                table: "category_budgets",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_user",
                schema: "app",
                table: "transactions",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
