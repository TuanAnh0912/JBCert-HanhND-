using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class update8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThongTinCaiChinh",
                table: "CaiChinhs");

            migrationBuilder.CreateTable(
                name: "ThongTinCaiChinhs",
                columns: table => new
                {
                    ThongTinCaiChinhId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaiChinhId = table.Column<int>(type: "int", nullable: false),
                    ThongTinThayDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTinCu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTinMoi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinCaiChinhs", x => x.ThongTinCaiChinhId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongTinCaiChinhs");

            migrationBuilder.AddColumn<string>(
                name: "ThongTinCaiChinh",
                table: "CaiChinhs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
