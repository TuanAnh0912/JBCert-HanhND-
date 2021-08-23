using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class hanhnd_13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnhYeuCauPhatBangs",
                columns: table => new
                {
                    AnhYeuCauPhatBangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DuongDanAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YeuCauPhatBangId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnhYeuCauPhatBangs", x => x.AnhYeuCauPhatBangId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnhYeuCauPhatBangs");
        }
    }
}
