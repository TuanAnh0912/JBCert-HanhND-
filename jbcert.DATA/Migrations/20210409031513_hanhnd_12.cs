using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class hanhnd_12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnhYeuCauPhatBangs");

            migrationBuilder.AddColumn<int>(
                name: "DonViId",
                table: "YeuCauPhatBangs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonViId",
                table: "YeuCauPhatBangs");

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
        }
    }
}
