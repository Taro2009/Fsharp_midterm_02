IF DB_ID('Test') IS NOT NULL
	DROP DATABASE Test;  

CREATE DATABASE Test;

USE Test;

IF OBJECT_ID('dbo.Book', 'U') IS NOT NULL 
	DROP TABLE Book; 
CREATE TABLE Book (
	ID int NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    Author varchar(100) NOT NULL,
	Title varchar(100) NOT NULL,
    ReleaseDate varchar(30) NOT NULL	
);

INSERT INTO Book (Author, Title, ReleaseDate)
VALUES 
	('Teszt Tamas', 'Kitalalt konyv 1', '2020.11.11'),
	('Nemletezo Nandor', 'Valami cim', '2019.04.12'),
	('Telejesenmindegyhogyanhivjak Tamara', 'Random cim', '2002.01.12'),
	('Vala Ki', 'Vala Mi', '2011.04.15'
);
	