using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddGiftCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiftCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PriceReduction = table.Column<decimal>(type: "numeric", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiftCardCodes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false),
                    GiftCardId = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftCardCodes", x => x.Code);
                    table.ForeignKey(
                        name: "FK_GiftCardCodes_GiftCards_GiftCardId",
                        column: x => x.GiftCardId,
                        principalTable: "GiftCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GiftCardCodes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GiftCardCodes_GiftCardId",
                table: "GiftCardCodes",
                column: "GiftCardId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftCardCodes_OrderId",
                table: "GiftCardCodes",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiftCardCodes");

            migrationBuilder.DropTable(
                name: "GiftCards");
        }
    }
}
