using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace minas.teste.prototype.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CNPJ = table.Column<string>(type: "TEXT", nullable: false),
                    Endereco = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Telefone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TemposExecucao",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Duracao = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemposExecucao", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CNPJ = table.Column<string>(type: "TEXT", nullable: true),
                    CPF = table.Column<string>(type: "TEXT", nullable: true),
                    Endereco = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Telefone = table.Column<string>(nullable: true),
                    EmpresaID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Clientes_Empresas_EmpresaID",
                        column: x => x.EmpresaID,
                        principalTable: "Empresas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    EmpresaID = table.Column<int>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Usuarios_Empresas_EmpresaID",
                        column: x => x.EmpresaID,
                        principalTable: "Empresas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Monitoramentos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    PSImin = table.Column<double>(nullable: true),
                    BARmin = table.Column<double>(nullable: true),
                    GPMmin = table.Column<double>(nullable: true),
                    LPMmin = table.Column<double>(nullable: true),
                    RPMmin = table.Column<double>(nullable: true),
                    CELSUSmin = table.Column<double>(nullable: true),
                    PSImax = table.Column<double>(nullable: true),
                    BARmax = table.Column<double>(nullable: true),
                    GPMmax = table.Column<double>(nullable: true),
                    LPMmax = table.Column<double>(nullable: true),
                    RPMmax = table.Column<double>(nullable: true),
                    CELSUSmax = table.Column<double>(nullable: true),
                    ModuloID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monitoramentos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Monitoramentos_Modulos_ModuloID",
                        column: x => x.ModuloID,
                        principalTable: "Modulos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Imagens",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Dado = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Notas = table.Column<string>(type: "TEXT", nullable: true),
                    EmpresaID = table.Column<int>(nullable: true),
                    ClienteID = table.Column<int>(nullable: true),
                    RelatorioID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Imagens_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Imagens_Empresas_EmpresaID",
                        column: x => x.EmpresaID,
                        principalTable: "Empresas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Calibracoes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Sensor = table.Column<string>(type: "TEXT", nullable: false),
                    DataCalibracao = table.Column<DateTime>(nullable: true),
                    ValorOffset = table.Column<double>(nullable: true),
                    ValorGanho = table.Column<double>(nullable: true),
                    LeituraMin = table.Column<double>(nullable: true),
                    LeituraMax = table.Column<double>(nullable: true),
                    ValorRefMin = table.Column<double>(nullable: true),
                    ValorRefMax = table.Column<double>(nullable: true),
                    Unidade = table.Column<string>(type: "TEXT", nullable: true),
                    Notas = table.Column<string>(type: "TEXT", nullable: true),
                    UsuarioID = table.Column<int>(nullable: true),
                    ModuloID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calibracoes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Calibracoes_Modulos_ModuloID",
                        column: x => x.ModuloID,
                        principalTable: "Modulos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Calibracoes_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Etapas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Modulo = table.Column<string>(type: "TEXT", nullable: true),
                    Ordem = table.Column<int>(nullable: true),
                    Modelo = table.Column<string>(type: "TEXT", nullable: true),
                    EmpresaID = table.Column<int>(nullable: true),
                    UsuarioID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etapas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Etapas_Empresas_EmpresaID",
                        column: x => x.EmpresaID,
                        principalTable: "Empresas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Etapas_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessoes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    ModuloID = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ClienteID = table.Column<int>(nullable: true),
                    EmpresaID = table.Column<int>(nullable: true),
                    UsuarioID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessoes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Sessoes_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessoes_Empresas_EmpresaID",
                        column: x => x.EmpresaID,
                        principalTable: "Empresas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessoes_Modulos_ModuloID",
                        column: x => x.ModuloID,
                        principalTable: "Modulos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessoes_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SensoresConfiguracao",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    CalibracaoID = table.Column<int>(nullable: true),
                    SensorDataID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensoresConfiguracao", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SensoresConfiguracao_Calibracoes_CalibracaoID",
                        column: x => x.CalibracaoID,
                        principalTable: "Calibracoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bombas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    DrenoGPM = table.Column<double>(nullable: true),
                    DrenoLPM = table.Column<double>(nullable: true),
                    PilotagemPSI = table.Column<double>(nullable: true),
                    PilotagemBAR = table.Column<double>(nullable: true),
                    PressaoPSI = table.Column<double>(nullable: true),
                    PressaoBAR = table.Column<double>(nullable: true),
                    VazaoGPM = table.Column<double>(nullable: true),
                    VazaoLPM = table.Column<double>(nullable: true),
                    TemperaturaCelsius = table.Column<double>(nullable: true),
                    RotacaoRPM = table.Column<double>(nullable: true),
                    SessaoID = table.Column<int>(nullable: true),
                    EtapaID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bombas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bombas_Etapas_EtapaID",
                        column: x => x.EtapaID,
                        principalTable: "Etapas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bombas_Sessoes_SessaoID",
                        column: x => x.SessaoID,
                        principalTable: "Sessoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cilindros",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    PressaoPSI = table.Column<double>(nullable: true),
                    PressaoBAR = table.Column<double>(nullable: true),
                    TemperaturaCelsius = table.Column<double>(nullable: true),
                    VazaoLPM = table.Column<double>(nullable: true),
                    SessaoID = table.Column<int>(nullable: true),
                    EtapaID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cilindros", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cilindros_Etapas_EtapaID",
                        column: x => x.EtapaID,
                        principalTable: "Etapas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cilindros_Sessoes_SessaoID",
                        column: x => x.SessaoID,
                        principalTable: "Sessoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comandos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    DrenoGPM = table.Column<double>(nullable: true),
                    DrenoLPM = table.Column<double>(nullable: true),
                    PilotagemPSI = table.Column<double>(nullable: true),
                    PilotagemBAR = table.Column<double>(nullable: true),
                    PressaoPSI = table.Column<double>(nullable: true),
                    PressaoBAR = table.Column<double>(nullable: true),
                    VazaoGPM = table.Column<double>(nullable: true),
                    VazaoLPM = table.Column<double>(nullable: true),
                    TemperaturaCelsius = table.Column<double>(nullable: true),
                    RotacaoRPM = table.Column<double>(nullable: true),
                    SessaoID = table.Column<int>(nullable: true),
                    EtapaID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comandos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comandos_Etapas_EtapaID",
                        column: x => x.EtapaID,
                        principalTable: "Etapas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comandos_Sessoes_SessaoID",
                        column: x => x.SessaoID,
                        principalTable: "Sessoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conexoes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    PortaName = table.Column<string>(type: "TEXT", nullable: false),
                    BaudRate = table.Column<int>(nullable: true),
                    Arduino = table.Column<string>(type: "TEXT", nullable: true),
                    SessaoID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conexoes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Conexoes_Sessoes_SessaoID",
                        column: x => x.SessaoID,
                        principalTable: "Sessoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Direcoes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    DrenoLPM = table.Column<double>(nullable: true),
                    PressaoPSI = table.Column<double>(nullable: true),
                    PressaoBAR = table.Column<double>(nullable: true),
                    VazaoGPM = table.Column<double>(nullable: true),
                    VazaoLPM = table.Column<double>(nullable: true),
                    TemperaturaCelsius = table.Column<double>(nullable: true),
                    SessaoID = table.Column<int>(nullable: true),
                    EtapaID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Direcoes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Direcoes_Etapas_EtapaID",
                        column: x => x.EtapaID,
                        principalTable: "Etapas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Direcoes_Sessoes_SessaoID",
                        column: x => x.SessaoID,
                        principalTable: "Sessoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Eletrovalvulas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    DrenoLPM = table.Column<double>(nullable: true),
                    PressaoPSI = table.Column<double>(nullable: true),
                    PressaoBAR = table.Column<double>(nullable: true),
                    VazaoGPM = table.Column<double>(nullable: true),
                    VazaoLPM = table.Column<double>(nullable: true),
                    TemperaturaCelsius = table.Column<double>(nullable: true),
                    SessaoID = table.Column<int>(nullable: true),
                    EtapaID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eletrovalvulas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Eletrovalvulas_Etapas_EtapaID",
                        column: x => x.EtapaID,
                        principalTable: "Etapas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Eletrovalvulas_Sessoes_SessaoID",
                        column: x => x.SessaoID,
                        principalTable: "Sessoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Motores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    DrenoLPM = table.Column<double>(nullable: true),
                    PressaoPSI = table.Column<double>(nullable: true),
                    PressaoBAR = table.Column<double>(nullable: true),
                    VazaoGPM = table.Column<double>(nullable: true),
                    VazaoLPM = table.Column<double>(nullable: true),
                    RotacaoRPM = table.Column<double>(nullable: true),
                    TemperaturaCelsius = table.Column<double>(nullable: true),
                    SessaoID = table.Column<int>(nullable: true),
                    EtapaID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Motores_Etapas_EtapaID",
                        column: x => x.EtapaID,
                        principalTable: "Etapas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Motores_Sessoes_SessaoID",
                        column: x => x.SessaoID,
                        principalTable: "Sessoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Relatorios",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Contato = table.Column<string>(type: "TEXT", nullable: true),
                    NumeroSerie = table.Column<string>(type: "TEXT", nullable: true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    Conclusao = table.Column<string>(type: "TEXT", nullable: true),
                    SessaoID = table.Column<int>(nullable: true),
                    EtapaID = table.Column<int>(nullable: true),
                    EmpresaID = table.Column<int>(nullable: true),
                    ClienteID = table.Column<int>(nullable: true),
                    ImagemID = table.Column<int>(nullable: true),
                    ModuloID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relatorios", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Relatorios_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Relatorios_Empresas_EmpresaID",
                        column: x => x.EmpresaID,
                        principalTable: "Empresas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Relatorios_Etapas_EtapaID",
                        column: x => x.EtapaID,
                        principalTable: "Etapas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Relatorios_Imagens_ImagemID",
                        column: x => x.ImagemID,
                        principalTable: "Imagens",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Relatorios_Modulos_ModuloID",
                        column: x => x.ModuloID,
                        principalTable: "Modulos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Relatorios_Sessoes_SessaoID",
                        column: x => x.SessaoID,
                        principalTable: "Sessoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SensorDataDB",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TerminateTime = table.Column<DateTime>(nullable: true),
                    Sensor = table.Column<string>(type: "TEXT", nullable: false),
                    Valor = table.Column<double>(nullable: true),
                    Medida = table.Column<string>(type: "TEXT", nullable: true),
                    SessaoID = table.Column<int>(nullable: true),
                    EtapaID = table.Column<int>(nullable: true),
                    SensorConfiguracaoID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorDataDB", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SensorDataDB_Etapas_EtapaID",
                        column: x => x.EtapaID,
                        principalTable: "Etapas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SensorDataDB_SensoresConfiguracao_SensorConfiguracaoID",
                        column: x => x.SensorConfiguracaoID,
                        principalTable: "SensoresConfiguracao",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SensorDataDB_Sessoes_SessaoID",
                        column: x => x.SessaoID,
                        principalTable: "Sessoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "ID", "CreateTime", "EmpresaID", "IsAdmin", "Name", "TerminateTime", "UpdateTime" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "admin", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_Bombas_EtapaID",
                table: "Bombas",
                column: "EtapaID");

            migrationBuilder.CreateIndex(
                name: "IX_Bombas_SessaoID",
                table: "Bombas",
                column: "SessaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Calibracoes_ModuloID",
                table: "Calibracoes",
                column: "ModuloID");

            migrationBuilder.CreateIndex(
                name: "IX_Calibracoes_UsuarioID",
                table: "Calibracoes",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Cilindros_EtapaID",
                table: "Cilindros",
                column: "EtapaID");

            migrationBuilder.CreateIndex(
                name: "IX_Cilindros_SessaoID",
                table: "Cilindros",
                column: "SessaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_EmpresaID",
                table: "Clientes",
                column: "EmpresaID");

            migrationBuilder.CreateIndex(
                name: "IX_Comandos_EtapaID",
                table: "Comandos",
                column: "EtapaID");

            migrationBuilder.CreateIndex(
                name: "IX_Comandos_SessaoID",
                table: "Comandos",
                column: "SessaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Conexoes_SessaoID",
                table: "Conexoes",
                column: "SessaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Direcoes_EtapaID",
                table: "Direcoes",
                column: "EtapaID");

            migrationBuilder.CreateIndex(
                name: "IX_Direcoes_SessaoID",
                table: "Direcoes",
                column: "SessaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Eletrovalvulas_EtapaID",
                table: "Eletrovalvulas",
                column: "EtapaID");

            migrationBuilder.CreateIndex(
                name: "IX_Eletrovalvulas_SessaoID",
                table: "Eletrovalvulas",
                column: "SessaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_CNPJ",
                table: "Empresas",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Etapas_EmpresaID",
                table: "Etapas",
                column: "EmpresaID");

            migrationBuilder.CreateIndex(
                name: "IX_Etapas_UsuarioID",
                table: "Etapas",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Imagens_ClienteID",
                table: "Imagens",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Imagens_EmpresaID",
                table: "Imagens",
                column: "EmpresaID");

            migrationBuilder.CreateIndex(
                name: "IX_Monitoramentos_ModuloID",
                table: "Monitoramentos",
                column: "ModuloID");

            migrationBuilder.CreateIndex(
                name: "IX_Motores_EtapaID",
                table: "Motores",
                column: "EtapaID");

            migrationBuilder.CreateIndex(
                name: "IX_Motores_SessaoID",
                table: "Motores",
                column: "SessaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_ClienteID",
                table: "Relatorios",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_EmpresaID",
                table: "Relatorios",
                column: "EmpresaID");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_EtapaID",
                table: "Relatorios",
                column: "EtapaID");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_ImagemID",
                table: "Relatorios",
                column: "ImagemID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_ModuloID",
                table: "Relatorios",
                column: "ModuloID");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_SessaoID",
                table: "Relatorios",
                column: "SessaoID");

            migrationBuilder.CreateIndex(
                name: "IX_SensorDataDB_EtapaID",
                table: "SensorDataDB",
                column: "EtapaID");

            migrationBuilder.CreateIndex(
                name: "IX_SensorDataDB_SensorConfiguracaoID",
                table: "SensorDataDB",
                column: "SensorConfiguracaoID");

            migrationBuilder.CreateIndex(
                name: "IX_SensorDataDB_SessaoID",
                table: "SensorDataDB",
                column: "SessaoID");

            migrationBuilder.CreateIndex(
                name: "IX_SensoresConfiguracao_CalibracaoID",
                table: "SensoresConfiguracao",
                column: "CalibracaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_ClienteID",
                table: "Sessoes",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_EmpresaID",
                table: "Sessoes",
                column: "EmpresaID");

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_ModuloID",
                table: "Sessoes",
                column: "ModuloID");

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_UsuarioID",
                table: "Sessoes",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaID",
                table: "Usuarios",
                column: "EmpresaID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Name",
                table: "Usuarios",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bombas");

            migrationBuilder.DropTable(
                name: "Cilindros");

            migrationBuilder.DropTable(
                name: "Comandos");

            migrationBuilder.DropTable(
                name: "Conexoes");

            migrationBuilder.DropTable(
                name: "Direcoes");

            migrationBuilder.DropTable(
                name: "Eletrovalvulas");

            migrationBuilder.DropTable(
                name: "Monitoramentos");

            migrationBuilder.DropTable(
                name: "Motores");

            migrationBuilder.DropTable(
                name: "Relatorios");

            migrationBuilder.DropTable(
                name: "SensorDataDB");

            migrationBuilder.DropTable(
                name: "TemposExecucao");

            migrationBuilder.DropTable(
                name: "Imagens");

            migrationBuilder.DropTable(
                name: "Etapas");

            migrationBuilder.DropTable(
                name: "SensoresConfiguracao");

            migrationBuilder.DropTable(
                name: "Sessoes");

            migrationBuilder.DropTable(
                name: "Calibracoes");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
