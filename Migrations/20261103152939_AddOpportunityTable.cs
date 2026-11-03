
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcxiomCRM.Migrations
{
    public partial class AddOpportunityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),

                    OpportunityName = table.Column<string>(type: "TEXT", nullable: false),

                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),

                    CompanyName = table.Column<string>(type: "TEXT", nullable: false),

                    DealValue = table.Column<decimal>(type: "TEXT", nullable: false),

                    Stage = table.Column<string>(type: "TEXT", nullable: false),

                    Status = table.Column<string>(type: "TEXT", nullable: false),

                    ExpectedCloseDate = table.Column<DateTime>(type: "TEXT", nullable: false),

                    Description = table.Column<string>(type: "TEXT", nullable: false),

                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Opportunities");
        }
    }
}
