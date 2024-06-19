CREATE DATABASE  IF NOT EXISTS `technical_college` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `technical_college`;
-- MySqlBackup.NET 2.3.6
-- Dump Time: 2024-06-14 01:35:57
-- --------------------------------------
-- Server version 8.0.1-dmr-log MySQL Community Server (GPL)


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- 
-- Definition of groups
-- 

DROP TABLE IF EXISTS `groups`;
CREATE TABLE IF NOT EXISTS `groups` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` text NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table groups
-- 

/*!40000 ALTER TABLE `groups` DISABLE KEYS */;

/*!40000 ALTER TABLE `groups` ENABLE KEYS */;

-- 
-- Definition of group_curators
-- 

DROP TABLE IF EXISTS `group_curators`;
CREATE TABLE IF NOT EXISTS `group_curators` (
  `id` int(11) NOT NULL,
  `group_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  UNIQUE KEY `group_id` (`group_id`),
  CONSTRAINT `group_curators_ibfk_1` FOREIGN KEY (`id`) REFERENCES `teachers` (`id`) ON DELETE CASCADE,
  CONSTRAINT `group_curators_ibfk_2` FOREIGN KEY (`group_id`) REFERENCES `groups` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table group_curators
-- 

/*!40000 ALTER TABLE `group_curators` DISABLE KEYS */;

/*!40000 ALTER TABLE `group_curators` ENABLE KEYS */;

-- 
-- Definition of peoples
-- 

DROP TABLE IF EXISTS `peoples`;
CREATE TABLE IF NOT EXISTS `peoples` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `first_name` text NOT NULL,
  `last_name` text NOT NULL,
  `patronomic` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table peoples
-- 

/*!40000 ALTER TABLE `peoples` DISABLE KEYS */;
INSERT INTO `peoples`(`id`,`first_name`,`last_name`,`patronomic`) VALUES(1,'Администратор','Главный',NULL);
/*!40000 ALTER TABLE `peoples` ENABLE KEYS */;

-- 
-- Definition of administrators
-- 

DROP TABLE IF EXISTS `administrators`;
CREATE TABLE IF NOT EXISTS `administrators` (
  `id` int(11) NOT NULL,
  KEY `administrators_ibfk_1_idx` (`id`),
  CONSTRAINT `administrators_ibfk_1` FOREIGN KEY (`id`) REFERENCES `peoples` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table administrators
-- 

/*!40000 ALTER TABLE `administrators` DISABLE KEYS */;
INSERT INTO `administrators`(`id`) VALUES(1);
/*!40000 ALTER TABLE `administrators` ENABLE KEYS */;

-- 
-- Definition of secretaries
-- 

DROP TABLE IF EXISTS `secretaries`;
CREATE TABLE IF NOT EXISTS `secretaries` (
  `id` int(11) NOT NULL,
  KEY `secretaries_ibfk_1_idx` (`id`),
  CONSTRAINT `secretaries_ibfk_1` FOREIGN KEY (`id`) REFERENCES `peoples` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table secretaries
-- 

/*!40000 ALTER TABLE `secretaries` DISABLE KEYS */;

/*!40000 ALTER TABLE `secretaries` ENABLE KEYS */;

-- 
-- Definition of students
-- 

DROP TABLE IF EXISTS `students`;
CREATE TABLE IF NOT EXISTS `students` (
  `id` int(11) NOT NULL,
  `group_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  KEY `group_id` (`group_id`),
  CONSTRAINT `students_ibfk_1` FOREIGN KEY (`id`) REFERENCES `peoples` (`id`) ON DELETE CASCADE,
  CONSTRAINT `students_ibfk_2` FOREIGN KEY (`group_id`) REFERENCES `groups` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table students
-- 

/*!40000 ALTER TABLE `students` DISABLE KEYS */;

/*!40000 ALTER TABLE `students` ENABLE KEYS */;

-- 
-- Definition of group_leaders
-- 

DROP TABLE IF EXISTS `group_leaders`;
CREATE TABLE IF NOT EXISTS `group_leaders` (
  `id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  CONSTRAINT `group_leaders_ibfk_1` FOREIGN KEY (`id`) REFERENCES `students` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table group_leaders
-- 

/*!40000 ALTER TABLE `group_leaders` DISABLE KEYS */;

/*!40000 ALTER TABLE `group_leaders` ENABLE KEYS */;

-- 
-- Definition of passes
-- 

DROP TABLE IF EXISTS `passes`;
CREATE TABLE IF NOT EXISTS `passes` (
  `student_id` int(11) NOT NULL,
  `date` date NOT NULL,
  `hours` int(11) NOT NULL,
  `is_excused` tinyint(1) NOT NULL,
  PRIMARY KEY (`student_id`,`date`),
  CONSTRAINT `passes_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table passes
-- 

/*!40000 ALTER TABLE `passes` DISABLE KEYS */;

/*!40000 ALTER TABLE `passes` ENABLE KEYS */;

-- 
-- Definition of teachers
-- 

DROP TABLE IF EXISTS `teachers`;
CREATE TABLE IF NOT EXISTS `teachers` (
  `id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  CONSTRAINT `teachers_ibfk_1` FOREIGN KEY (`id`) REFERENCES `peoples` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table teachers
-- 

/*!40000 ALTER TABLE `teachers` DISABLE KEYS */;

/*!40000 ALTER TABLE `teachers` ENABLE KEYS */;

-- 
-- Definition of users
-- 

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `id` int(11) NOT NULL,
  `login` text NOT NULL,
  `hash` text NOT NULL,
  `is_active` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`id`) REFERENCES `peoples` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table users
-- 

/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users`(`id`,`login`,`hash`,`is_active`) VALUES(1,'admin','8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',1);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;


/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;


-- Dump completed on 2024-06-14 01:35:57
-- Total time: 0:0:0:0:126 (d:h:m:s:ms)
