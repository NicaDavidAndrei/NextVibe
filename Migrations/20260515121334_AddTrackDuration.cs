using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextVibe.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "AlbumTracks",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "AlbumTracks");
        }
    }
}
