using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ucsUpdatedApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDOBAndDOJToMasterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DOB",
                table: "MasterTable",
                type: "DATE",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DOJ",
                table: "MasterTable",
                type: "DATE",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DOB",
                table: "MasterTable");

            migrationBuilder.DropColumn(
                name: "DOJ",
                table: "MasterTable");
        }
    }
}
