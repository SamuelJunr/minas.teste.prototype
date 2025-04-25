-- TABELAS PRINCIPAIS --
-----------------------------------------
CREATE TABLE Usuario (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Name TEXT NOT NULL,
    EmpresaID INTEGER,
    IsAdmin BOOLEAN DEFAULT 0,
    FOREIGN KEY (EmpresaID) REFERENCES Empresa(ID)
);

CREATE TABLE Empresa (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Name TEXT NOT NULL,
    CNPJ TEXT UNIQUE NOT NULL,
    Endereco TEXT,
    Email TEXT,
    Telefone TEXT
);

CREATE TABLE Cliente (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Name TEXT NOT NULL,
    CNPJ TEXT,
    CPF TEXT,
    Endereco TEXT,
    Email TEXT,
    Telefone TEXT,
    EmpresaID INTEGER,
    FOREIGN KEY (EmpresaID) REFERENCES Empresa(ID)
);

CREATE TABLE Sessao (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    ModuloID INTEGER,
    Name TEXT,
    ClienteID INTEGER,
    EmpresaID INTEGER,
    UsuarioID INTEGER,
    FOREIGN KEY (ModuloID) REFERENCES Modulo(ID),
    FOREIGN KEY (ClienteID) REFERENCES Cliente(ID),
    FOREIGN KEY (EmpresaID) REFERENCES Empresa(ID),
    FOREIGN KEY (UsuarioID) REFERENCES Usuario(ID)
);

CREATE TABLE Etapa (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Modulo TEXT,
    Ordem INTEGER,
    Modelo TEXT,
    EmpresaID INTEGER,
    UsuarioID INTEGER,
    FOREIGN KEY (EmpresaID) REFERENCES Empresa(ID),
    FOREIGN KEY (UsuarioID) REFERENCES Usuario(ID)
);

-----------------------------------------
-- TABELAS DE EQUIPAMENTOS E SENSORES --
-----------------------------------------
CREATE TABLE Bombas (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    DrenoGPM REAL,
    DrenoLPM REAL,
    PilotagemPSI REAL,
    PilotagemBAR REAL,
    PressaoPSI REAL,
    PressaoBAR REAL,
    VazaoGPM REAL,
    VazaoLPM REAL,
    TemperaturaCelsius REAL,
    RotacaoRPM REAL,
    SessaoID INTEGER,
    EtapaID INTEGER,
    FOREIGN KEY (SessaoID) REFERENCES Sessao(ID),
    FOREIGN KEY (EtapaID) REFERENCES Etapa(ID)
);

CREATE TABLE Motor (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    DrenoLPM REAL,
    PressaoPSI REAL,
    PressaoBAR REAL,
    VazaoGPM REAL,
    VazaoLPM REAL,
    RotacaoRPM REAL,
    TemperaturaCelsius REAL,
    SessaoID INTEGER,
    EtapaID INTEGER,
    FOREIGN KEY (SessaoID) REFERENCES Sessao(ID),
    FOREIGN KEY (EtapaID) REFERENCES Etapa(ID)
);

CREATE TABLE Eletrovaulas (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    DrenoLPM REAL,
    PressaoPSI REAL,
    PressaoBAR REAL,
    VazaoGPM REAL,
    VazaoLPM REAL,
    TemperaturaCelsius REAL,
    SessaoID INTEGER,
    EtapaID INTEGER,
    FOREIGN KEY (SessaoID) REFERENCES Sessao(ID),
    FOREIGN KEY (EtapaID) REFERENCES Etapa(ID)
);
CREATE TABLE Comandos (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    DrenoGPM REAL,
    DrenoLPM REAL,
    PilotagemPSI REAL,
    PilotagemBAR REAL,
    PressaoPSI REAL,
    PressaoBAR REAL,
    VazaoGPM REAL,
    VazaoLPM REAL,
    TemperaturaCelsius REAL,
    RotacaoRPM REAL,
    SessaoID INTEGER,
    EtapaID INTEGER,
    FOREIGN KEY (SessaoID) REFERENCES Sessao(ID),
    FOREIGN KEY (EtapaID) REFERENCES Etapa(ID)
);

CREATE TABLE Direcao (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    DrenoLPM REAL,
    PressaoPSI REAL,
    PressaoBAR REAL,
    VazaoGPM REAL,
    VazaoLPM REAL,
    TemperaturaCelsius REAL,
    SessaoID INTEGER,
    EtapaID INTEGER,
    FOREIGN KEY (SessaoID) REFERENCES Sessao(ID),
    FOREIGN KEY (EtapaID) REFERENCES Etapa(ID)
);

CREATE TABLE Cilindros (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    PressaoPSI REAL,
    PressaoBAR REAL,
    TemperaturaCelsius REAL,
    VazaoLPM REAL,
    SessaoID INTEGER,
    EtapaID INTEGER,
    FOREIGN KEY (SessaoID) REFERENCES Sessao(ID),
    FOREIGN KEY (EtapaID) REFERENCES Etapa(ID)
);

CREATE TABLE SensorData (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Sensor TEXT NOT NULL,
    Valor REAL,
    Medida TEXT,
    SessaoID INTEGER,
    EtapaID INTEGER,
    SensorConfiguracaoID INTEGER,
    FOREIGN KEY (SessaoID) REFERENCES Sessao(ID),
    FOREIGN KEY (EtapaID) REFERENCES Etapa(ID),
    FOREIGN KEY (SensorConfiguracaoID) REFERENCES SensorConfiguracao(ID)
);

CREATE TABLE Conexao (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    PortaName TEXT NOT NULL,
    BaudRate INTEGER,
    Arduino TEXT,
    SessaoID INTEGER,
    FOREIGN KEY (SessaoID) REFERENCES Sessao(ID)
);

CREATE TABLE Monitoramento (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    PSImin REAL,
    BARmin REAL,
    GPMmin REAL,
    LPMmin REAL,
    RPMmin REAL,
    CELSUSmin REAL,
    PSImax REAL,
    BARmax REAL,
    GPMmax REAL,
    LPMmax REAL,
    RPMmax REAL,
    CELSUSmax REAL,
    ModuloID INTEGER,
    FOREIGN KEY (ModuloID) REFERENCES Modulo(ID)
);

CREATE TABLE TempoExecucao (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Duracao INTEGER  -- Em segundos
);

CREATE TABLE Relatorio (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Contato TEXT,
    NumeroSerie TEXT,
    Descricao TEXT,
    Conclusao TEXT,
    SessaoID INTEGER,
    EtapaID INTEGER,
    EmpresaID INTEGER,
    ClienteID INTEGER,
    ImagemID INTEGER,
    ModuloID INTEGER,
    FOREIGN KEY (SessaoID) REFERENCES Sessao(ID),
    FOREIGN KEY (EtapaID) REFERENCES Etapa(ID),
    FOREIGN KEY (EmpresaID) REFERENCES Empresa(ID),
    FOREIGN KEY (ClienteID) REFERENCES Cliente(ID),
    FOREIGN KEY (ImagemID) REFERENCES Imagem(ID),
    FOREIGN KEY (ModuloID) REFERENCES Modulo(ID)
);

CREATE TABLE Calibracao (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Sensor TEXT NOT NULL,
    DataCalibracao DATETIME,
    ValorOffset REAL,
    ValorGanho REAL,
    LeituraMin REAL,
    LeituraMax REAL,
    ValorRefMin REAL,
    ValorRefMax REAL,
    Unidade TEXT,
    Notas TEXT,
    UsuarioID INTEGER,
    ModuloID INTEGER,
    FOREIGN KEY (UsuarioID) REFERENCES Usuario(ID),
    FOREIGN KEY (ModuloID) REFERENCES Modulo(ID)
);

CREATE TABLE SensorConfiguracao (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Nome TEXT NOT NULL,
    CalibracaoID INTEGER,
    SensorDataID INTEGER,
    FOREIGN KEY (CalibracaoID) REFERENCES Calibracao(ID),
    FOREIGN KEY (SensorDataID) REFERENCES SensorData(ID)
);

CREATE TABLE Modulo (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Nome TEXT NOT NULL
);

CREATE TABLE Imagem (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    TerminateTime DATETIME,
    Dado BLOB,  -- Armazena a imagem em formato binário
    Notas TEXT,
    EmpresaID INTEGER,
    ClienteID INTEGER,
    RelatorioID INTEGER,
    FOREIGN KEY (EmpresaID) REFERENCES Empresa(ID),
    FOREIGN KEY (ClienteID) REFERENCES Cliente(ID),
    FOREIGN KEY (RelatorioID) REFERENCES Relatorio(ID)
);

-----------------------------------------
-- ÍNDICES PARA DESEMPENHO --
-----------------------------------------
CREATE INDEX idx_usuario_empresa ON Usuario(EmpresaID);
CREATE INDEX idx_cliente_empresa ON Cliente(EmpresaID);
CREATE INDEX idx_sessao_modulo ON Sessao(ModuloID);
CREATE INDEX idx_relatorio_sessao ON Relatorio(SessaoID);
CREATE INDEX idx_sensordata_sessao ON SensorData(SessaoID);
CREATE INDEX idx_calibracao_usuario ON Calibracao(UsuarioID);
CREATE INDEX idx_imagem_empresa ON Imagem(EmpresaID);

-----------------------------------------
-- USUÁRIO MASTER INICIAL --
-----------------------------------------
INSERT INTO Usuario (Name, IsAdmin) VALUES ('admin', 1);