using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcBook.Migrations
{
    /// <inheritdoc />
    public partial class ClearData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Genres");
            migrationBuilder.Sql("DELETE FROM Authors");
            migrationBuilder.Sql("DELETE FROM Books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
