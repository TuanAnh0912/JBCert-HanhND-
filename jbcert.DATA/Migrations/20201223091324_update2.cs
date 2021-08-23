using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ChoPhepTaoLai",
                table: "NhomTaoVanBangs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TongSohocSinh",
                table: "NhomTaoVanBangs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DonViId",
                table: "HocSinhTrongNhomTaoVanBangs",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChoPhepTaoLai",
                table: "NhomTaoVanBangs");

            migrationBuilder.DropColumn(
                name: "TongSohocSinh",
                table: "NhomTaoVanBangs");

            migrationBuilder.DropColumn(
                name: "DonViId",
                table: "HocSinhTrongNhomTaoVanBangs");
        }
    }
}
