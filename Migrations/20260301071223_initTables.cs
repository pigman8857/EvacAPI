using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace evacPlanMoni.Migrations
{
    /// <inheritdoc />
    public partial class initTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvacuationPlans",
                columns: table => new
                {
                    ZoneId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VehicleId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ETAHours = table.Column<double>(type: "double precision", nullable: false),
                    PeopleToEvacuate = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvacuationPlans", x => new { x.ZoneId, x.VehicleId });
                });

            migrationBuilder.CreateTable(
                name: "EvacuationStatuses",
                columns: table => new
                {
                    ZoneId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalEvacuated = table.Column<int>(type: "integer", nullable: false),
                    RemainingPeople = table.Column<int>(type: "integer", nullable: false),
                    LastVehicleUsed = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvacuationStatuses", x => x.ZoneId);
                });

            migrationBuilder.CreateTable(
                name: "EvacuationZones",
                columns: table => new
                {
                    ZoneId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    TotalPeople = table.Column<int>(type: "integer", nullable: false),
                    UrgencyLevel = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvacuationZones", x => x.ZoneId);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Speed = table.Column<double>(type: "double precision", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvacuationZones_UrgencyLevel",
                table: "EvacuationZones",
                column: "UrgencyLevel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvacuationPlans");

            migrationBuilder.DropTable(
                name: "EvacuationStatuses");

            migrationBuilder.DropTable(
                name: "EvacuationZones");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
