using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBusinessTypeTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remover a constraint antiga que aponta para a tabela incorreta
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_BussinessType_BussinessTypeId",
                table: "Businesses");

            // Recriar a constraint apontando para a tabela correta BusinessTypes
            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_BusinessTypes_BussinessTypeId",
                table: "Businesses",
                column: "BussinessTypeId",
                principalTable: "BusinessTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Remover a tabela duplicada incorreta (se existir)
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.BussinessType', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [dbo].[BussinessType];
                END
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_ComissionPayment_Comissions_ComissionId",
                table: "ComissionPayment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComissionPayment",
                table: "ComissionPayment");

            migrationBuilder.RenameTable(
                name: "ComissionPayment",
                newName: "ComissionPayments");

            migrationBuilder.RenameIndex(
                name: "IX_ComissionPayment_ComissionId",
                table: "ComissionPayments",
                newName: "IX_ComissionPayments_ComissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComissionPayments",
                table: "ComissionPayments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComissionPayments_Comissions_ComissionId",
                table: "ComissionPayments",
                column: "ComissionId",
                principalTable: "Comissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverter as alterações do ComissionPayment
            migrationBuilder.DropForeignKey(
                name: "FK_ComissionPayments_Comissions_ComissionId",
                table: "ComissionPayments");

            // Reverter a constraint de BusinessTypes
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_BusinessTypes_BussinessTypeId",
                table: "Businesses");

            // Recriar a constraint antiga
            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_BussinessType_BussinessTypeId",
                table: "Businesses",
                column: "BussinessTypeId",
                principalTable: "BussinessType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropForeignKey(
                name: "FK_ComissionPayments_Comissions_ComissionId",
                table: "ComissionPayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComissionPayments",
                table: "ComissionPayments");

            migrationBuilder.RenameTable(
                name: "ComissionPayments",
                newName: "ComissionPayment");

            migrationBuilder.RenameIndex(
                name: "IX_ComissionPayments_ComissionId",
                table: "ComissionPayment",
                newName: "IX_ComissionPayment_ComissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComissionPayment",
                table: "ComissionPayment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComissionPayment_Comissions_ComissionId",
                table: "ComissionPayment",
                column: "ComissionId",
                principalTable: "Comissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
