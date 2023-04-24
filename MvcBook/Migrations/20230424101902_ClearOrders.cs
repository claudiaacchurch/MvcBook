using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcBook.Migrations
{
    /// <inheritdoc />
    public partial class ClearOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM ShoppingCartItems");
            migrationBuilder.Sql("DELETE FROM Orders");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
