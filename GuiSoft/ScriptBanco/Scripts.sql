/* ----------------------------------- */
/* CRIANDO E USANDO BD */
/* ----------------------------------- */

CREATE DATABASE GuiSoft;

USE GuiSoft;

/* ----------------------------------- */
/* CRIANDO TABELAS DO BD */
/* ----------------------------------- */


BEGIN TRANSACTION 

CREATE TABLE Usuario(
IdUsuario UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
LoginUsuarioNVARCHAR (20) NOT NULL,
Senha NVARCHAR (6) NOT NULL
);

CREATE TABLE Pessoa(
IdPessoa UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
NomePessoa NVARCHAR(100) NOT NULL,
TelefonePessoa NVARCHAR(15) NOT NULL,
EmailPessoa NVARCHAR(75),
IdentificacaoPessoa NVARCHAR(25) NOT NULL,
CepPessoa NVARCHAR(10),
LogradouroPessoa NVARCHAR(50),
NumeroPessoa NVARCHAR(5),
ComplementoPessoa NVARCHAR(15),
BairroPessoa NVARCHAR(30),
CidadePessoa NVARCHAR(30),
EstadoPessoa NVARCHAR(2),
IdUsuario UNIQUEIDENTIFIER

constraint fk_Pessoa_Usuario foreign key (IdUsuario) references Usuario (IdUsuario)
);

CREATE TABLE ModoPagamento (
IdPagamento UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
Pagamento NVARCHAR(20),
ATIVO BIT,
IdUsuario UNIQUEIDENTIFIER

constraint fk_Pagamento_Usuario foreign key (IdUsuario) references Usuario (IdUsuario)
);


CREATE TABLE PlanoDeConta (
IdPlanoDeConta UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
DescricaoPlanoDeConta NVARCHAR(50) NOT NULL,
CategoriaPlanoDeConta NVARCHAR(15) NOT NULL,
ATIVO BIT,
IdUsuario UNIQUEIDENTIFIER

constraint fk_PlanoDeConta_Usuario foreign key (IdUsuario) references Usuario (IdUsuario)
);

CREATE TABLE UnidadeNeg (
IdUnidade UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
NomeUnidadeNeg NVARCHAR(50) NOT NULL,
IdUsuario UNIQUEIDENTIFIER

constraint fk_Unidade_Usuario foreign key (IdUsuario) references Usuario (IdUsuario)
);


CREATE TABLE Financeiro(
IdFinanceiro UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
TotalFinanceiro decimal(18,2),       
IdPessoa UNIQUEIDENTIFIER,
IdPlanoDeConta UNIQUEIDENTIFIER,
IdPagamento UNIQUEIDENTIFIER,
DataLancamento DateTime,
DataVencimento Date,
IdUsuario UNIQUEIDENTIFIER


constraint fk_saida_cliente foreign key (IdPessoa) references Pessoa (IdPessoa),
constraint fk_saida_tipoconta foreign key (IdPlanoDeConta) references PlanoDeConta (IdPlanoDeConta),
constraint fk_saida_pagamento foreign key (IdPagamento) references ModoPagamento (IdPagamento),
constraint Fk_financeiro_usuario foreign key (IdUsuario) references Usuario (IdUsuario)
);

CREATE TABLE FinanceiroDetalhe(
IdFinanceiroDetalhe UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
IdFinanceiro UNIQUEIDENTIFIER,  
IdUnidade UNIQUEIDENTIFIER,
SubTotalFinanceiroDetalhe decimal(10,2),  
IdUsuario UNIQUEIDENTIFIER  

constraint fk_financeirodetalhe_financeiro foreign key (IdFinanceiro) references Financeiro (IdFinanceiro),
constraint fk_financeirodetalhe_unidade foreign key (IdUnidade) references UnidadeNeg (IdUnidade),
constraint Fk_financeiro_detalhe foreign key (IdUsuario) references Usuario (IdUsuario)
);

--COMMIT
--ROLLBACK