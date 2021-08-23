using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaDiemLayPhoi",
                table: "Phoi");

            migrationBuilder.DropColumn(
                name: "NgayCapNhat",
                table: "Phoi");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhat",
                table: "Phoi");

            migrationBuilder.DropColumn(
                name: "TenNguoiLayPhoi",
                table: "Phoi");

            migrationBuilder.DropColumn(
                name: "ThoiGianLayPhoi",
                table: "Phoi");

            migrationBuilder.RenameColumn(
                name: "MoTa",
                table: "Phoi",
                newName: "MoTaTrangThai");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MoTaTrangThai",
                table: "Phoi",
                newName: "MoTa");

            migrationBuilder.AddColumn<string>(
                name: "DiaDiemLayPhoi",
                table: "Phoi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhat",
                table: "Phoi",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NguoiCapNhat",
                table: "Phoi",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenNguoiLayPhoi",
                table: "Phoi",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianLayPhoi",
                table: "Phoi",
                type: "datetime",
                nullable: true);
        }
    }
}
