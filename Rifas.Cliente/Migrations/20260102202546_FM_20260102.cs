using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rifas.Client.Migrations
{
    /// <inheritdoc />
    public partial class FM_20260102 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PurchaseId",
                table: "Tickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PurchaseId",
                table: "Tickets",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_RaffleId_TicketNumber",
                table: "Tickets",
                columns: new[] { "RaffleId", "TicketNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Purchases_PurchaseId",
                table: "Tickets",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Purchases_PurchaseId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_PurchaseId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_RaffleId_TicketNumber",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PurchaseId",
                table: "Tickets");
        }
    }
}
