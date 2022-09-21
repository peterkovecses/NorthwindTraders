using Microsoft.EntityFrameworkCore.Migrations;
using Northwind.Infrastructure.Properties;

#nullable disable

namespace Northwind.Infrastructure.Persistence.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(Resources.ResourceManager.GetString("SeedData_Up"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
