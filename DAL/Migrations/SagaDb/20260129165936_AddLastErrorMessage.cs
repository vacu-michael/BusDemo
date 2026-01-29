using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations.SagaDb
{
    /// <inheritdoc />
    public partial class AddLastErrorMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkflowCorrelationId",
                table: "ApplicationWorkflowStates");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                table: "ApplicationWorkflowStates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ApplicationWorkflowStates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastErrorMessage",
                table: "ApplicationWorkflowStates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ApplicationWorkflowStates");

            migrationBuilder.DropColumn(
                name: "LastErrorMessage",
                table: "ApplicationWorkflowStates");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                table: "ApplicationWorkflowStates",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkflowCorrelationId",
                table: "ApplicationWorkflowStates",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
