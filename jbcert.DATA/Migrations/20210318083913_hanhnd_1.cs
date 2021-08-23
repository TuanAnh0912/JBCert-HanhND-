using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class hanhnd_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChiSoCoDinh",
                table: "Phoi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChiSoThayDoi",
                table: "Phoi",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DonViDaNhan",
                table: "Phoi",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiSoCoDinh",
                table: "Phoi");

            migrationBuilder.DropColumn(
                name: "ChiSoThayDoi",
                table: "Phoi");

            migrationBuilder.DropColumn(
                name: "DonViDaNhan",
                table: "Phoi");
        }
    }
}
