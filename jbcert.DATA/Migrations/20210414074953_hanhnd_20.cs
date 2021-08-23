using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class hanhnd_20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DuongDanFile",
                table: "FileHocSinhYeuCaus",
                newName: "Url");

            migrationBuilder.AlterColumn<int>(
                name: "YeuCauId",
                table: "FileHocSinhYeuCaus",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DonViId",
                table: "FileHocSinhYeuCaus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ext",
                table: "FileHocSinhYeuCaus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconFile",
                table: "FileHocSinhYeuCaus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MimeTypes",
                table: "FileHocSinhYeuCaus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "FileHocSinhYeuCaus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NguoiTao",
                table: "FileHocSinhYeuCaus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenFile",
                table: "FileHocSinhYeuCaus",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonViId",
                table: "FileHocSinhYeuCaus");

            migrationBuilder.DropColumn(
                name: "Ext",
                table: "FileHocSinhYeuCaus");

            migrationBuilder.DropColumn(
                name: "IconFile",
                table: "FileHocSinhYeuCaus");

            migrationBuilder.DropColumn(
                name: "MimeTypes",
                table: "FileHocSinhYeuCaus");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "FileHocSinhYeuCaus");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "FileHocSinhYeuCaus");

            migrationBuilder.DropColumn(
                name: "TenFile",
                table: "FileHocSinhYeuCaus");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "FileHocSinhYeuCaus",
                newName: "DuongDanFile");

            migrationBuilder.AlterColumn<int>(
                name: "YeuCauId",
                table: "FileHocSinhYeuCaus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
