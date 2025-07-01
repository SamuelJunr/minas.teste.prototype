using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace minas.teste.prototype.Migrations
{
    public partial class AddSessionAndSensorDataTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorDataDB_SensoresConfiguracao_SensorConfiguracaoID",
                table: "SensorDataDB");

            migrationBuilder.DropIndex(
                name: "IX_SensorDataDB_SensorConfiguracaoID",
                table: "SensorDataDB");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "SensorDataDB");

            migrationBuilder.DropColumn(
                name: "Sensor",
                table: "SensorDataDB");

            migrationBuilder.DropColumn(
                name: "SensorConfiguracaoID",
                table: "SensorDataDB");

            migrationBuilder.DropColumn(
                name: "TerminateTime",
                table: "SensorDataDB");

            migrationBuilder.DropColumn(
                name: "UpdateTime",
                table: "SensorDataDB");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "SensorDataDB");

            migrationBuilder.RenameColumn(
                name: "Medida",
                table: "SensorDataDB",
                newName: "Unit");

            migrationBuilder.AddColumn<double>(
                name: "DuracaoMinutos",
                table: "Sessoes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeBombaTextBox",
                table: "Sessoes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeClienteTextBox",
                table: "Sessoes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrdemServicoTextBox",
                table: "Sessoes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SensorName",
                table: "SensorDataDB",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "SensorDataDB",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Value",
                table: "SensorDataDB",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SessaoID",
                table: "Etapas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Etapas_SessaoID",
                table: "Etapas",
                column: "SessaoID");

            migrationBuilder.AddForeignKey(
                name: "FK_Etapas_Sessoes_SessaoID",
                table: "Etapas",
                column: "SessaoID",
                principalTable: "Sessoes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etapas_Sessoes_SessaoID",
                table: "Etapas");

            migrationBuilder.DropIndex(
                name: "IX_Etapas_SessaoID",
                table: "Etapas");

            migrationBuilder.DropColumn(
                name: "DuracaoMinutos",
                table: "Sessoes");

            migrationBuilder.DropColumn(
                name: "NomeBombaTextBox",
                table: "Sessoes");

            migrationBuilder.DropColumn(
                name: "NomeClienteTextBox",
                table: "Sessoes");

            migrationBuilder.DropColumn(
                name: "OrdemServicoTextBox",
                table: "Sessoes");

            migrationBuilder.DropColumn(
                name: "SensorName",
                table: "SensorDataDB");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "SensorDataDB");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "SensorDataDB");

            migrationBuilder.DropColumn(
                name: "SessaoID",
                table: "Etapas");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "SensorDataDB",
                newName: "Medida");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "SensorDataDB",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Sensor",
                table: "SensorDataDB",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SensorConfiguracaoID",
                table: "SensorDataDB",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TerminateTime",
                table: "SensorDataDB",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                table: "SensorDataDB",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<double>(
                name: "Valor",
                table: "SensorDataDB",
                type: "REAL",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SensorDataDB_SensorConfiguracaoID",
                table: "SensorDataDB",
                column: "SensorConfiguracaoID");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorDataDB_SensoresConfiguracao_SensorConfiguracaoID",
                table: "SensorDataDB",
                column: "SensorConfiguracaoID",
                principalTable: "SensoresConfiguracao",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
