using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rifas.Client.Migrations
{
    /// <inheritdoc />
    public partial class FM_20251226 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountActive",
                table: "Raffles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BottomNumber",
                table: "Raffles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GarantedWinner",
                table: "Raffles",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RaffleNumber",
                table: "Raffles",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopNUmber",
                table: "Raffles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "level",
                table: "Raffles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RaffleId = table.Column<long>(type: "bigint", nullable: false),
                    RaffleNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaffleId = table.Column<long>(type: "bigint", nullable: false),
                    RaffleNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    WinningNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    FirstPlace = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    SecondPlace = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    ThirdPlace = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LotteryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropColumn(
                name: "AmountActive",
                table: "Raffles");

            migrationBuilder.DropColumn(
                name: "BottomNumber",
                table: "Raffles");

            migrationBuilder.DropColumn(
                name: "GarantedWinner",
                table: "Raffles");

            migrationBuilder.DropColumn(
                name: "RaffleNumber",
                table: "Raffles");

            migrationBuilder.DropColumn(
                name: "TopNUmber",
                table: "Raffles");

            migrationBuilder.DropColumn(
                name: "level",
                table: "Raffles");
        }
    }
}
