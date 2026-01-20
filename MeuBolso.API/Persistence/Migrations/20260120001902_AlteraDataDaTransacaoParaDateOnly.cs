using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuBolso.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlteraDataDaTransacaoParaDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "paid_or_received_at",
                schema: "app",
                table: "transactions",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "paid_or_received_at",
                schema: "app",
                table: "transactions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
