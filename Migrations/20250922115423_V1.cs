using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectLaborBackend.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductWarehouse_Products_ProductsId",
                table: "ProductWarehouse");

            migrationBuilder.DropTable(
                name: "DeliveryProduct");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.RenameColumn(
                name: "ProductsId",
                table: "ProductWarehouse",
                newName: "ProductsEAN");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Products",
                newName: "EAN");

            migrationBuilder.CreateTable(
                name: "StockChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockChanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductStockChange",
                columns: table => new
                {
                    ProductsEAN = table.Column<int>(type: "int", nullable: false),
                    StockChangesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStockChange", x => new { x.ProductsEAN, x.StockChangesId });
                    table.ForeignKey(
                        name: "FK_ProductStockChange_Products_ProductsEAN",
                        column: x => x.ProductsEAN,
                        principalTable: "Products",
                        principalColumn: "EAN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductStockChange_StockChanges_StockChangesId",
                        column: x => x.StockChangesId,
                        principalTable: "StockChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductStockChange_StockChangesId",
                table: "ProductStockChange",
                column: "StockChangesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWarehouse_Products_ProductsEAN",
                table: "ProductWarehouse",
                column: "ProductsEAN",
                principalTable: "Products",
                principalColumn: "EAN",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductWarehouse_Products_ProductsEAN",
                table: "ProductWarehouse");

            migrationBuilder.DropTable(
                name: "ProductStockChange");

            migrationBuilder.DropTable(
                name: "StockChanges");

            migrationBuilder.RenameColumn(
                name: "ProductsEAN",
                table: "ProductWarehouse",
                newName: "ProductsId");

            migrationBuilder.RenameColumn(
                name: "EAN",
                table: "Products",
                newName: "Id");

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryProduct",
                columns: table => new
                {
                    DeliveriesId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryProduct", x => new { x.DeliveriesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_DeliveryProduct_Deliveries_DeliveriesId",
                        column: x => x.DeliveriesId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryProduct_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryProduct_ProductsId",
                table: "DeliveryProduct",
                column: "ProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWarehouse_Products_ProductsId",
                table: "ProductWarehouse",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
