using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPasswordOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UserPasswordOptions_RequireDigit",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UserPasswordOptions_RequireLowercase",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UserPasswordOptions_RequireNonAlphanumeric",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UserPasswordOptions_RequireUppercase",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserPasswordOptions_RequiredPasswordLength",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserPasswordOptions_RequiredUniqueChars",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPasswordOptions_RequireDigit",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserPasswordOptions_RequireLowercase",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserPasswordOptions_RequireNonAlphanumeric",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserPasswordOptions_RequireUppercase",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserPasswordOptions_RequiredPasswordLength",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserPasswordOptions_RequiredUniqueChars",
                table: "AspNetUsers");
        }
    }
}
