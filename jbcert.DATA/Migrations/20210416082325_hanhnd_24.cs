using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class hanhnd_24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiemThi",
                table: "BangCus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoiDongThi",
                table: "BangCus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiemThi",
                table: "Bang",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoiDongThi",
                table: "Bang",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiemThi",
                table: "BangCus");

            migrationBuilder.DropColumn(
                name: "HoiDongThi",
                table: "BangCus");

            migrationBuilder.DropColumn(
                name: "DiemThi",
                table: "Bang");

            migrationBuilder.DropColumn(
                name: "HoiDongThi",
                table: "Bang");
        }
    }
}
