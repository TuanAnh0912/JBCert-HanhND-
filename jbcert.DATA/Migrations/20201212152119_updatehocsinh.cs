using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class updatehocsinh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CongNhanTotNghiep",
                table: "HocSinh",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoLanXet",
                table: "HocSinh",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CongNhanTotNghiep",
                table: "HocSinh");

            migrationBuilder.DropColumn(
                name: "SoLanXet",
                table: "HocSinh");
        }
    }
}
