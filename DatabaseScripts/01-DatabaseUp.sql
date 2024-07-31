CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240722134138_01-Initial') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240722134138_01-Initial') THEN

    CREATE TABLE `outboxEntry` (
        `id` int NOT NULL AUTO_INCREMENT,
        `payload` json NOT NULL,
        `sourcesystem` longtext CHARACTER SET utf8mb4 NOT NULL,
        `eventtype` longtext CHARACTER SET utf8mb4 NOT NULL,
        `processingStartDate` datetime(6) NULL,
        `processingCompletedDate` datetime(6) NULL,
        `status` int NOT NULL,
        `created` datetime(6) NOT NULL,
        CONSTRAINT `PK_outboxEntry` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240722134138_01-Initial') THEN

    CREATE TABLE `profileInfo` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `created` datetime(6) NOT NULL,
        CONSTRAINT `PK_profileInfo` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240722134138_01-Initial') THEN

    CREATE TABLE `profileIdentity` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `sub` varchar(250) CHARACTER SET utf8mb4 NOT NULL,
        `profileInfoId` int NOT NULL,
        `created` datetime(6) NOT NULL,
        CONSTRAINT `PK_profileIdentity` PRIMARY KEY (`Id`),
        CONSTRAINT `fk_profileIdentity_profile` FOREIGN KEY (`profileInfoId`) REFERENCES `profileInfo` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240722134138_01-Initial') THEN

    CREATE TABLE `profileInfoPersonName` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `profileInfoId` int NOT NULL,
        `family` varchar(250) CHARACTER SET utf8mb4 NOT NULL,
        `given` varchar(250) CHARACTER SET utf8mb4 NOT NULL,
        `created` datetime(6) NOT NULL,
        CONSTRAINT `PK_profileInfoPersonName` PRIMARY KEY (`Id`),
        CONSTRAINT `fk_profileInfoPersonName_profile` FOREIGN KEY (`profileInfoId`) REFERENCES `profileInfo` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240722134138_01-Initial') THEN

    CREATE INDEX `IX_profileIdentity_profileInfoId` ON `profileIdentity` (`profileInfoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240722134138_01-Initial') THEN

    CREATE INDEX `IX_profileInfoPersonName_profileInfoId` ON `profileInfoPersonName` (`profileInfoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240722134138_01-Initial') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20240722134138_01-Initial', '6.0.25');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

