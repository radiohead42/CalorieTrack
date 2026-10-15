using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalorieTrack.Api.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentWeightkg",
                table: "UserProfiles",
                newName: "CurrentWeightKg");

            migrationBuilder.RenameColumn(
                name: "GoalType",
                table: "UserProfiles",
                newName: "Goal");

            migrationBuilder.RenameColumn(
                name: "CreateAtUtc",
                table: "UserProfiles",
                newName: "CreatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentWeightKg",
                table: "UserProfiles",
                newName: "CurrentWeightkg");

            migrationBuilder.RenameColumn(
                name: "Goal",
                table: "UserProfiles",
                newName: "GoalType");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "UserProfiles",
                newName: "CreateAtUtc");
        }
    }
}
