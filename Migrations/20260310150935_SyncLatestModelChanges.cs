using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace evacPlanMoni.Migrations
{
    /// <inheritdoc />
    public partial class SyncLatestModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETAHours",
                table: "EvacuationPlans");

            migrationBuilder.RenameColumn(
                name: "PeopleToEvacuate",
                table: "EvacuationPlans",
                newName: "NumberOfPeople");

            migrationBuilder.AddColumn<string>(
                name: "ETA",
                table: "EvacuationPlans",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETA",
                table: "EvacuationPlans");

            migrationBuilder.RenameColumn(
                name: "NumberOfPeople",
                table: "EvacuationPlans",
                newName: "PeopleToEvacuate");

            migrationBuilder.AddColumn<double>(
                name: "ETAHours",
                table: "EvacuationPlans",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
