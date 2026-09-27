using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceBooking.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourceTypeInfo",
                columns: table => new
                {
                    ResourceTypeId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceTypeInfo", x => x.ResourceTypeId);
                }
            );

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    UserId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.UserId);
                }
            );

            migrationBuilder.CreateTable(
                name: "ResourceInfo",
                columns: table => new
                {
                    ResourceId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResourceTypeId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceInfo", x => x.ResourceId);
                    table.ForeignKey(
                        name: "FK_ResourceInfo_ResourceTypeInfo_ResourceTypeId",
                        column: x => x.ResourceTypeId,
                        principalTable: "ResourceTypeInfo",
                        principalColumn: "ResourceTypeId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "StudentInfo",
                columns: table => new
                {
                    BookingId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentInfo", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_StudentInfo_ResourceInfo_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "ResourceInfo",
                        principalColumn: "ResourceId",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_StudentInfo_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ResourceInfo_ResourceTypeId",
                table: "ResourceInfo",
                column: "ResourceTypeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTypeInfo_TypeName",
                table: "ResourceTypeInfo",
                column: "TypeName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_StudentInfo_ResourceId",
                table: "StudentInfo",
                column: "ResourceId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_StudentInfo_UserId",
                table: "StudentInfo",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_Email",
                table: "UserInfo",
                column: "Email",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "StudentInfo");

            migrationBuilder.DropTable(name: "ResourceInfo");

            migrationBuilder.DropTable(name: "UserInfo");

            migrationBuilder.DropTable(name: "ResourceTypeInfo");
        }
    }
}
