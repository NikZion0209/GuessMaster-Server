using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuessMaster.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClearUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Users]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
