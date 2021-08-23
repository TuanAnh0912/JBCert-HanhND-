using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class hanhnd_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsChungChi",
                table: "LoaiBang",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsChungChi",
                table: "BangCus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsChungChi",
                table: "Bang",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChungChi",
                table: "LoaiBang");

            migrationBuilder.DropColumn(
                name: "IsChungChi",
                table: "BangCus");

            migrationBuilder.DropColumn(
                name: "IsChungChi",
                table: "Bang");
        }
    }
}
