using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuessMaster.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserUpdatedAtTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS SetUpdatedAtOnUserUpdate;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
