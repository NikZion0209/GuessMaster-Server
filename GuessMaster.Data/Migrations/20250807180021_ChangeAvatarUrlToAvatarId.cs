using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuessMaster.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAvatarUrlToAvatarId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "Users",
                newName: "AvatarId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
