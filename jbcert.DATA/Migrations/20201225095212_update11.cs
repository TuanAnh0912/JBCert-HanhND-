using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class update11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Loai",
                table: "NhomTaoVanBangs",
                newName: "LoaiNhomTaoVanBangId");

            migrationBuilder.CreateTable(
                name: "LoaiNhomTaoVanBangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiNhomTaoVanBangs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoaiNhomTaoVanBangs");

            migrationBuilder.RenameColumn(
                name: "LoaiNhomTaoVanBangId",
                table: "NhomTaoVanBangs",
                newName: "Loai");
        }
    }
}
