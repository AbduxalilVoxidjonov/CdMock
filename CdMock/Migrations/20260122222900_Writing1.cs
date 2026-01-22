using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CdMock.Migrations
{
    /// <inheritdoc />
    public partial class Writing1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WritingTasks",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TaskType = table.Column<int>(type: "int", nullable: false),
                    Task1ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Task1ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Task2ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Task2ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Task1Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Task2Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Task1Instructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Task2Instructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TimeLimit = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MockId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingTasks", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_WritingTasks_Mocks_MockId",
                        column: x => x.MockId,
                        principalTable: "Mocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WritingTasks_MockId",
                table: "WritingTasks",
                column: "MockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WritingTasks");
        }
    }
}
