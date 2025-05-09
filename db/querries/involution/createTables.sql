-- Информативные таблицы

CREATE TABLE IF NOT EXISTS client (
	id				SERIAL PRIMARY KEY,
	client_login	VARCHAR(255),
	client_password	VARCHAR(255),
	mail			VARCHAR(255),
	privilege		role_type NOT NULL
);

CREATE TABLE IF NOT EXISTS chart (
	id				SERIAL PRIMARY KEY,
	city			VARCHAR(255),
	title			VARCHAR(255),
	svg_content		XML NOT NULL
);

CREATE TABLE IF NOT EXISTS branch (
	id				SERIAL PRIMARY KEY,
	title			VARCHAR(255),
	color			decimal_hexcolor NOT NULL,
	access			access_type NOT NULL
);

CREATE TABLE IF NOT EXISTS station (
	id				SERIAL PRIMARY KEY,
	duty_id			INT,
	title			VARCHAR(255),
	occupancy		occupancy_level NOT NULL,
	access			access_type NOT NULL,
	open_time		TIME,
	close_time		TIME,

	FOREIGN KEY (duty_id) REFERENCES client (id)
);

CREATE TABLE IF NOT EXISTS transition (
	id				SERIAL PRIMARY KEY,
	duty_id			INT,
	occupancy		occupancy_level NOT NULL,
	access			access_type NOT NULL,
	duration		TIME,
	open_time		TIME,
	close_time		TIME,

	FOREIGN KEY (duty_id) REFERENCES client (id)
);

CREATE TABLE IF NOT EXISTS way (
	id				SERIAL PRIMARY KEY,
	client_id		INT NOT NULL,
	chart_id		INT NOT NULL,
	title			VARCHAR(255),
	init_date		TIMESTAMP,

	FOREIGN KEY (client_id) REFERENCES client (id),
	FOREIGN KEY (chart_id) REFERENCES chart (id),
	UNIQUE (client_id, title)
);

CREATE TABLE IF NOT EXISTS way_item (
	id				SERIAL PRIMARY KEY,
	way_id			INT NOT NULL,
	nexus			nexus_type NOT NULL,
	step_nomer		INT,
	
	FOREIGN KEY (way_id) REFERENCES way (id),
	UNIQUE (way_id, step_nomer)
);

-- Связующие таблицы

CREATE TABLE IF NOT EXISTS chart_branch (
	id				SERIAL PRIMARY KEY,
	chart_id		INT NOT NULL,
	branch_id		INT NOT NULL,

	FOREIGN KEY (chart_id) REFERENCES chart (id),
	FOREIGN KEY (branch_id) REFERENCES branch (id),
	UNIQUE (chart_id, branch_id)
);

CREATE TABLE IF NOT EXISTS branch_station (
	id				SERIAL PRIMARY KEY,
	branch_id		INT NOT NULL,
	station_id		INT NOT NULL,

	FOREIGN KEY (branch_id) REFERENCES branch (id),
	FOREIGN KEY (station_id) REFERENCES station (id),
	UNIQUE (branch_id, station_id)
);

CREATE TABLE IF NOT EXISTS railway (
	id				SERIAL PRIMARY KEY,
	from_id			INT NOT NULL,
	to_id			INT NOT NULL,
	duration		TIME,

	FOREIGN KEY (from_id) REFERENCES station (id),
	FOREIGN KEY (to_id) REFERENCES station (id),
	UNIQUE (from_id, to_id),
	CHECK (from_id <> to_id)
);

CREATE TABLE IF NOT EXISTS station_transition (
	id				SERIAL PRIMARY KEY,
	station_id		INT NOT NULL,
	transition_id	INT NOT NULL,

	FOREIGN KEY (station_id) REFERENCES station (id),
	FOREIGN KEY (transition_id) REFERENCES transition (id)
);

CREATE TABLE IF NOT EXISTS way_item_station (
	id				SERIAL PRIMARY KEY,
	station_id		INT NOT NULL,
	wayitem_id		INT NOT NULL,

	FOREIGN KEY (station_id) REFERENCES station (id),
	FOREIGN KEY (wayitem_id) REFERENCES way_item (id),
	UNIQUE (station_id, wayitem_id),
	UNIQUE (wayitem_id)
);

CREATE TABLE IF NOT EXISTS way_item_railway (
	id				SERIAL PRIMARY KEY,
	railway_id		INT NOT NULL,
	wayitem_id		INT NOT NULL,

	FOREIGN KEY (railway_id) REFERENCES railway (id),
	FOREIGN KEY (wayitem_id) REFERENCES way_item (id),
	UNIQUE (railway_id, wayitem_id),
	UNIQUE (wayitem_id)
);

CREATE TABLE IF NOT EXISTS way_item_transition (
	id				SERIAL PRIMARY KEY,
	transition_id	INT NOT NULL,
	wayitem_id		INT NOT NULL,
	
	FOREIGN KEY (transition_id) REFERENCES transition (id),
	FOREIGN KEY (wayitem_id) REFERENCES way_item (id),
	UNIQUE (transition_id, wayitem_id),
	UNIQUE (wayitem_id)
);