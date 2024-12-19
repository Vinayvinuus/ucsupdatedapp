using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ucsUpdatedApp.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MasterTable",
                columns: table => new
                {
                    MasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    Employeename = table.Column<string>(type: "text", nullable: false),
                    FingerPrintData = table.Column<string>(type: "text", nullable: true),
                    LastTransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterTable", x => x.MasterId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    Op = table.Column<int>(type: "integer", nullable: false),
                    OpDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckInMethod = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionTable_MasterTable_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "MasterTable",
                        principalColumn: "MasterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTable_EmployeeId",
                table: "TransactionTable",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionTable");

            migrationBuilder.DropTable(
                name: "MasterTable");
        }
    }
}
