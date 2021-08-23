using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class update7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BangCus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiBangId = table.Column<int>(type: "int", nullable: true),
                    PhoiId = table.Column<int>(type: "int", nullable: true),
                    TrangThaiBangId = table.Column<int>(type: "int", nullable: true),
                    HocSinhId = table.Column<int>(type: "int", nullable: true),
                    TruongHocId = table.Column<int>(type: "int", nullable: true),
                    YeuCauId = table.Column<int>(type: "int", nullable: true),
                    SoVaoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TruongHoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoHieu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuongDanFileAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuongDanFileDeIn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CmtnguoiLayBang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoaiNguoiLayBang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhThucNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViId = table.Column<int>(type: "int", nullable: true),
                    NgayInBang = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayPhatBang = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiTao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiCapNhat = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangCus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaiChinhs",
                columns: table => new
                {
                    CaiChinhId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanCaiChinh = table.Column<int>(type: "int", nullable: false),
                    BangId = table.Column<int>(type: "int", nullable: false),
                    BangCuId = table.Column<int>(type: "int", nullable: false),
                    NguoiThucHien = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiThucHien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViThucHien = table.Column<int>(type: "int", nullable: false),
                    ThoiGianThucHien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThongTinCaiChinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDoCaiChinh = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaiChinhs", x => x.CaiChinhId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangCus");

            migrationBuilder.DropTable(
                name: "CaiChinhs");
        }
    }
}
