using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class _333333 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoaiBangId",
                table: "HocSinhTrongNhomTaoVanBangs",
                newName: "TrangThaiBangId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrangThaiBangId",
                table: "HocSinhTrongNhomTaoVanBangs",
                newName: "LoaiBangId");
        }
    }
}
