using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class hanhnd_28 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoGocFileDinhKem",
                columns: table => new
                {
                    FileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoHoaId = table.Column<int>(type: "int", nullable: true),
                    NguoiTao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DonViId = table.Column<int>(type: "int", nullable: true),
                    Ext = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MimeTypes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoGocFileDinhKem", x => x.FileId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoGocFileDinhKem");
        }
    }
}
