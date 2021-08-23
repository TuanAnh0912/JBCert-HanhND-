using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HoVaTen",
                table: "Bang",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoHieu",
                table: "Bang",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoVaoSo",
                table: "Bang",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TruongHoc",
                table: "Bang",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoVaTen",
                table: "Bang");

            migrationBuilder.DropColumn(
                name: "SoHieu",
                table: "Bang");

            migrationBuilder.DropColumn(
                name: "SoVaoSo",
                table: "Bang");

            migrationBuilder.DropColumn(
                name: "TruongHoc",
                table: "Bang");
        }
    }
}
