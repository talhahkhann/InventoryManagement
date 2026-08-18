using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProductPrice_Customers_CustomerId",
                table: "CustomerProductPrice");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProductPrice_Products_ProductId",
                table: "CustomerProductPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerProductPrice",
                table: "CustomerProductPrice");

            migrationBuilder.RenameTable(
                name: "CustomerProductPrice",
                newName: "CustomerProductPrices");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerProductPrice_ProductId",
                table: "CustomerProductPrices",
                newName: "IX_CustomerProductPrices_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerProductPrice_CustomerId_ProductId",
                table: "CustomerProductPrices",
                newName: "IX_CustomerProductPrices_CustomerId_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerProductPrices",
                table: "CustomerProductPrices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProductPrices_Customers_CustomerId",
                table: "CustomerProductPrices",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProductPrices_Products_ProductId",
                table: "CustomerProductPrices",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProductPrices_Customers_CustomerId",
                table: "CustomerProductPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProductPrices_Products_ProductId",
                table: "CustomerProductPrices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerProductPrices",
                table: "CustomerProductPrices");

            migrationBuilder.RenameTable(
                name: "CustomerProductPrices",
                newName: "CustomerProductPrice");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerProductPrices_ProductId",
                table: "CustomerProductPrice",
                newName: "IX_CustomerProductPrice_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerProductPrices_CustomerId_ProductId",
                table: "CustomerProductPrice",
                newName: "IX_CustomerProductPrice_CustomerId_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerProductPrice",
                table: "CustomerProductPrice",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProductPrice_Customers_CustomerId",
                table: "CustomerProductPrice",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProductPrice_Products_ProductId",
                table: "CustomerProductPrice",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
