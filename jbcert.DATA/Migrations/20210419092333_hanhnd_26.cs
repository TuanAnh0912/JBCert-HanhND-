using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class hanhnd_26 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoHoa_Bangs");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "SoHoas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoHoaId",
                table: "Bang",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "SoHoas");

            migrationBuilder.DropColumn(
                name: "SoHoaId",
                table: "Bang");

            migrationBuilder.CreateTable(
                name: "SoHoa_Bangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BangId = table.Column<int>(type: "int", nullable: true),
                    SoHoaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoHoa_Bangs", x => x.Id);
                });
        }
    }
}
