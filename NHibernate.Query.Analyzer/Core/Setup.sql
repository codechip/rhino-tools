CREATE TABLE Projects
(
	id INTEGER PRIMARY KEY,
	name VARCHAR(255) NOT NULL
)
--$
CREATE TABLE Files
(
	id INTEGER PRIMARY KEY,
	filename varchar(255) NOT NULL,
	project_id INT NOT NULL
)
--$
CREATE TABLE Queries
(
	id INTEGER PRIMARY KEY,
	project_id INT NOT NULL,
	name varchar(255) NOT NULL,
	text TEXT NOT NULL
)