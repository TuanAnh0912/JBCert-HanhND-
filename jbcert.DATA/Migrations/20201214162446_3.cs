using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class _3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Border",
                table: "TrangThaiPhoi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaMau",
                table: "TrangThaiPhoi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MauChu",
                table: "TrangThaiPhoi",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Border",
                table: "TrangThaiPhoi");

            migrationBuilder.DropColumn(
                name: "MaMau",
                table: "TrangThaiPhoi");

            migrationBuilder.DropColumn(
                name: "MauChu",
                table: "TrangThaiPhoi");
        }
    }
}
