using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcBook.Migrations
{
    /// <inheritdoc />
    public partial class ResetAuthors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Authors WHERE Name LIKE '%Me%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
