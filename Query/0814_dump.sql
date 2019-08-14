/*
SQLyog Community v13.1.5  (64 bit)
MySQL - 8.0.13 : Database - dapper_test
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`dapper_test` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */;

USE `dapper_test`;

/*Table structure for table `customer` */

DROP TABLE IF EXISTS `customer`;

CREATE TABLE `customer` (
  `CustomerID` int(11) NOT NULL AUTO_INCREMENT,
  `CustomerName` varchar(45) NOT NULL,
  `ContactName` varchar(45) NOT NULL,
  `Address` varchar(45) NOT NULL,
  `City` varchar(45) NOT NULL,
  `PostalCode` varchar(45) NOT NULL,
  `Country` varchar(45) NOT NULL,
  PRIMARY KEY (`CustomerID`),
  UNIQUE KEY `CustomerName_UNIQUE` (`CustomerName`),
  KEY `IDX_CUSTOMER` (`CustomerID`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `customer` */

insert  into `customer`(`CustomerID`,`CustomerName`,`ContactName`,`Address`,`City`,`PostalCode`,`Country`) values 
(1,'Jin','alalssk','souel','souel','A','republic of Korea'),
(2,'Jekiy','kmeeo','souel','souel','A','republic of Korea'),
(3,'Minsu','tuna','souel','souel','A','republic of Korea'),
(4,'alalssk','alalssk','Koung Ki','Koung Ki','B','r of Koread'),
(9,'alalssk1','alalssk','Koung Ki','Koung Ki','B','r of Koread'),
(10,'alalssk2','alalssk','Koung Ki','Koung Ki','B','r of Koread'),
(11,'alalssk3','alalssk','Koung Ki','Koung Ki','B','r of Koread'),
(13,'alalssk4','alalssk','Koung Ki','Koung Ki','B','r of Koread'),
(15,'alalssk5','alalssk','Koung Ki','Koung Ki','B','r of Koread'),
(16,'alalssk6','alalssk','Koung Ki','Koung Ki','B','r of Koread'),
(17,'alalssk7','alalssk','Koung Ki','Koung Ki','B','r of Koread');

/*Table structure for table `login_user_info` */

DROP TABLE IF EXISTS `login_user_info`;

CREATE TABLE `login_user_info` (
  `user_idx` bigint(20) NOT NULL AUTO_INCREMENT,
  `platform_type` varchar(2) NOT NULL DEFAULT '00',
  `platform_user_id` varchar(100) NOT NULL,
  `db_idx` tinyint(4) NOT NULL DEFAULT '1',
  `reg_date` datetime NOT NULL,
  `last_login_date` datetime NOT NULL,
  `password` varchar(100) NOT NULL,
  PRIMARY KEY (`platform_type`,`platform_user_id`),
  UNIQUE KEY `user_idx_UNIQUE` (`user_idx`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `login_user_info` */

insert  into `login_user_info`(`user_idx`,`platform_type`,`platform_user_id`,`db_idx`,`reg_date`,`last_login_date`,`password`) values 
(8,'00','alalssk',1,'2019-08-10 15:50:32','2019-08-10 15:50:32','74498063'),
(9,'00','alalssk1',1,'2019-08-10 15:51:33','2019-08-10 15:51:33','74498063'),
(7,'00','alalssk4',1,'2019-07-15 15:01:03','2019-07-15 06:01:03','1010229'),
(10,'00','jhh',1,'2019-08-10 16:10:16','2019-08-10 16:10:16','1111');

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
