using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace evacPlanMoni.Migrations
{
    /// <inheritdoc />
    public partial class setSearchPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Sets the default schema for all entities in the model
            // This is only for postgres. In appsettings connection string also need `SearchPath=public;`.
            migrationBuilder.Sql("ALTER DATABASE \"EvacuationDb\" SET search_path TO public, \"$user\";");
            // apply to the specific user
            migrationBuilder.Sql("ALTER ROLE \"postgres\" SET search_path TO public, \"$user\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Revert
            migrationBuilder.Sql("ALTER DATABASE \"EvacuationDb\" RESET search_path;");
            migrationBuilder.Sql("ALTER ROLE \"postgres\" RESET search_path;");
        }
    }
}
