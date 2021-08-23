using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class _333 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HocSinhTrongNhomTaoVanBangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhomTaoVanBangId = table.Column<int>(type: "int", nullable: false),
                    HocSinhId = table.Column<int>(type: "int", nullable: true),
                    BangId = table.Column<int>(type: "int", nullable: true),
                    LoaiBangId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocSinhTrongNhomTaoVanBangs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhomTaoVanBangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TruongHocId = table.Column<int>(type: "int", nullable: true),
                    DonViId = table.Column<int>(type: "int", nullable: true),
                    LoaiBang = table.Column<int>(type: "int", nullable: true),
                    TrangThaiBang = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiTao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiCapNhat = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomTaoVanBangs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HocSinhTrongNhomTaoVanBangs");

            migrationBuilder.DropTable(
                name: "NhomTaoVanBangs");
        }
    }
}
