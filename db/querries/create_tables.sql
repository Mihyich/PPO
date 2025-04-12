-- Информативные таблицы

CREATE TABLE IF NOT EXISTS Client (
	ID SERIAL PRIMARY KEY,
	client_login VARCHAR(255),
	client_password VARCHAR(255),
	mail VARCHAR(255),
	role_type "roletype"
);

CREATE TABLE IF NOT EXISTS Chart (
	ID SERIAL PRIMARY KEY,
	city VARCHAR(255),
	title VARCHAR(255),
	svg_content TEXT
);

CREATE TABLE IF NOT EXISTS Branch (
	ID SERIAL PRIMARY KEY,
	title VARCHAR(255),
	color INT, -- HEX число [0, 16777215]
	access_type "accesstype"
);

CREATE TABLE IF NOT EXISTS Station (
	ID SERIAL PRIMARY KEY,
	dutyID INT,
	title VARCHAR(255),
	occupancy INT, -- [0, 10]
	access_type "accesstype",
	open_time TIME,
	close_time TIME,
	FOREIGN KEY (dutyID) REFERENCES Client (ID)
);

CREATE TABLE IF NOT EXISTS Transition (
	ID SERIAL PRIMARY KEY,
	dutyID INT,
	occupancy INT, -- [0, 10]
	access_type "accesstype",
	duration TIME,
	open_time TIME,
	close_time TIME,
	FOREIGN KEY (dutyID) REFERENCES Client (ID)
);

CREATE TABLE IF NOT EXISTS Way (
	ID SERIAL PRIMARY KEY,
	clientID INT,
	chartID INT,
	title VARCHAR(255),
	init_date TIMESTAMP,
	FOREIGN KEY (clientID) REFERENCES Client (ID),
	FOREIGN KEY (chartID) REFERENCES Chart (ID)
);

CREATE TABLE IF NOT EXISTS WayItem (
	ID SERIAL PRIMARY KEY,
	wayID INT,
	chain_type "wayitemtype",
	step_nomer INT,
	FOREIGN KEY (wayID) REFERENCES Way (ID)
);

-- Связующие таблицы

CREATE TABLE IF NOT EXISTS ChartBranch (
	ID SERIAL PRIMARY KEY,
	chartID INT,
	branchID INT,
	FOREIGN KEY (chartID) REFERENCES Chart (ID),
	FOREIGN KEY (branchID) REFERENCES Branch (ID)
);

CREATE TABLE IF NOT EXISTS BranchStation (
	ID SERIAL PRIMARY KEY,
	branchID INT,
	stationID INT,
	FOREIGN KEY (branchID) REFERENCES Branch (ID),
	FOREIGN KEY (stationID) REFERENCES Station (ID)
);

CREATE TABLE IF NOT EXISTS Railway (
	ID SERIAL PRIMARY KEY,
	fromID INT,
	toID INT,
	duration TIME,
	FOREIGN KEY (fromID) REFERENCES Station (ID),
	FOREIGN KEY (toID) REFERENCES Station (ID)
);

CREATE TABLE IF NOT EXISTS StationTransition (
	ID SERIAL PRIMARY KEY,
	stationID INT,
	transitionID INT,
	FOREIGN KEY (stationID) REFERENCES Station (ID),
	FOREIGN KEY (transitionID) REFERENCES Transition (ID)
);

CREATE TABLE IF NOT EXISTS WayItemStation (
	ID SERIAL PRIMARY KEY,
	stationID INT,
	wayitemID INT,
	FOREIGN KEY (stationID) REFERENCES Station (ID),
	FOREIGN KEY (wayitemID) REFERENCES WayItem (ID)
);

CREATE TABLE IF NOT EXISTS WayItemRailway (
	ID SERIAL PRIMARY KEY,
	railwayID INT,
	wayitemID INT,
	FOREIGN KEY (railwayID) REFERENCES Railway (ID),
	FOREIGN KEY (wayitemID) REFERENCES WayItem (ID)
);

CREATE TABLE IF NOT EXISTS WayItemTransition (
	ID SERIAL PRIMARY KEY,
	transitionID INT,
	wayitemID INT,
	FOREIGN KEY (transitionID) REFERENCES Transition (ID),
	FOREIGN KEY (wayitemID) REFERENCES WayItem (ID)
);