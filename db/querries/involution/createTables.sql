-- Информативные таблицы

CREATE TABLE IF NOT EXISTS Client (
	ID SERIAL PRIMARY KEY,
	client_login VARCHAR(255),
	client_password VARCHAR(255),
	mail VARCHAR(255),
	role_type VARCHAR(8) NOT NULL CHECK (role_type IN ('UNSIGNED', 'SIGNED', 'DUTY'))
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
	color INT NOT NULL CHECK (color >= 0 AND color <= 16777215),
	access_type VARCHAR(12) NOT NULL CHECK (access_type IN ('ACCESSIBLE', 'INACCESSIBLE'))
);

CREATE TABLE IF NOT EXISTS Station (
	ID SERIAL PRIMARY KEY,
	dutyID INT,
	title VARCHAR(255),
	occupancy INT NOT NULL CHECK (occupancy >= 0 AND occupancy <= 10),
	access_type VARCHAR(12) NOT NULL CHECK (access_type IN ('ACCESSIBLE', 'INACCESSIBLE')),
	open_time TIME,
	close_time TIME,
	FOREIGN KEY (dutyID) REFERENCES Client (ID)
);

CREATE TABLE IF NOT EXISTS Transition (
	ID SERIAL PRIMARY KEY,
	dutyID INT,
	occupancy INT NOT NULL CHECK (occupancy >= 0 AND occupancy <= 10),
	access_type VARCHAR(12) NOT NULL CHECK (access_type IN ('ACCESSIBLE', 'INACCESSIBLE')),
	duration TIME,
	open_time TIME,
	close_time TIME,
	FOREIGN KEY (dutyID) REFERENCES Client (ID)
);

CREATE TABLE IF NOT EXISTS Way (
	ID SERIAL PRIMARY KEY,
	clientID INT NOT NULL,
	chartID INT NOT NULL,
	title VARCHAR(255),
	init_date TIMESTAMP,
	FOREIGN KEY (clientID) REFERENCES Client (ID),
	FOREIGN KEY (chartID) REFERENCES Chart (ID),
	UNIQUE (clientID, title)
);

CREATE TABLE IF NOT EXISTS WayItem (
	ID SERIAL PRIMARY KEY,
	wayID INT NOT NULL,
	way_type VARCHAR(10) NOT NULL CHECK (way_type IN ('STATION', 'TRANSITION', 'RAILWAY')),
	step_nomer INT,
	FOREIGN KEY (wayID) REFERENCES Way (ID),
	UNIQUE (wayID, step_nomer)
);

-- Связующие таблицы

CREATE TABLE IF NOT EXISTS ChartBranch (
	ID SERIAL PRIMARY KEY,
	chartID INT NOT NULL,
	branchID INT NOT NULL,
	FOREIGN KEY (chartID) REFERENCES Chart (ID),
	FOREIGN KEY (branchID) REFERENCES Branch (ID),
	UNIQUE (chartID, branchID)
);

CREATE TABLE IF NOT EXISTS BranchStation (
	ID SERIAL PRIMARY KEY,
	branchID INT NOT NULL,
	stationID INT NOT NULL,
	FOREIGN KEY (branchID) REFERENCES Branch (ID),
	FOREIGN KEY (stationID) REFERENCES Station (ID),
	UNIQUE (branchID, stationID)
);

CREATE TABLE IF NOT EXISTS Railway (
	ID SERIAL PRIMARY KEY,
	fromID INT NOT NULL,
	toID INT NOT NULL,
	duration TIME,
	FOREIGN KEY (fromID) REFERENCES Station (ID),
	FOREIGN KEY (toID) REFERENCES Station (ID),
	UNIQUE (fromID, toID),
	CHECK (fromID <> toID)
);

CREATE TABLE IF NOT EXISTS StationTransition (
	ID SERIAL PRIMARY KEY,
	stationID INT NOT NULL,
	transitionID INT NOT NULL,
	FOREIGN KEY (stationID) REFERENCES Station (ID),
	FOREIGN KEY (transitionID) REFERENCES Transition (ID)
);

CREATE TABLE IF NOT EXISTS WayItemStation (
	ID SERIAL PRIMARY KEY,
	stationID INT NOT NULL,
	wayitemID INT NOT NULL,
	FOREIGN KEY (stationID) REFERENCES Station (ID),
	FOREIGN KEY (wayitemID) REFERENCES WayItem (ID),
	UNIQUE (stationID, wayitemID),
	UNIQUE (wayitemID)
);

CREATE TABLE IF NOT EXISTS WayItemRailway (
	ID SERIAL PRIMARY KEY,
	railwayID INT NOT NULL,
	wayitemID INT NOT NULL,
	FOREIGN KEY (railwayID) REFERENCES Railway (ID),
	FOREIGN KEY (wayitemID) REFERENCES WayItem (ID),
	UNIQUE (railwayID, wayitemID),
    UNIQUE (wayitemID)
);

CREATE TABLE IF NOT EXISTS WayItemTransition (
	ID SERIAL PRIMARY KEY,
	transitionID INT NOT NULL,
	wayitemID INT NOT NULL,
	FOREIGN KEY (transitionID) REFERENCES Transition (ID),
	FOREIGN KEY (wayitemID) REFERENCES WayItem (ID),
	UNIQUE (transitionID, wayitemID),
    UNIQUE (wayitemID)
);