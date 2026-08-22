using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameOutboxColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccurredOnUTC",
                table: "OutBoxMessages",
                newName: "OccurredOnUtc");

            migrationBuilder.RenameColumn(
                name: "ProceedInUTC",
                table: "OutBoxMessages",
                newName: "ProcessedOnUtc");

            migrationBuilder.RenameIndex(
                name: "IX_OutBoxMessages_ProceedInUTC",
                table: "OutBoxMessages",
                newName: "IX_OutBoxMessages_ProcessedOnUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccurredOnUtc",
                table: "OutBoxMessages",
                newName: "OccurredOnUTC");

            migrationBuilder.RenameColumn(
                name: "ProcessedOnUtc",
                table: "OutBoxMessages",
                newName: "ProceedInUTC");

            migrationBuilder.RenameIndex(
                name: "IX_OutBoxMessages_ProcessedOnUtc",
                table: "OutBoxMessages",
                newName: "IX_OutBoxMessages_ProceedInUTC");
        }
    }
}
