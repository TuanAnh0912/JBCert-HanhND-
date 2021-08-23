using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class updatelkbanghocsinh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bang_HocSinh",
                table: "Bang");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Bang_HocSinh",
                table: "Bang",
                column: "HocSinhId",
                principalTable: "HocSinh",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
