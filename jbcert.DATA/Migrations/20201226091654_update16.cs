using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class update16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoLanCaiChinh",
                table: "Bang",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLanCaiChinh",
                table: "Bang");
        }
    }
}
