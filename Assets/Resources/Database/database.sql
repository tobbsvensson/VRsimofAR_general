-- phpMyAdmin SQL Dump
-- version 4.7.9
-- https://www.phpmyadmin.net/
--
-- Host: mysql-levelup.alwaysdata.net
-- Generation Time: Aug 28, 2019 at 12:34 PM
-- Server version: 10.2.22-MariaDB
-- PHP Version: 7.2.9

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `levelup_newsvr`
--

-- --------------------------------------------------------

--
-- Table structure for table `COMMENTS`
--

CREATE TABLE `COMMENTS` (
  `idComment` int(10) UNSIGNED NOT NULL,
  `idNews` int(10) UNSIGNED NOT NULL,
  `idPlayer` int(10) UNSIGNED NOT NULL,
  `text` text NOT NULL,
  `date` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Triggers `COMMENTS`
--
DELIMITER $$
CREATE TRIGGER `after_delete_comment` AFTER DELETE ON `COMMENTS` FOR EACH ROW BEGIN
UPDATE PLAYERS SET nbOfComment = nbOfComment - 1 WHERE idPlayer = old.idPlayer;
UPDATE NEWS SET nbComment = nbComment - 1 WHERE idNews = old.idNews;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `after_insert_comment` AFTER INSERT ON `COMMENTS` FOR EACH ROW BEGIN
UPDATE PLAYERS SET nbOfComment = nbOfComment + 1 WHERE idPlayer = new.idPlayer;
UPDATE NEWS SET nbComment = nbComment + 1 WHERE idNews = new.idNews;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `FAVORITES`
--

CREATE TABLE `FAVORITES` (
  `idPlayer` int(10) UNSIGNED NOT NULL,
  `idNews` int(10) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `MEDIA`
--

CREATE TABLE `MEDIA` (
  `idMedia` int(10) UNSIGNED NOT NULL,
  `idNews` int(10) UNSIGNED NOT NULL,
  `link` text NOT NULL,
  `type` tinyint(3) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `NEWS`
--

CREATE TABLE `NEWS` (
  `idNews` int(10) UNSIGNED NOT NULL,
  `title` varchar(80) NOT NULL,
  `text` text NOT NULL,
  `author` varchar(50) NOT NULL,
  `creationDate` datetime NOT NULL,
  `nbView` int(10) UNSIGNED NOT NULL,
  `nbComment` int(10) UNSIGNED NOT NULL,
  `nbHappy` int(10) UNSIGNED NOT NULL,
  `nbSad` int(10) UNSIGNED NOT NULL,
  `nbAngry` int(10) UNSIGNED NOT NULL,
  `nbSurprised` int(10) UNSIGNED NOT NULL,
  `positionX` float NOT NULL,
  `positionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `NOTIFICATIONS`
--

CREATE TABLE `NOTIFICATIONS` (
  `idNotif` int(10) UNSIGNED NOT NULL,
  `tagName` varchar(30) NOT NULL,
  `idPlayer` int(10) UNSIGNED NOT NULL,
  `color` varchar(15) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `PLAYERS`
--

CREATE TABLE `PLAYERS` (
  `idPlayer` int(10) UNSIGNED NOT NULL,
  `name` varchar(20) NOT NULL,
  `password` varchar(20) NOT NULL,
  `nbOfView` mediumint(8) UNSIGNED NOT NULL,
  `nbOfComment` mediumint(8) UNSIGNED NOT NULL,
  `cmtPositionPref` tinyint(3) UNSIGNED NOT NULL,
  `cmtNbShown` tinyint(3) UNSIGNED NOT NULL,
  `lastPlayDate` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `TAGS`
--

CREATE TABLE `TAGS` (
  `tagLabel` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `TOPICS`
--

CREATE TABLE `TOPICS` (
  `idTopic` int(10) UNSIGNED NOT NULL,
  `tagName` varchar(30) NOT NULL,
  `idNews` int(10) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `VIEWS`
--

CREATE TABLE `VIEWS` (
  `idPlayer` int(10) UNSIGNED NOT NULL,
  `idNews` int(10) UNSIGNED NOT NULL,
  `dateLatestView` datetime NOT NULL,
  `reactionSelected` tinyint(3) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `COMMENTS`
--
ALTER TABLE `COMMENTS`
  ADD PRIMARY KEY (`idComment`),
  ADD KEY `idNews` (`idNews`),
  ADD KEY `idPlayer` (`idPlayer`);

--
-- Indexes for table `FAVORITES`
--
ALTER TABLE `FAVORITES`
  ADD KEY `idPlayer` (`idPlayer`,`idNews`),
  ADD KEY `idNews` (`idNews`);

--
-- Indexes for table `MEDIA`
--
ALTER TABLE `MEDIA`
  ADD PRIMARY KEY (`idMedia`),
  ADD KEY `idNews` (`idNews`);

--
-- Indexes for table `NEWS`
--
ALTER TABLE `NEWS`
  ADD PRIMARY KEY (`idNews`);

--
-- Indexes for table `NOTIFICATIONS`
--
ALTER TABLE `NOTIFICATIONS`
  ADD PRIMARY KEY (`idNotif`),
  ADD KEY `idTag` (`tagName`),
  ADD KEY `idPlayer` (`idPlayer`);

--
-- Indexes for table `PLAYERS`
--
ALTER TABLE `PLAYERS`
  ADD PRIMARY KEY (`idPlayer`);

--
-- Indexes for table `TAGS`
--
ALTER TABLE `TAGS`
  ADD UNIQUE KEY `tagLabel` (`tagLabel`);

--
-- Indexes for table `TOPICS`
--
ALTER TABLE `TOPICS`
  ADD PRIMARY KEY (`idTopic`),
  ADD KEY `idTag` (`tagName`,`idNews`),
  ADD KEY `idNews` (`idNews`);

--
-- Indexes for table `VIEWS`
--
ALTER TABLE `VIEWS`
  ADD UNIQUE KEY `idPlayer_2` (`idPlayer`,`idNews`),
  ADD KEY `VIEW_ibfk_1` (`idNews`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `COMMENTS`
--
ALTER TABLE `COMMENTS`
  MODIFY `idComment` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=108;

--
-- AUTO_INCREMENT for table `MEDIA`
--
ALTER TABLE `MEDIA`
  MODIFY `idMedia` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT for table `NEWS`
--
ALTER TABLE `NEWS`
  MODIFY `idNews` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;

--
-- AUTO_INCREMENT for table `NOTIFICATIONS`
--
ALTER TABLE `NOTIFICATIONS`
  MODIFY `idNotif` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=257;

--
-- AUTO_INCREMENT for table `PLAYERS`
--
ALTER TABLE `PLAYERS`
  MODIFY `idPlayer` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT for table `TOPICS`
--
ALTER TABLE `TOPICS`
  MODIFY `idTopic` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=31;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `COMMENTS`
--
ALTER TABLE `COMMENTS`
  ADD CONSTRAINT `COMMENTS_ibfk_1` FOREIGN KEY (`idNews`) REFERENCES `NEWS` (`idNews`) ON DELETE CASCADE,
  ADD CONSTRAINT `COMMENTS_ibfk_2` FOREIGN KEY (`idPlayer`) REFERENCES `PLAYERS` (`idPlayer`) ON DELETE CASCADE;

--
-- Constraints for table `FAVORITES`
--
ALTER TABLE `FAVORITES`
  ADD CONSTRAINT `FAVORITES_ibfk_1` FOREIGN KEY (`idNews`) REFERENCES `NEWS` (`idNews`),
  ADD CONSTRAINT `FAVORITES_ibfk_2` FOREIGN KEY (`idPlayer`) REFERENCES `PLAYERS` (`idPlayer`);

--
-- Constraints for table `MEDIA`
--
ALTER TABLE `MEDIA`
  ADD CONSTRAINT `MEDIA_ibfk_1` FOREIGN KEY (`idNews`) REFERENCES `NEWS` (`idNews`) ON DELETE CASCADE;

--
-- Constraints for table `NOTIFICATIONS`
--
ALTER TABLE `NOTIFICATIONS`
  ADD CONSTRAINT `NOTIFICATIONS_ibfk_1` FOREIGN KEY (`idPlayer`) REFERENCES `PLAYERS` (`idPlayer`) ON DELETE CASCADE,
  ADD CONSTRAINT `NOTIFICATIONS_ibfk_2` FOREIGN KEY (`tagName`) REFERENCES `TAGS` (`tagLabel`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `TOPICS`
--
ALTER TABLE `TOPICS`
  ADD CONSTRAINT `TOPICS_ibfk_2` FOREIGN KEY (`idNews`) REFERENCES `NEWS` (`idNews`) ON DELETE CASCADE,
  ADD CONSTRAINT `TOPICS_ibfk_3` FOREIGN KEY (`tagName`) REFERENCES `TAGS` (`tagLabel`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `VIEWS`
--
ALTER TABLE `VIEWS`
  ADD CONSTRAINT `VIEWS_ibfk_1` FOREIGN KEY (`idNews`) REFERENCES `NEWS` (`idNews`) ON DELETE CASCADE,
  ADD CONSTRAINT `VIEWS_ibfk_2` FOREIGN KEY (`idPlayer`) REFERENCES `PLAYERS` (`idPlayer`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
