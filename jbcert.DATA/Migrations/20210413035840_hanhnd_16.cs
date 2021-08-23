using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class hanhnd_16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DuongDanFileAnh",
                table: "NhomTaoVanBangs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DuongDanFileAnhDeIn",
                table: "NhomTaoVanBangs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuongDanFileAnh",
                table: "NhomTaoVanBangs");

            migrationBuilder.DropColumn(
                name: "DuongDanFileAnhDeIn",
                table: "NhomTaoVanBangs");
        }
    }
}
