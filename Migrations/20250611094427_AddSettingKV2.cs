using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingKV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "Settings",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Settings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "AdminSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AdminSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "AdminSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "AdminSettings");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AdminSettings");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "AdminSettings");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Settings",
                newName: "Tax");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Settings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Settings",
                type: "TEXT",
                nullable: true);
        }
    }
}
