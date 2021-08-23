﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace jbcert.DATA.Migrations
{
    public partial class addfiledinhkemloaibang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileDinhKemLoaiBangs",
                columns: table => new
                {
                    FileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiBangId = table.Column<int>(type: "int", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiTao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ext = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDinhKemLoaiBangs", x => x.FileId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileDinhKemLoaiBangs");
        }
    }
}
