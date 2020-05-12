using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicamentsAggregator.Service.Migrations
{
    public partial class AddCityAndFormattedAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Pharmacy",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormattedAddress",
                table: "Pharmacy",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Pharmacy");

            migrationBuilder.DropColumn(
                name: "FormattedAddress",
                table: "Pharmacy");
        }
    }
}
