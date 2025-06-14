﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testbills.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceDatacomment1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Invoices",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Invoices");
        }
    }
}
