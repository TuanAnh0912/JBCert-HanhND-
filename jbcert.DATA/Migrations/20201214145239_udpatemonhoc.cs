using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class udpatemonhoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonViId",
                table: "MonHocs");

            migrationBuilder.AddColumn<string>(
                name: "CodeCapDonVi",
                table: "MonHocs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeCapDonVi",
                table: "MonHocs");

            migrationBuilder.AddColumn<int>(
                name: "DonViId",
                table: "MonHocs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
