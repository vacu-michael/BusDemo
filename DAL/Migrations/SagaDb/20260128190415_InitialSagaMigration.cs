using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations.SagaDb
{
    /// <inheritdoc />
    public partial class InitialSagaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationWorkflowStates",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ValidateNameTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateAccountTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LinkAccountTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkflowCorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationWorkflowStates", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationWorkflowStates");
        }
    }
}
