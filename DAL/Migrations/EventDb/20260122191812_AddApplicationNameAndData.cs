using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class AddApplicationNameAndData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationName",
                table: "ApplicationEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "ApplicationEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationName",
                table: "ApplicationEvents");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "ApplicationEvents");
        }
    }
}
