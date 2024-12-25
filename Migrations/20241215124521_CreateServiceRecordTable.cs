using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateServiceRecordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "ServiceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonnelName",
                table: "ServiceRecords",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "ServiceRecords");

            migrationBuilder.DropColumn(
                name: "PersonnelName",
                table: "ServiceRecords");
        }
    }
}
