using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ucsUpdatedApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionTableLatestTransactionDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionTable_MasterTable_EmployeeId",
                table: "TransactionTable");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "TransactionTable",
                newName: "MasterId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionTable_EmployeeId",
                table: "TransactionTable",
                newName: "IX_TransactionTable_MasterId");

            migrationBuilder.AddColumn<DateTime>(
                name: "LatestTransactionDate",
                table: "TransactionTable",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionTable_MasterTable_MasterId",
                table: "TransactionTable",
                column: "MasterId",
                principalTable: "MasterTable",
                principalColumn: "MasterId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionTable_MasterTable_MasterId",
                table: "TransactionTable");

            migrationBuilder.DropColumn(
                name: "LatestTransactionDate",
                table: "TransactionTable");

            migrationBuilder.RenameColumn(
                name: "MasterId",
                table: "TransactionTable",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionTable_MasterId",
                table: "TransactionTable",
                newName: "IX_TransactionTable_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionTable_MasterTable_EmployeeId",
                table: "TransactionTable",
                column: "EmployeeId",
                principalTable: "MasterTable",
                principalColumn: "MasterId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
