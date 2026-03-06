using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace evacPlanMoni.Migrations
{
    /// <inheritdoc />
    public partial class changeZoneTotalPeopleColName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPeople",
                table: "EvacuationZones",
                newName: "NumberOfPeople");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfPeople",
                table: "EvacuationZones",
                newName: "TotalPeople");
        }
    }
}
