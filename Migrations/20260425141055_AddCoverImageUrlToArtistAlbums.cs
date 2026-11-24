using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextVibe.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverImageUrlToArtistAlbums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "ArtistAlbums",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "ArtistAlbums");
        }
    }
}
