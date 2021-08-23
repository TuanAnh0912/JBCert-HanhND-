using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class update14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThongTinVanBangCus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TruongDuLieuCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BangId = table.Column<int>(type: "int", nullable: false),
                    GiaTri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiTao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiCapNhat = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonViId = table.Column<int>(type: "int", nullable: false),
                    CaiChinhOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinVanBangCus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThongTinVanBangCus_BangCus_BangId",
                        column: x => x.BangId,
                        principalTable: "BangCus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinVanBangCus_BangId",
                table: "ThongTinVanBangCus",
                column: "BangId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongTinVanBangCus");
        }
    }
}
