using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceBooking.Migrations
{
    /// <inheritdoc />
    public partial class BookingOverlapIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_BookingInfo_ResourceId", table: "BookingInfo");

            migrationBuilder.CreateIndex(
                name: "IX_BookingInfo_ResourceId_StartDate_EndDate",
                table: "BookingInfo",
                columns: new[] { "ResourceId", "StartDate", "EndDate" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingInfo_ResourceId_StartDate_EndDate",
                table: "BookingInfo"
            );

            migrationBuilder.CreateIndex(
                name: "IX_BookingInfo_ResourceId",
                table: "BookingInfo",
                column: "ResourceId"
            );
        }
    }
}
