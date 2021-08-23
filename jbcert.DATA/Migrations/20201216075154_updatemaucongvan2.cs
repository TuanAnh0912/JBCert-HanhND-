using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class updatemaucongvan2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoaiYeuCau",
                table: "MauCongVans",
                newName: "LoaiYeuCauId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoaiYeuCauId",
                table: "MauCongVans",
                newName: "LoaiYeuCau");
        }
    }
}
