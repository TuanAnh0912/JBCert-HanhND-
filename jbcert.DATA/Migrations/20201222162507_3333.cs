using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class _3333 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoaiBang",
                table: "NhomTaoVanBangs",
                newName: "LoaiBangId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoaiBangId",
                table: "NhomTaoVanBangs",
                newName: "LoaiBang");
        }
    }
}
