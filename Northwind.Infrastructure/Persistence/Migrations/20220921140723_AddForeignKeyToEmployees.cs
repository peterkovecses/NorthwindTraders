using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Infrastructure.Persistence.Migrations
{
    public partial class AddForeignKeyToEmployees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees",
                table: "Employees",
                column: "ReportsTo",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
