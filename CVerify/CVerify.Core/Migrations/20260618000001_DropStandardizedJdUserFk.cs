using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVerify.API.Migrations
{
    /// <inheritdoc />
    public partial class DropStandardizedJdUserFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // owner_user_id stores either a UserId or an OrganizationId depending on actor_type.
            // Organization IDs are not in the users table, so the FK constraint breaks JD creation
            // for business accounts. Removing the constraint allows both actor types to own JDs.
            // IF EXISTS guards against new databases where the table was already created without this FK.
            migrationBuilder.Sql(
                "ALTER TABLE standardized_jds DROP CONSTRAINT IF EXISTS fk_standardized_jds_users_owner_user_id;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "fk_standardized_jds_users_owner_user_id",
                table: "standardized_jds",
                column: "owner_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
