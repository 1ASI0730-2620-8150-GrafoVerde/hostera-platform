using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HosteraBackend.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            // Existing users are operations managers of their organization until someone changes their role
            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "users",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "OperationsManager");

            // Existing users show their username as full name until they get a real one
            migrationBuilder.Sql("UPDATE users SET full_name = username WHERE full_name = '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "full_name",
                table: "users");

            migrationBuilder.DropColumn(
                name: "role",
                table: "users");
        }
    }
}
