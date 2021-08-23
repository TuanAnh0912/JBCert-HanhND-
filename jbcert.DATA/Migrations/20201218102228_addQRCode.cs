using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class addQRCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "TruongDuLieuLoaiBang",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KieuDuLieu",
                table: "TruongDuLieuLoaiBang",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "TruongDuLieuLoaiBang",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KieuDuLieu",
                table: "TruongDuLieu",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "TruongDuLieuLoaiBang");

            migrationBuilder.DropColumn(
                name: "KieuDuLieu",
                table: "TruongDuLieuLoaiBang");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "TruongDuLieuLoaiBang");

            migrationBuilder.DropColumn(
                name: "KieuDuLieu",
                table: "TruongDuLieu");
        }
    }
}
