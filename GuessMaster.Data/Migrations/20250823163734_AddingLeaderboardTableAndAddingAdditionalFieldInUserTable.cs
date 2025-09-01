using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuessMaster.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingLeaderboardTableAndAddingAdditionalFieldInUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Users]");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HighscoreDoodleChamp",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HighscoreFlagWhiz",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HighscoreWordSnap",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PremiumToken",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Leaderboards",
                columns: table => new
                {
                    LeaderBoardRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gamemode = table.Column<int>(type: "int", nullable: false),
                    AvatarId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaderboards", x => x.LeaderBoardRecordId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leaderboards");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HighscoreDoodleChamp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HighscoreFlagWhiz",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HighscoreWordSnap",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PremiumToken",
                table: "Users");
        }
    }
}
