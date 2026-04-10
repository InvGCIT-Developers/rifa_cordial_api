using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rifas.Service.Migrations
{
    /// <inheritdoc />
    public partial class fm_vacia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TicketId",
                table: "Results",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Raffles",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "WinnersNumber",
                table: "Raffles",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Categories",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Results_RaffleId",
                table: "Results",
                column: "RaffleId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_TicketId",
                table: "Results",
                column: "TicketId",
                unique: true,
                filter: "[TicketId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_RaffleId",
                table: "Purchases",
                column: "RaffleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Raffles_RaffleId",
                table: "Purchases",
                column: "RaffleId",
                principalTable: "Raffles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Raffles_RaffleId",
                table: "Results",
                column: "RaffleId",
                principalTable: "Raffles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Tickets_TicketId",
                table: "Results",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Raffles_RaffleId",
                table: "Tickets",
                column: "RaffleId",
                principalTable: "Raffles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Raffles_RaffleId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_Raffles_RaffleId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_Tickets_TicketId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Raffles_RaffleId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Results_RaffleId",
                table: "Results");

            migrationBuilder.DropIndex(
                name: "IX_Results_TicketId",
                table: "Results");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_RaffleId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "WinnersNumber",
                table: "Raffles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Categories");

            migrationBuilder.AlterColumn<long>(
                name: "Category",
                table: "Raffles",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Categories",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
