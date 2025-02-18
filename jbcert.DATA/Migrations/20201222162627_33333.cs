﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class _33333 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThaiBang",
                table: "NhomTaoVanBangs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayTao",
                table: "NhomTaoVanBangs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiBangId",
                table: "NhomTaoVanBangs",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThaiBangId",
                table: "NhomTaoVanBangs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayTao",
                table: "NhomTaoVanBangs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiBang",
                table: "NhomTaoVanBangs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
