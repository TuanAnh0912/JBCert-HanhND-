using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class hanhnd_10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnhYeuCauPhatBangs",
                columns: table => new
                {
                    AnhYeuCauPhatBangId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DuongDanAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YeuCauPhatBangId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnhYeuCauPhatBangs", x => x.AnhYeuCauPhatBangId);
                });

            migrationBuilder.CreateTable(
                name: "TrangThaiYeuCauPhatBangs",
                columns: table => new
                {
                    TrangThaiYeuCauPhatBangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTrangThaiYeuCauPhatBang = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrangThaiYeuCauPhatBangs", x => x.TrangThaiYeuCauPhatBangId);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauPhatBangs",
                columns: table => new
                {
                    YeuCauPhatBangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BangId = table.Column<int>(type: "int", nullable: false),
                    Mota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTaoYeuCau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiYeuCauPhatBangId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauPhatBangs", x => x.YeuCauPhatBangId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnhYeuCauPhatBangs");

            migrationBuilder.DropTable(
                name: "TrangThaiYeuCauPhatBangs");

            migrationBuilder.DropTable(
                name: "YeuCauPhatBangs");
        }
    }
}
