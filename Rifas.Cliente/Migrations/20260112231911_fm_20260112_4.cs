using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rifas.Client.Migrations
{
    /// <inheritdoc />
    public partial class fm_20260112_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Category",
                table: "Raffles",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Raffles_Category",
                table: "Raffles",
                column: "Category");

            migrationBuilder.AddForeignKey(
                name: "FK_Raffles_Categories_Category",
                table: "Raffles",
                column: "Category",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Raffles_Categories_Category",
                table: "Raffles");

            migrationBuilder.DropIndex(
                name: "IX_Raffles_Category",
                table: "Raffles");

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Raffles",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
