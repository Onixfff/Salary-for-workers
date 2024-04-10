-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: localhost    Database: authorization
-- ------------------------------------------------------
-- Server version	8.0.36

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
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `department`
--

LOCK TABLES `department` WRITE;
/*!40000 ALTER TABLE `department` DISABLE KEYS */;
INSERT INTO `department` VALUES (1,'Зам начальника отдела продаж'),(2,'Начальник отдела продаж'),(3,'Менеджер   по сбыту продукции');
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
  `EmploymentDate` date NOT NULL,
  `idPassword` int DEFAULT NULL,
  `idPositions` int NOT NULL,
  `idDepartment` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `id_UNIQUE` (`Id`),
  KEY `idDepartment_idx` (`idDepartment`),
  KEY `idPosition_idx` (`idPositions`),
  KEY `IdPassword_idx` (`idPassword`),
  CONSTRAINT `idDepartment` FOREIGN KEY (`idDepartment`) REFERENCES `department` (`id`),
  CONSTRAINT `IdPassword` FOREIGN KEY (`idPassword`) REFERENCES `passwords` (`id`),
  CONSTRAINT `idPosition` FOREIGN KEY (`idPositions`) REFERENCES `positions` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `people`
--

LOCK TABLES `people` WRITE;
/*!40000 ALTER TABLE `people` DISABLE KEYS */;
INSERT INTO `people` VALUES (1,'Александр','Десятов','Вениаминович','2018-03-13',1,1,1),(8,'Екатерина','Уральская','Сергеевна','2016-04-06',NULL,1,3),(9,'Анна','Хомина','Владимировна','2016-04-06',NULL,1,3),(10,'321','321','321','2024-04-10',NULL,1,1);
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
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `positions`
--

LOCK TABLES `positions` WRITE;
/*!40000 ALTER TABLE `positions` DISABLE KEYS */;
INSERT INTO `positions` VALUES (1,'Коммерческий отдел'),(2,'Отдел охраны'),(3,'Отдел ремонта и технического обслуживания оборудования'),(4,'Отдел экономики и финансов'),(5,'Погрузочно-разгрузочный узел'),(6,'Полистирол'),(7,'Производственный отдел газобетон                   '),(8,'Производственный отдел силикатный кирпич                   '),(9,'Производственный отдел сухие смеси'),(10,'Cклад готовой продукции'),(11,'Технологический отдел'),(12,'Транспортный отдел'),(13,'Управление'),(14,'Хозяйственная служба');
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
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `timework`
--

LOCK TABLES `timework` WRITE;
/*!40000 ALTER TABLE `timework` DISABLE KEYS */;
INSERT INTO `timework` VALUES (26,'2024-04-01',12,21,1,5,5);
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

-- Dump completed on 2024-04-10 16:11:12
