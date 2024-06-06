CREATE DATABASE  IF NOT EXISTS `technical_college` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `technical_college`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: technical_college
-- ------------------------------------------------------
-- Server version	8.0.1-dmr-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `administrators`
--

DROP TABLE IF EXISTS `administrators`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `administrators` (
  `id` int(11) NOT NULL,
  KEY `administrators_ibfk_1_idx` (`id`),
  CONSTRAINT `administrators_ibfk_1` FOREIGN KEY (`id`) REFERENCES `peoples` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `administrators`
--

LOCK TABLES `administrators` WRITE;
/*!40000 ALTER TABLE `administrators` DISABLE KEYS */;
/*!40000 ALTER TABLE `administrators` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `group_curators`
--

DROP TABLE IF EXISTS `group_curators`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `group_curators` (
  `id` int(11) NOT NULL,
  `group_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  UNIQUE KEY `group_id` (`group_id`),
  CONSTRAINT `group_curators_ibfk_1` FOREIGN KEY (`id`) REFERENCES `teachers` (`id`) ON DELETE CASCADE,
  CONSTRAINT `group_curators_ibfk_2` FOREIGN KEY (`group_id`) REFERENCES `groups` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `group_curators`
--

LOCK TABLES `group_curators` WRITE;
/*!40000 ALTER TABLE `group_curators` DISABLE KEYS */;
INSERT INTO `group_curators` VALUES (25,1);
/*!40000 ALTER TABLE `group_curators` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `group_leaders`
--

DROP TABLE IF EXISTS `group_leaders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `group_leaders` (
  `id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  CONSTRAINT `group_leaders_ibfk_1` FOREIGN KEY (`id`) REFERENCES `students` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `group_leaders`
--

LOCK TABLES `group_leaders` WRITE;
/*!40000 ALTER TABLE `group_leaders` DISABLE KEYS */;
INSERT INTO `group_leaders` VALUES (10);
/*!40000 ALTER TABLE `group_leaders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `groups`
--

DROP TABLE IF EXISTS `groups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `groups` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` text NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `groups`
--

LOCK TABLES `groups` WRITE;
/*!40000 ALTER TABLE `groups` DISABLE KEYS */;
INSERT INTO `groups` VALUES (1,'ПС-20Б'),(2,'БУ-19А');
/*!40000 ALTER TABLE `groups` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `passes`
--

DROP TABLE IF EXISTS `passes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `passes` (
  `student_id` int(11) NOT NULL,
  `date` date NOT NULL,
  `hours` int(11) NOT NULL,
  `is_excused` tinyint(1) NOT NULL,
  PRIMARY KEY (`student_id`,`date`),
  CONSTRAINT `passes_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `passes`
--

LOCK TABLES `passes` WRITE;
/*!40000 ALTER TABLE `passes` DISABLE KEYS */;
INSERT INTO `passes` VALUES (6,'2024-06-07',8,0),(6,'2024-06-08',8,0),(6,'2024-06-10',8,1),(6,'2024-06-11',8,1),(6,'2024-06-12',8,0),(6,'2024-06-13',8,0),(7,'2024-06-03',3,1),(7,'2024-06-04',2,1),(7,'2024-06-05',2,1),(7,'2024-06-06',2,1),(7,'2024-06-07',2,0),(7,'2024-06-08',3,0),(7,'2024-06-10',2,0),(7,'2024-06-13',2,0);
/*!40000 ALTER TABLE `passes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `peoples`
--

DROP TABLE IF EXISTS `peoples`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `peoples` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `first_name` text NOT NULL,
  `last_name` text NOT NULL,
  `patronomic` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `peoples`
--

LOCK TABLES `peoples` WRITE;
/*!40000 ALTER TABLE `peoples` DISABLE KEYS */;
INSERT INTO `peoples` VALUES (1,' Артём',' Тарасов',' Владимирович'),(2,' Арина',' Ульянова',' Львовна'),(3,' Ульяна',' Игнатова',' Арсентьевна'),(4,' Анна',' Боброва',' Вячеславовна'),(6,' Евгения',' Горохова',' Михайловна'),(7,' Георгий',' Волков',' Маркович'),(8,' Степан',' Уткин',' Константинович'),(9,' Евгений',' Щербаков',' Егорович'),(10,' Кира',' Белова',' Даниловна'),(11,' Софья',' Данилова',' Константиновна'),(12,' Максим',' Скворцов',' Иванович'),(14,' Вероника',' Козлова',' Максимовна'),(15,' Валерия',' Соловьева',' Марковна'),(16,' Ева',' Волкова',' Артёмовна'),(17,' Виктория',' Леонова',' Никитична'),(18,' Кира',' Волкова',' Георгиевна'),(19,' Арина',' Маркова',' Артёмовна'),(20,' Дмитрий',' Михайлов',' Ильич'),(21,' Дмитрий',' Левин',' Данилович'),(22,' Олег',' Ермаков',' Алиевич'),(23,' Аврора',' Кузнецова',' Андреевна'),(25,' Александра',' Леонтьева',' Егоровна'),(26,' Роман',' Колесников',' Платонович'),(27,' Денис',' Орлов',' Матвеевич'),(28,' Эмир',' Гладков',' Арсентьевич'),(30,' Агния',' Константинова',' Михайловна');
/*!40000 ALTER TABLE `peoples` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `secretaries`
--

DROP TABLE IF EXISTS `secretaries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `secretaries` (
  `id` int(11) NOT NULL,
  KEY `secretaries_ibfk_1_idx` (`id`),
  CONSTRAINT `secretaries_ibfk_1` FOREIGN KEY (`id`) REFERENCES `peoples` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `secretaries`
--

LOCK TABLES `secretaries` WRITE;
/*!40000 ALTER TABLE `secretaries` DISABLE KEYS */;
/*!40000 ALTER TABLE `secretaries` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `students`
--

DROP TABLE IF EXISTS `students`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `students` (
  `id` int(11) NOT NULL,
  `group_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  KEY `group_id` (`group_id`),
  CONSTRAINT `students_ibfk_1` FOREIGN KEY (`id`) REFERENCES `peoples` (`id`) ON DELETE CASCADE,
  CONSTRAINT `students_ibfk_2` FOREIGN KEY (`group_id`) REFERENCES `groups` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `students`
--

LOCK TABLES `students` WRITE;
/*!40000 ALTER TABLE `students` DISABLE KEYS */;
INSERT INTO `students` VALUES (1,1),(2,1),(3,1),(4,1),(6,1),(7,1),(8,1),(9,1),(10,1),(11,1),(12,1),(14,1),(15,2),(16,2),(17,2),(18,2),(19,2),(20,2);
/*!40000 ALTER TABLE `students` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `teachers`
--

DROP TABLE IF EXISTS `teachers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `teachers` (
  `id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  CONSTRAINT `teachers_ibfk_1` FOREIGN KEY (`id`) REFERENCES `peoples` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `teachers`
--

LOCK TABLES `teachers` WRITE;
/*!40000 ALTER TABLE `teachers` DISABLE KEYS */;
INSERT INTO `teachers` VALUES (21),(22),(23),(25),(26),(27);
/*!40000 ALTER TABLE `teachers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `login` text NOT NULL,
  `hash` text NOT NULL,
  `is_active` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`id`) REFERENCES `peoples` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (3,'','asfasdfasfdasdfsdfasfasdfasdfas',0),(8,'','asfasdfasfdasdfsdfasfasdfasdfas',0),(9,'',' asfasdfasfdasdfsdfasfasdfasdfas',0),(10,'1','1asfasdfasfdasdfsdfasfasdfasdfas',1),(11,'','asfasdfasfdasdfsdfasfasdfasdfas',0),(12,'','asfasdfasfdasdfsdfasfasdfasdfas',0),(14,'','asfasdfasfdasdfsdfasfasdfasdfas',0),(16,'','asfasdfasfdasdfsdfasfasdfasdfas',0),(17,'','asfasdfasfdasdfsdfasfasdfasdfas',0),(20,'','asfasdfasfdasdfsdfasfasdfasdfas',0),(22,'','asfasdfasfdasdfsdfasfasdfasdfas',0),(25,'2','2asfasdfasfdasdfsdfasfasdfasdfas',1),(28,'133','qweasfasdfasfdasdfsdfasfasdfasdfas',1);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-06-06 23:57:30
