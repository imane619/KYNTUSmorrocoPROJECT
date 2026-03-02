using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftMaster.Employee.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeForEquityScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmploymentType",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "EquityScore",
                table: "Employees",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PreavisReduction",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmploymentType",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EquityScore",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PreavisReduction",
                table: "Employees");
        }
    }
}
