using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDatabase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventAttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Event = table.Column<bool>(type: "bit", nullable: true),
                    Program = table.Column<bool>(type: "bit", nullable: true),
                    Cafe = table.Column<bool>(type: "bit", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AttendanceCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsFlagged = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendanceRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendanceRecords_Date",
                table: "EventAttendanceRecords",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendanceRecords_EventName",
                table: "EventAttendanceRecords",
                column: "EventName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAttendanceRecords");
        }
    }
}
