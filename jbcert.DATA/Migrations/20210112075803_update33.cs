using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class update33 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuanHeVoiNguoiDuocCapBang",
                table: "BangCus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuanHeVoiNguoiDuocCapBang",
                table: "Bang",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuanHeVoiNguoiDuocCapBang",
                table: "BangCus");

            migrationBuilder.DropColumn(
                name: "QuanHeVoiNguoiDuocCapBang",
                table: "Bang");
        }
    }
}
