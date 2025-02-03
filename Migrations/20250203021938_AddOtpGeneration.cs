using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LastGeneratedX",
                table: "AspNetUsers",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LastGeneratedY",
                table: "AspNetUsers",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastGeneratedX",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastGeneratedY",
                table: "AspNetUsers");
        }
    }
}
