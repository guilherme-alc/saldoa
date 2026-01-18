using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuBolso.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AjusteDeDateTimeCategoryTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NormalizedName",
                schema: "app",
                table: "categories",
                newName: "normalized_name");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "paid_or_received_at",
                schema: "app",
                table: "transactions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "app",
                table: "transactions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                schema: "app",
                table: "categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "app",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "normalized_name",
                schema: "app",
                table: "categories",
                newName: "NormalizedName");

            migrationBuilder.AlterColumn<DateTime>(
                name: "paid_or_received_at",
                schema: "app",
                table: "transactions",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "app",
                table: "transactions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }
    }
}
