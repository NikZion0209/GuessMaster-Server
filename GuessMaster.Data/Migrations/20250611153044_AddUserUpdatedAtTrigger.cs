using Microsoft.EntityFrameworkCore.Migrations;
namespace GuessMaster.Data.Migrations
{
    public partial class AddUserUpdatedAtTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
            CREATE TRIGGER SetUpdatedAtOnUserUpdate
            ON Users
            AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;
                UPDATE Users
                SET UpdatedAt = GETDATE()
                FROM inserted
                WHERE Users.UserId = inserted.UserId;
            END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"DROP TRIGGER IF EXISTS SetUpdatedAtOnUserUpdate;"
            );
        }
    }
};