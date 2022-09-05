using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricityDataAPI.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RealEstates",
                columns: table => new
                {
                    ObjectNumber = table.Column<int>(type: "int", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    HouseType = table.Column<byte>(type: "tinyint", nullable: false),
                    ObjectType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstates", x => x.ObjectNumber);
                });

            migrationBuilder.CreateTable(
                name: "ElectricityReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumedElectricity = table.Column<float>(type: "real", nullable: true),
                    GeneratedElectricity = table.Column<float>(type: "real", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    RealEstateObjectNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectricityReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectricityReports_RealEstates_RealEstateObjectNumber",
                        column: x => x.RealEstateObjectNumber,
                        principalTable: "RealEstates",
                        principalColumn: "ObjectNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElectricityReports_RealEstateObjectNumber",
                table: "ElectricityReports",
                column: "RealEstateObjectNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElectricityReports");

            migrationBuilder.DropTable(
                name: "RealEstates");
        }
    }
}
