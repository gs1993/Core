using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class Add_Order_Status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentCanceledDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentErrorDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentInProgressDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentStartDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentSuccededDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentCanceledDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentErrorDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentInProgressDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentStartDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentSuccededDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Orders");
        }
    }
}
