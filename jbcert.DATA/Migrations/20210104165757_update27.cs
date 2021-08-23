using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class update27 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "ThongBaos");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "ThongBaoTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThongBaoTypeId",
                table: "ThongBaos",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "ThongBaoTypes");

            migrationBuilder.DropColumn(
                name: "ThongBaoTypeId",
                table: "ThongBaos");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "ThongBaos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
