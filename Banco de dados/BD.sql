CREATE DATABASE  IF NOT EXISTS `bd_aula_backend` /*!40100 DEFAULT CHARACTER SET utf8mb3 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `bd_aula_backend`;
-- MySQL dump 10.13  Distrib 8.0.29, for Win64 (x86_64)
--
-- Host: localhost    Database: bd_aula_backend
-- ------------------------------------------------------
-- Server version	8.2.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `atendimento`
--

DROP TABLE IF EXISTS `atendimento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `atendimento` (
  `idatendimento` int NOT NULL AUTO_INCREMENT,
  `idpessoa` int NOT NULL,
  `data_atendimento` date NOT NULL,
  `status_atendimento` char(1) NOT NULL,
  PRIMARY KEY (`idatendimento`),
  KEY `fk_atendimento_pessoa1_idx` (`idpessoa`),
  CONSTRAINT `fk_atendimento_pessoa1` FOREIGN KEY (`idpessoa`) REFERENCES `pessoa` (`idpessoa`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `atendimento`
--

LOCK TABLES `atendimento` WRITE;
/*!40000 ALTER TABLE `atendimento` DISABLE KEYS */;
INSERT INTO `atendimento` VALUES (1,1,'2026-10-21','A'),(2,4,'2026-10-21','A'),(3,1,'2026-10-21','A'),(4,4,'2026-10-27','A'),(5,1,'2026-11-04','A'),(6,1,'2026-11-04','A'),(7,1,'2026-11-04','A'),(8,1,'2026-11-04','A'),(9,10,'2026-11-04','A'),(10,2,'2026-11-05','C'),(11,6,'2026-11-05','P'),(12,1,'2026-11-05','X'),(13,1,'2026-11-05','X');
/*!40000 ALTER TABLE `atendimento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `atendimentoitem`
--

DROP TABLE IF EXISTS `atendimentoitem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `atendimentoitem` (
  `idatendimento` int NOT NULL,
  `iditem` int NOT NULL,
  `idfuncionario` int NOT NULL,
  `quantidade_itens` int NOT NULL,
  `valorunitario_entrada` decimal(10,2) NOT NULL,
  PRIMARY KEY (`idatendimento`,`iditem`),
  KEY `fk_item_has_atendimento_atendimento1_idx` (`idatendimento`),
  KEY `fk_item_has_atendimento_item1_idx` (`iditem`),
  KEY `fk_atendimentoitem_funcionario1_idx` (`idfuncionario`),
  CONSTRAINT `fk_atendimentoitem_funcionario1` FOREIGN KEY (`idfuncionario`) REFERENCES `funcionario` (`idpessoa`),
  CONSTRAINT `fk_item_has_atendimento_atendimento1` FOREIGN KEY (`idatendimento`) REFERENCES `atendimento` (`idatendimento`),
  CONSTRAINT `fk_item_has_atendimento_item1` FOREIGN KEY (`iditem`) REFERENCES `item` (`iditem`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `atendimentoitem`
--

LOCK TABLES `atendimentoitem` WRITE;
/*!40000 ALTER TABLE `atendimentoitem` DISABLE KEYS */;
INSERT INTO `atendimentoitem` VALUES (1,1,1,1,156.00),(1,3,2,1,40.00),(5,2,1,3,37.80),(6,1,1,1,26.52),(7,1,1,2,26.52),(7,2,1,2,37.80),(8,1,1,1,26.52),(9,2,1,1,37.80),(10,1,1,1,26.52),(11,2,1,1,37.80),(12,7,1,10,20.80),(13,5,1,1,260.00);
/*!40000 ALTER TABLE `atendimentoitem` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `tr_debito_estoque_atendimento_item` AFTER INSERT ON `atendimentoitem` FOR EACH ROW begin
	declare estoque_atual int;
    declare disponibilidade char;
    declare tpitem char;
        
    -- 	ATRIBUINDO O VALOR DO ESTOQUE ATUAL NA VARIÁVEL
    select
		quantidade_estoque
	into
		estoque_atual
	from
		estoque
	where
		iditem = new.iditem
	for update;
  /*      
	-- CAPTURANDO A DISPOSNIBILIDADE DO PRODUTO POR MEIO DE SEU STATUS
    select
		ativo_produto_servico
	into 
		disponibilidade
	from
		item
	where
			iditem = new.iditem
	for update;
    
     -- 	ATRIBUINDO O TIPO DO ITEM NA VARIÁVEL
    select
		tipo_item
	into
		tpitem
	from
		item
	where
		iditem = new.iditem
	for update;
    
    */
    
    SELECT
		ativo_produto_servico,
		tipo_item
	INTO
		disponibilidade,
		tpitem
	FROM 
		item
	WHERE 
		iditem = NEW.iditem
	FOR UPDATE;
    
    -- Verificar a disponibilidade do produto
    if disponibilidade <> 'S' then
		signal sqlstate '45000'
        set message_text = 'Produto indisponível';
	end if ; 
    
    -- Verificar se o produto existe no estoque
    if tpitem is null then
		signal sqlstate '45000'
        set message_text = 'Produto não econtrado';
	end if ; 
    
    -- Verificar se a quantidade em estoque é insuficiente
    if estoque_atual < new.quantidade_itens then
		signal sqlstate '45000'
        set message_text = 'Produto insuficiente no estoque';
    end if ;
       
	-- VERIFICANDO TIPO PRODUTO OU SERVIÇO
    if tpitem = 'P' then
		update
			estoque
		set
			quantidade_estoque = quantidade_estoque - new.quantidade_itens
		where
			iditem = new.iditem;	
	end if;
	
end */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `categoria`
--

DROP TABLE IF EXISTS `categoria`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categoria` (
  `idcategoria` int NOT NULL AUTO_INCREMENT,
  `categoria` varchar(35) NOT NULL,
  PRIMARY KEY (`idcategoria`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categoria`
--

LOCK TABLES `categoria` WRITE;
/*!40000 ALTER TABLE `categoria` DISABLE KEYS */;
INSERT INTO `categoria` VALUES (1,'Acessorios'),(2,'Manutencao'),(3,'Servicos');
/*!40000 ALTER TABLE `categoria` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `endereco`
--

DROP TABLE IF EXISTS `endereco`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `endereco` (
  `idendereco` int NOT NULL AUTO_INCREMENT,
  `idpessoa` int NOT NULL,
  `cep` char(8) NOT NULL,
  `numero` char(10) NOT NULL,
  `complemento` varchar(150) NOT NULL,
  PRIMARY KEY (`idendereco`),
  KEY `fk_endereco_pessoa1_idx` (`idpessoa`),
  CONSTRAINT `fk_endereco_pessoa1` FOREIGN KEY (`idpessoa`) REFERENCES `pessoa` (`idpessoa`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `endereco`
--

LOCK TABLES `endereco` WRITE;
/*!40000 ALTER TABLE `endereco` DISABLE KEYS */;
INSERT INTO `endereco` VALUES (1,1,'49150000','900','Casa'),(2,1,'49151000','120','Comercial'),(3,2,'49010000','101','Casa atualizada'),(4,2,'49060000','230','Casa'),(5,2,'49012000','230','Apartamento'),(6,3,'','',''),(7,4,'49000000','123','Casa'),(8,5,'49000000','123','Casa'),(9,6,'49050180','100','Casa'),(10,9,'49000000','123','Casa'),(11,10,'49000000','456','Casa'),(12,11,'49000001','96','casa'),(13,12,'12312312','23','');
/*!40000 ALTER TABLE `endereco` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `entrada_produto`
--

DROP TABLE IF EXISTS `entrada_produto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `entrada_produto` (
  `identrada_produto` int NOT NULL AUTO_INCREMENT,
  `iditem` int NOT NULL,
  `data_entrada_produto` date NOT NULL,
  `quantidade_entrada` int NOT NULL,
  `unidade` char(10) NOT NULL,
  `valor_unitario_entrada` decimal(10,2) NOT NULL,
  PRIMARY KEY (`identrada_produto`,`iditem`),
  KEY `fk_entrada_produto_estoque1_idx` (`iditem`),
  CONSTRAINT `fk_entrada_produto_estoque1` FOREIGN KEY (`iditem`) REFERENCES `estoque` (`iditem`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `entrada_produto`
--

LOCK TABLES `entrada_produto` WRITE;
/*!40000 ALTER TABLE `entrada_produto` DISABLE KEYS */;
INSERT INTO `entrada_produto` VALUES (1,1,'2026-10-22',5,'UN',30.00),(2,2,'2026-10-20',20,'UN',35.00),(3,1,'2026-10-20',10,'UN',15.50),(4,1,'2026-10-27',10,'UN',25.50),(5,5,'2026-11-04',10,'UN',250.00),(6,6,'2026-11-04',10,'UN',180.00),(7,7,'2026-11-04',10,'UN',20.00),(8,8,'2026-11-04',10,'UN',60.00),(9,9,'2026-11-04',10,'UN',100.00),(10,10,'2026-11-04',10,'UN',300.00),(11,11,'2026-11-06',12,'PAR',90.99);
/*!40000 ALTER TABLE `entrada_produto` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `tr_entrada_produto` AFTER INSERT ON `entrada_produto` FOR EACH ROW begin
	-- DECLARANDO UMA VARIÁVEL PARA PEGAR TIPO PRODUTO OU SERVIÇO
    declare tp_prod_serv int;
	-- DECLARANDO UMA VARIÁVEL PARA PEGAR O CÓDIGO PREÇO
    declare cod_preco int;    
    
	-- ALTERANDO O PRECO DA TABELA ITEM
    update 
		item
	set 
        preco = new.valor_unitario_entrada
	where
		iditem = new.iditem;
    
    -- ATRIBUINDO O CODIGO DO PREÇO NA VARÁVEL
    select 
		codigo_preco
	into
		cod_preco
	from
		produto
	where
		iditem = new.iditem;

    -- ALTERANDO O PRECO VENDA
    update 
		produto
	set 
        preco_venda = fn_calc_preco_venda(cod_preco, new.valor_unitario_entrada)
	where
		iditem = new.iditem;
        
	-- ALTERANDO A QUANTIDADE NO ESTOQUE
	update
			estoque
		set
			quantidade_estoque = quantidade_estoque + new.quantidade_entrada
		where
			iditem = new.iditem;
		
end */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `estoque`
--

DROP TABLE IF EXISTS `estoque`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `estoque` (
  `iditem` int NOT NULL,
  `quantidade_estoque` int NOT NULL,
  PRIMARY KEY (`iditem`),
  KEY `fk_estoque_produto1_idx` (`iditem`),
  CONSTRAINT `fk_estoque_produto1` FOREIGN KEY (`iditem`) REFERENCES `produto` (`iditem`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `estoque`
--

LOCK TABLES `estoque` WRITE;
/*!40000 ALTER TABLE `estoque` DISABLE KEYS */;
INSERT INTO `estoque` VALUES (1,24),(2,11),(5,9),(6,10),(7,0),(8,10),(9,10),(10,10),(11,12);
/*!40000 ALTER TABLE `estoque` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `funcionario`
--

DROP TABLE IF EXISTS `funcionario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `funcionario` (
  `idpessoa` int NOT NULL,
  `cargo` varchar(20) NOT NULL,
  `data_admissao` date NOT NULL,
  PRIMARY KEY (`idpessoa`),
  CONSTRAINT `fk_funcionario_pessoa` FOREIGN KEY (`idpessoa`) REFERENCES `pessoa` (`idpessoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `funcionario`
--

LOCK TABLES `funcionario` WRITE;
/*!40000 ALTER TABLE `funcionario` DISABLE KEYS */;
INSERT INTO `funcionario` VALUES (1,'Atendente','2024-01-10'),(2,'Gerente','2024-01-10'),(4,'Mecanico','2026-10-19'),(5,'Mecanico','2026-10-19'),(6,'Atendente','2026-10-01');
/*!40000 ALTER TABLE `funcionario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `item`
--

DROP TABLE IF EXISTS `item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `item` (
  `iditem` int NOT NULL AUTO_INCREMENT,
  `idcategoria` int NOT NULL,
  `item` varchar(40) NOT NULL,
  `descricao_item` varchar(200) NOT NULL,
  `preco` decimal(10,2) NOT NULL,
  `tipo_item` char(1) NOT NULL,
  `ativo_produto_servico` char(1) NOT NULL,
  PRIMARY KEY (`iditem`),
  KEY `fk_produto_servico_categoria1_idx` (`idcategoria`),
  CONSTRAINT `fk_produto_servico_categoria1` FOREIGN KEY (`idcategoria`) REFERENCES `categoria` (`idcategoria`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `item`
--

LOCK TABLES `item` WRITE;
/*!40000 ALTER TABLE `item` DISABLE KEYS */;
INSERT INTO `item` VALUES (1,1,'Capacete','Capacete para motociclista',25.50,'P','N'),(2,1,'Luvas','Luvas para motociclista',89.99,'P','N'),(3,2,'Troca de oleo','Servico de troca de oleo',40.00,'S','S'),(4,2,'Revisao','Servico de revisao geral',100.00,'S','S'),(5,1,'Jaqueta de Motociclista','Jaqueta com estilo esportivo para motociclistas',250.00,'P','S'),(6,1,'Bota de Motociclista','Bota para uso motociclístico',180.00,'P','S'),(7,1,'Balaclava Preta','Balaclava preta para motociclistas',20.00,'P','S'),(8,1,'Capa de Chuva','Capa de chuva para motociclistas',60.00,'P','S'),(9,1,'Mochila para Motociclista','Mochila para transportar objetos',100.00,'P','S'),(10,1,'Capacete Esportivo','Capacete de estilo esportivo',300.00,'P','S'),(11,1,'Luvas de Motociclista','Luvas de proteção',90.99,'P','S');
/*!40000 ALTER TABLE `item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pessoa`
--

DROP TABLE IF EXISTS `pessoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pessoa` (
  `idpessoa` int NOT NULL AUTO_INCREMENT,
  `nome` varchar(100) NOT NULL,
  `cpf` bigint unsigned NOT NULL,
  `data_nascimento` date NOT NULL,
  `sexo` char(1) NOT NULL,
  `email` varchar(100) NOT NULL,
  `telefone` bigint unsigned NOT NULL,
  `ativo_pessoa` char(1) NOT NULL,
  PRIMARY KEY (`idpessoa`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pessoa`
--

LOCK TABLES `pessoa` WRITE;
/*!40000 ALTER TABLE `pessoa` DISABLE KEYS */;
INSERT INTO `pessoa` VALUES (1,'Marcelo Augusto Nunes',66789012345,'1983-08-11','M','marcelo.nunes@email.com',79999990015,'S'),(2,'Joao Silva Atualizado',11111111111,'2000-05-10','M','joao.atualizado@example.com',79999990001,'S'),(3,'',0,'0001-01-01','','',0,''),(4,'Carlos Teste',12345678901,'1995-05-20','M','carlos.teste@example.com',79912345678,'S'),(5,'Carlos Teste',12345678901,'1995-05-20','M','carlos.teste@example.com',79912345678,'S'),(6,'Funcionario Teste',12345678901,'1995-05-15','M','funcionario.teste@gmail.com',7999999999,'S'),(7,'João Teste',0,'2000-01-01','M','joao@email.com',0,'S'),(8,'João Teste',0,'2000-01-01','M','joao@email.com',0,'S'),(9,'João Teste',12345678901,'2000-05-15','M','joaoteste@email.com',79999999999,'S'),(10,'Carlos Angular',98765432100,'2001-08-18','M','carlos@teste.com',799913434453,'S'),(11,'joao teste',12312312312,'2004-08-17','M','joaoteste@gmail.com',7991456634,'S'),(12,'Carlos miguel',1234553224,'0005-03-12','M','carlosteste@teste.com',8193322334,'S');
/*!40000 ALTER TABLE `pessoa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `produto`
--

DROP TABLE IF EXISTS `produto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `produto` (
  `iditem` int NOT NULL,
  `codigo_preco` int NOT NULL,
  `preco_venda` decimal(10,2) NOT NULL,
  `unidade` char(10) NOT NULL,
  PRIMARY KEY (`iditem`),
  KEY `fk_produto_item1_idx` (`iditem`),
  CONSTRAINT `fk_produto_item1` FOREIGN KEY (`iditem`) REFERENCES `item` (`iditem`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `produto`
--

LOCK TABLES `produto` WRITE;
/*!40000 ALTER TABLE `produto` DISABLE KEYS */;
INSERT INTO `produto` VALUES (1,1,26.52,'UN'),(2,1,37.80,'UN'),(5,1,260.00,'UN'),(6,1,187.20,'PAR'),(7,1,20.80,'UN'),(8,1,62.40,'UN'),(9,1,104.00,'UN'),(10,1,312.00,'UN'),(11,22,90.99,'PAR');
/*!40000 ALTER TABLE `produto` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `tr_addestoque` AFTER INSERT ON `produto` FOR EACH ROW begin
	insert into estoque
		(iditem, quantidade_estoque)
	values
		(new.iditem, 0);
end */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Temporary view structure for view `vw_atendimento`
--

DROP TABLE IF EXISTS `vw_atendimento`;
/*!50001 DROP VIEW IF EXISTS `vw_atendimento`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_atendimento` AS SELECT 
 1 AS `idatendimento`,
 1 AS `data_atendimento`,
 1 AS `status_atendimento`,
 1 AS `nome`,
 1 AS `telefone`,
 1 AS `email`,
 1 AS `cpf`,
 1 AS `item`,
 1 AS `quantidade_itens`,
 1 AS `valorunitario_entrada`,
 1 AS `nome_funcionario`,
 1 AS `cargo`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_funcionario_endereco`
--

DROP TABLE IF EXISTS `vw_funcionario_endereco`;
/*!50001 DROP VIEW IF EXISTS `vw_funcionario_endereco`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_funcionario_endereco` AS SELECT 
 1 AS `idpessoa`,
 1 AS `nome`,
 1 AS `cpf`,
 1 AS `telefone`,
 1 AS `email`,
 1 AS `cargo`,
 1 AS `cep`,
 1 AS `numero`,
 1 AS `complemento`,
 1 AS `data_admissao`,
 1 AS `ativo_pessoa`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_pessoa_endereco`
--

DROP TABLE IF EXISTS `vw_pessoa_endereco`;
/*!50001 DROP VIEW IF EXISTS `vw_pessoa_endereco`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_pessoa_endereco` AS SELECT 
 1 AS `idpessoa`,
 1 AS `nome`,
 1 AS `cpf`,
 1 AS `telefone`,
 1 AS `email`,
 1 AS `cep`,
 1 AS `numero`,
 1 AS `complemento`,
 1 AS `ativo_pessoa`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_produto_categoria`
--

DROP TABLE IF EXISTS `vw_produto_categoria`;
/*!50001 DROP VIEW IF EXISTS `vw_produto_categoria`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_produto_categoria` AS SELECT 
 1 AS `iditem`,
 1 AS `item`,
 1 AS `descricao_item`,
 1 AS `codigo_preco`,
 1 AS `preco`,
 1 AS `preco_venda`,
 1 AS `unidade`,
 1 AS `categoria`,
 1 AS `quantidade_estoque`,
 1 AS `ativo_produto_servico`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_servico_categoria`
--

DROP TABLE IF EXISTS `vw_servico_categoria`;
/*!50001 DROP VIEW IF EXISTS `vw_servico_categoria`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_servico_categoria` AS SELECT 
 1 AS `iditem`,
 1 AS `item`,
 1 AS `descricao_item`,
 1 AS `preco`,
 1 AS `categoria`,
 1 AS `ativo_produto_servico`*/;
SET character_set_client = @saved_cs_client;

--
-- Dumping events for database 'bd_aula_backend'
--

--
-- Dumping routines for database 'bd_aula_backend'
--
/*!50003 DROP FUNCTION IF EXISTS `fn_calc_preco_venda` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `fn_calc_preco_venda`(codigo_preco int, preco_custo decimal(10,2)) RETURNS decimal(10,2)
    DETERMINISTIC
begin
	declare calcValor decimal(10,2);
	
    case codigo_preco
		when 1 then
			set calcValor = preco_custo + (preco_custo * 0.04);
		when 2 then
			set calcValor = preco_custo + (preco_custo * 0.08);
		when 3 then
			set calcValor = preco_custo + (preco_custo * 0.10);
		else
			set calcValor = preco_custo;
	end case;
    
    return calcValor;    
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `pr_atendimento` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `pr_atendimento`(in p_idpessoa int, in p_itens_selecionados json)
begin
	-- DECLARAÇÃO DE VARIÁVEL PARA PEGAR O ID GERADO
    declare idatendimento_gerado int;
    
    -- Aqui captura qualquer erro e desfaz a transação
    declare exit handler for sqlexception
    begin
		rollback;
        resignal;
    end;
    
    start transaction;
    
    -- INSERIR OS DADOS NA TABELA DE ATENDIMENTO
    insert into atendimento
		(idpessoa, data_atendimento, status_atendimento)
	values
		(p_idpessoa, curdate(), 'A');  
        
	-- PEGANDO O ID GERADO DA TABELA atendimento
    set idatendimento_gerado = last_insert_id();
    
    -- INSERINDO OS DADOS, EM LOTE SET-BASED, NA TABELA ATENDIMENTO_SERVICO
    insert into atendimentoitem
		(idatendimento, iditem, idfuncionario, quantidade_itens, valorunitario_entrada)
	select
		idatendimento_gerado, iditem, idfuncionario, quantidade_itens, valorunitario_entrada
	from 
		json_table(
			p_itens_selecionados, 
            '$[*]'columns(
				iditem int path '$.iditem',
				idfuncionario int path '$.idfuncionario',
				quantidade_itens int path '$.quantidade_itens',
				valorunitario_entrada decimal(10,2) path '$.valorunitario_entrada'
			) 
		) as carrinho;
    
    -- CONFIRMAR 
    commit;
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `pr_funcionario` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `pr_funcionario`(in pessoa json)
begin
	declare idpessoa_gerado int;

	declare exit handler for sqlexception
    begin
		rollback;
        resignal;
    end;
    
    start transaction;
    
	insert into pessoa
		(nome, cpf, data_nascimento, sexo, email, telefone, ativo_pessoa)
	select
		nome, cpf, data_nascimento, sexo, email, telefone, ativo_pessoa
	from
		json_table(pessoa,'$' columns(
			nome varchar(100) path '$.nome',
            cpf bigint(11) path '$.cpf',
            data_nascimento date path '$.data_nascimento',
            sexo char(1) path '$.sexo',
            email varchar(100) path '$.email',
            telefone bigint(11) path '$.telefone',
            ativo_pessoa char(1) path '$.ativo')			
        ) as p;
        
	set idpessoa_gerado = last_insert_id();
    
    insert into funcionario
		(idpessoa, cargo, data_admissao)
	select
		idpessoa_gerado, cargo, data_admissao
	from
		json_table(pessoa,'$' columns(
            cargo varchar(20) path '$.cargo',
            data_admissao date path '$.data_admissao')
        ) as f;
    
    insert into endereco
		(idpessoa, cep, numero, complemento)
	select
		idpessoa_gerado, cep, numero, complemento
	from
		json_table(pessoa, '$.enderecos[*]' columns(
			cep int path '$.cep',
            numero char(8) path '$.numero',
            complemento varchar(150) path '$.complemento')
        ) as e;
    commit;
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `pr_pessoa` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `pr_pessoa`(in pessoa json)
begin
	declare idpessoa_gerado int;

	declare exit handler for sqlexception
    begin
		rollback;
        resignal;
    end;
    
    start transaction;
    
	insert into pessoa
		(nome, cpf, data_nascimento, sexo, email, telefone, ativo_pessoa)
	select
		nome, cpf, data_nascimento, sexo, email, telefone, ativo_pessoa
	from
		json_table(pessoa,'$' columns(
			nome varchar(100) path '$.nome',
            cpf bigint(11) path '$.cpf',
            data_nascimento date path '$.data_nascimento',
            sexo char(1) path '$.sexo',
            email varchar(100) path '$.email',
            telefone bigint(11) path '$.telefone',
            ativo_pessoa char(1) path '$.ativo')			
        ) as p;
        
	set idpessoa_gerado = last_insert_id();
    
    insert into endereco
		(idpessoa, cep, numero, complemento)
	select
		idpessoa_gerado, cep, numero, complemento
	from
		json_table(pessoa, '$.enderecos[*]' columns(
			cep int path '$.cep',
            numero char(8) path '$.numero',
            complemento varchar(150) path '$.complemento')
        ) as e;
    commit;
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `pr_produto` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `pr_produto`(in produto json)
begin
	declare iditem_gerado int;
    declare preco_item decimal(10,2);

	declare exit handler for sqlexception
    begin
		rollback;
        resignal;
    end;
    
    start transaction;
    
	insert into item
		(idcategoria, item, descricao_item, preco, tipo_item, ativo_produto_servico)
	select
		idcategoria, item, descricao_item, preco, tipo_item, ativo_produto_servico
	from
		json_table(produto,'$' columns(
			idcategoria int path '$.idcategoria',
            item varchar(40) path '$.item',
            descricao_item varchar(200) path '$.descricao_item',
			preco decimal(10,2) path '$.preco',
            tipo_item char(1) path '$.tipo_item',
            ativo_produto_servico char(1) path '$.ativo_produto_servico')			
        ) as i;
        
	set iditem_gerado = last_insert_id();

    SELECT preco
    INTO preco_item
    FROM item
    WHERE iditem = iditem_gerado;
    
    insert into produto
		(iditem, codigo_preco, preco_venda, unidade)
	select
		iditem_gerado, codigo_preco, fn_calc_preco_venda(codigo_preco, preco_item), unidade
	from
		json_table(produto,'$' columns(
			codigo_preco int path '$.codigo_preco',
            unidade char(10) path '$.unidade')
        ) as p;
    
    commit;

end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Final view structure for view `vw_atendimento`
--

/*!50001 DROP VIEW IF EXISTS `vw_atendimento`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_atendimento` AS select `a`.`idatendimento` AS `idatendimento`,`a`.`data_atendimento` AS `data_atendimento`,`a`.`status_atendimento` AS `status_atendimento`,`p`.`nome` AS `nome`,`p`.`telefone` AS `telefone`,`p`.`email` AS `email`,`p`.`cpf` AS `cpf`,`i`.`item` AS `item`,`ai`.`quantidade_itens` AS `quantidade_itens`,`ai`.`valorunitario_entrada` AS `valorunitario_entrada`,`pf`.`nome` AS `nome_funcionario`,`f`.`cargo` AS `cargo` from (((((`atendimento` `a` join `pessoa` `p` on((`a`.`idpessoa` = `p`.`idpessoa`))) join `atendimentoitem` `ai` on((`a`.`idatendimento` = `ai`.`idatendimento`))) join `item` `i` on((`i`.`iditem` = `ai`.`iditem`))) join `funcionario` `f` on((`ai`.`idfuncionario` = `f`.`idpessoa`))) join `pessoa` `pf` on((`pf`.`idpessoa` = `f`.`idpessoa`))) where (`a`.`idatendimento` = 19) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_funcionario_endereco`
--

/*!50001 DROP VIEW IF EXISTS `vw_funcionario_endereco`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_funcionario_endereco` AS select `p`.`idpessoa` AS `idpessoa`,`p`.`nome` AS `nome`,`p`.`cpf` AS `cpf`,`p`.`telefone` AS `telefone`,`p`.`email` AS `email`,`f`.`cargo` AS `cargo`,`e`.`cep` AS `cep`,`e`.`numero` AS `numero`,`e`.`complemento` AS `complemento`,`f`.`data_admissao` AS `data_admissao`,`p`.`ativo_pessoa` AS `ativo_pessoa` from ((`pessoa` `p` left join `funcionario` `f` on((`p`.`idpessoa` = `f`.`idpessoa`))) join `endereco` `e` on((`f`.`idpessoa` = `e`.`idpessoa`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_pessoa_endereco`
--

/*!50001 DROP VIEW IF EXISTS `vw_pessoa_endereco`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_pessoa_endereco` AS select `p`.`idpessoa` AS `idpessoa`,`p`.`nome` AS `nome`,`p`.`cpf` AS `cpf`,`p`.`telefone` AS `telefone`,`p`.`email` AS `email`,`e`.`cep` AS `cep`,`e`.`numero` AS `numero`,`e`.`complemento` AS `complemento`,`p`.`ativo_pessoa` AS `ativo_pessoa` from ((`pessoa` `p` left join `funcionario` `f` on((`f`.`idpessoa` = `p`.`idpessoa`))) join `endereco` `e` on((`p`.`idpessoa` = `e`.`idpessoa`))) where (`f`.`idpessoa` is null) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_produto_categoria`
--

/*!50001 DROP VIEW IF EXISTS `vw_produto_categoria`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_produto_categoria` AS select `i`.`iditem` AS `iditem`,`i`.`item` AS `item`,`i`.`descricao_item` AS `descricao_item`,`p`.`codigo_preco` AS `codigo_preco`,`i`.`preco` AS `preco`,`p`.`preco_venda` AS `preco_venda`,`p`.`unidade` AS `unidade`,`c`.`categoria` AS `categoria`,`e`.`quantidade_estoque` AS `quantidade_estoque`,`i`.`ativo_produto_servico` AS `ativo_produto_servico` from (((`item` `i` join `produto` `p` on((`i`.`iditem` = `p`.`iditem`))) join `estoque` `e` on((`p`.`iditem` = `e`.`iditem`))) join `categoria` `c` on((`i`.`idcategoria` = `c`.`idcategoria`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_servico_categoria`
--

/*!50001 DROP VIEW IF EXISTS `vw_servico_categoria`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_servico_categoria` AS select `i`.`iditem` AS `iditem`,`i`.`item` AS `item`,`i`.`descricao_item` AS `descricao_item`,`i`.`preco` AS `preco`,`c`.`categoria` AS `categoria`,`i`.`ativo_produto_servico` AS `ativo_produto_servico` from (`item` `i` join `categoria` `c` on((`i`.`idcategoria` = `c`.`idcategoria`))) where (`i`.`tipo_item` = 'S') */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-06 21:09:18
