using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Infrastructure.Persistence.Migrations
{
    public partial class AddComputedColumnFullName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "[LastName] + ', ' + [FirstName]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Employees");
        }
    }
}
