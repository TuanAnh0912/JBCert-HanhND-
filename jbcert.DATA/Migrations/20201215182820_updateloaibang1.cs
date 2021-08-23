using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class updateloaibang1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapDonViId",
                table: "LoaiBang");

            migrationBuilder.AddColumn<string>(
                name: "CodeCapDonVi",
                table: "LoaiBang",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeCapDonVi",
                table: "LoaiBang");

            migrationBuilder.AddColumn<int>(
                name: "CapDonViId",
                table: "LoaiBang",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
