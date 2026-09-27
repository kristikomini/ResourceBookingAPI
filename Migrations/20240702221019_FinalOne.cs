using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceBooking.Migrations
{
    /// <inheritdoc />
    public partial class FinalOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentInfo_ResourceInfo_ResourceId",
                table: "StudentInfo"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_StudentInfo_UserInfo_UserId",
                table: "StudentInfo"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_StudentInfo", table: "StudentInfo");

            migrationBuilder.RenameTable(name: "StudentInfo", newName: "BookingInfo");

            migrationBuilder.RenameIndex(
                name: "IX_StudentInfo_UserId",
                table: "BookingInfo",
                newName: "IX_BookingInfo_UserId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_StudentInfo_ResourceId",
                table: "BookingInfo",
                newName: "IX_BookingInfo_ResourceId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingInfo",
                table: "BookingInfo",
                column: "BookingId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_BookingInfo_ResourceInfo_ResourceId",
                table: "BookingInfo",
                column: "ResourceId",
                principalTable: "ResourceInfo",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_BookingInfo_UserInfo_UserId",
                table: "BookingInfo",
                column: "UserId",
                principalTable: "UserInfo",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingInfo_ResourceInfo_ResourceId",
                table: "BookingInfo"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_BookingInfo_UserInfo_UserId",
                table: "BookingInfo"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_BookingInfo", table: "BookingInfo");

            migrationBuilder.RenameTable(name: "BookingInfo", newName: "StudentInfo");

            migrationBuilder.RenameIndex(
                name: "IX_BookingInfo_UserId",
                table: "StudentInfo",
                newName: "IX_StudentInfo_UserId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_BookingInfo_ResourceId",
                table: "StudentInfo",
                newName: "IX_StudentInfo_ResourceId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentInfo",
                table: "StudentInfo",
                column: "BookingId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_StudentInfo_ResourceInfo_ResourceId",
                table: "StudentInfo",
                column: "ResourceId",
                principalTable: "ResourceInfo",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_StudentInfo_UserInfo_UserId",
                table: "StudentInfo",
                column: "UserId",
                principalTable: "UserInfo",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
