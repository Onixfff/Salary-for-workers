-- MySQL dump 10.13  Distrib 8.0.23, for Win64 (x86_64)
--
-- Host: localhost    Database: authorization
-- ------------------------------------------------------
-- Server version	8.0.23

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
-- Table structure for table `department`
--

DROP TABLE IF EXISTS `department`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `department` (
  `id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `idPeople` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `idDepartment>People_idx` (`idPeople`),
  CONSTRAINT `idDepartment>People` FOREIGN KEY (`idPeople`) REFERENCES `people` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `department`
--

LOCK TABLES `department` WRITE;
/*!40000 ALTER TABLE `department` DISABLE KEYS */;
INSERT INTO `department` VALUES (1,'rwq',1),(2,'tew',2),(3,'ttew',3);
/*!40000 ALTER TABLE `department` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `passwords`
--

DROP TABLE IF EXISTS `passwords`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `passwords` (
  `id` int NOT NULL AUTO_INCREMENT,
  `password` varchar(180) NOT NULL,
  `login` varchar(45) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `login_UNIQUE` (`login`),
  UNIQUE KEY `password_UNIQUE` (`password`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `passwords`
--

LOCK TABLES `passwords` WRITE;
/*!40000 ALTER TABLE `passwords` DISABLE KEYS */;
INSERT INTO `passwords` VALUES (1,'ktc8YGN+EDWhqYinmi+z0cLH0RALIpOwsTU0mxY608I=','13'),(2,'c5uZJbqJC49UV+i4LoFE6JK3kRkapnMTbyEi20m7Leo=','14'),(3,'4','4');
/*!40000 ALTER TABLE `passwords` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `people`
--

DROP TABLE IF EXISTS `people`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `people` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) NOT NULL,
  `Surname` varchar(45) NOT NULL,
  `Patronymic` varchar(45) NOT NULL,
  `EmploymentDate` datetime NOT NULL,
  `idPassword` int DEFAULT NULL,
  `idPositions` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `id_UNIQUE` (`Id`),
  KEY `idPasswords_idx` (`idPassword`),
  CONSTRAINT `idPasswords` FOREIGN KEY (`idPassword`) REFERENCES `passwords` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `people`
--

LOCK TABLES `people` WRITE;
/*!40000 ALTER TABLE `people` DISABLE KEYS */;
INSERT INTO `people` VALUES (1,'Alex','Srr','RE','2021-01-20 01:00:00',1,1),(2,'Юра','Александрович','Александров','2001-01-01 00:00:00',3,2),(3,'Алексей','Бибинов','Эдуардович','2022-02-01 00:00:00',2,1);
/*!40000 ALTER TABLE `people` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `positions`
--

DROP TABLE IF EXISTS `positions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `positions` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `positions`
--

LOCK TABLES `positions` WRITE;
/*!40000 ALTER TABLE `positions` DISABLE KEYS */;
INSERT INTO `positions` VALUES (1,'rq'),(2,'trr'),(3,'ttt');
/*!40000 ALTER TABLE `positions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `states`
--

DROP TABLE IF EXISTS `states`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `states` (
  `id` int NOT NULL AUTO_INCREMENT,
  `abbreviation` varchar(4) NOT NULL,
  `Decoding_abbreviation` varchar(160) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `states`
--

LOCK TABLES `states` WRITE;
/*!40000 ALTER TABLE `states` DISABLE KEYS */;
INSERT INTO `states` VALUES (1,'Я','Продолжительность работы в дневное время'),(2,'Н','Продолжительность работы в ночное время'),(3,'РВ','Продолжительность работы в выходные и нерабочие праздничные дни'),(4,'С','Продолжительность сверхурочной работы'),(5,'ВМ','Продолжительность работы вахтовым методом'),(6,'К','Служебная командировка'),(7,'ПК','Повышение квалификации с отрывом от работы'),(8,'ПМ','Повышение квалификации с отрывом от работы в другой местности'),(9,'ОТ','Ежегодный основной оплачиваемый отпуск'),(10,'ОД','Ежегодный дополнительный оплачиваемый отпуск'),(11,'У','Дополнительный отпуск в связи с обучением с сохранением среднего заработка работникам, совмещающим работу с обучением'),(12,'УВ','Сокращенная продолжительность рабочего времени для обучающихся без отрыва от производства с частичным сохранением заработной платы'),(13,'УД','Дополнительный отпуск в связи с обучением без сохранения заработной платы'),(14,'Р','Отпуск по беременности и родам (отпуск в связи с усыновлением новорожденного ребенка)'),(15,'ОЖ','Отпуск по уходу за ребенком до достижения им возраста трех лет'),(16,'ДО','Отпуск без сохранения заработной платы, предоставляемый работнику по разрешению работодателя'),(17,'ОЗ','Отпуск без сохранения заработной платы при условиях, предусмотренных действующим законодательством Российской Федерации'),(18,'ДБ','Ежегодный дополнительный отпуск без сохранения заработной платы'),(19,'Б','Временная нетрудоспособность (кроме случаев, предусмотренных кодом «Т») с назначением пособия согласно законодательству'),(20,'Т','Временная нетрудоспособность без назначения пособия в случаях, предусмотренных законодательством'),(21,'ЛЧ','Сокращенная продолжительность рабочего времени против нормальной продолжительности рабочего дня в случаях, предусмотренных законодательством'),(22,'ПВ','Время вынужденного прогула в случае признания увольнения, перевода на другую работу или отстранения от работы незаконными с восстановлением на прежней работе'),(23,'Г','Невыходы на время исполнения государственных или общественных обязанностей согласно законодательству'),(24,'ПР','Прогулы (отсутствие на рабочем месте без уважительных причин в течение времени, установленного законодательством)'),(25,'НС','Продолжительность работы в режиме неполного рабочего времени по инициативе работодателя в случаях, предусмотренных законодательством'),(26,'В','Выходные дни (еженедельный отпуск) и нерабочие праздничные дни'),(27,'ОВ','Дополнительные выходные дни (оплачиваемые)'),(28,'НВ','Дополнительные выходные дни (без сохранения заработной платы)'),(29,'ЗБ','Забастовка (при условиях и в порядке, предусмотренных законом)'),(30,'НН','Неявки по невыясненным причинам (до выяснения обстоятельств)'),(31,'РП','Время простоя по вине работодателя'),(32,'НП','Время простоя по причинам, не зависящим от работодателя и работника'),(33,'ВП','Время простоя по вине работника'),(34,'НО','Отстранение от работы (недопущение к работе) с оплатой (пособием) в соответствии с законодательством'),(35,'НБ','Отстранение от работы (недопущение к работе) по причинам, предусмотренным законодательством, без начисления заработной платы'),(36,'НЗ','Время приостановки работы в случае задержки выплаты заработной платы');
/*!40000 ALTER TABLE `states` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `timework`
--

DROP TABLE IF EXISTS `timework`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `timework` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Date` date NOT NULL,
  `Day` int DEFAULT NULL,
  `Night` int DEFAULT NULL,
  `idPeople` int NOT NULL,
  `IdStateDay` int DEFAULT NULL,
  `IdStateNight` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`),
  KEY `idPeoples_idx` (`idPeople`),
  KEY `idStateDay_idx` (`IdStateDay`),
  KEY `rrrr_idx` (`IdStateNight`),
  CONSTRAINT `idPeoples` FOREIGN KEY (`idPeople`) REFERENCES `people` (`Id`),
  CONSTRAINT `IdStateDay` FOREIGN KEY (`IdStateDay`) REFERENCES `states` (`id`),
  CONSTRAINT `IdStateNight` FOREIGN KEY (`IdStateNight`) REFERENCES `states` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `timework`
--

LOCK TABLES `timework` WRITE;
/*!40000 ALTER TABLE `timework` DISABLE KEYS */;
INSERT INTO `timework` VALUES (1,'2024-03-28',12,13,2,1,2),(2,'2024-03-29',13,14,2,1,2),(3,'2024-04-01',13,13,3,1,2),(4,'2024-04-01',3,5,3,5,16),(7,'2024-04-15',13,NULL,2,1,NULL),(8,'2024-04-15',13,NULL,3,1,NULL),(9,'2024-04-01',4,2,1,13,13),(10,'2024-04-18',31,43,3,8,11),(11,'2024-04-18',31,43,1,8,11),(12,'2024-04-30',43,4,3,15,15),(13,'2024-04-30',42,NULL,1,6,NULL);
/*!40000 ALTER TABLE `timework` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-04-08 10:50:29
