-- Информативные таблицы

CREATE TABLE IF NOT EXISTS client (
	id              SERIAL PRIMARY KEY,
	client_login    login_inst,
	client_password password_inst,
	mail            mail_inst,
	privilege       role_type
);

CREATE TABLE IF NOT EXISTS chart (
	id              SERIAL PRIMARY KEY,
	city            VARCHAR(255),
	title           VARCHAR(255),
	svg_content     XML
);

CREATE TABLE IF NOT EXISTS branch (
	id              SERIAL PRIMARY KEY,
	title           VARCHAR(255),
	color           decimal_hexcolor,
	access          access_type
);

CREATE TABLE IF NOT EXISTS station (
	id              SERIAL PRIMARY KEY,
	duty_id         INT,
	title           VARCHAR(255),
	occupancy       occupancy_level,
	access          access_type,
	open_time       TIME,
	close_time      TIME
);

CREATE TABLE IF NOT EXISTS transition (
	id              SERIAL PRIMARY KEY,
	duty_id         INT,
	occupancy       occupancy_level,
	access          access_type,
	duration        TIME,
	open_time       TIME,
	close_time      TIME
);

CREATE TABLE IF NOT EXISTS way (
	id              SERIAL PRIMARY KEY,
	client_id       INT,
	chart_id        INT,
	title           VARCHAR(255),
	duration        TIME,
	init_date       TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS way_item (
	id              SERIAL PRIMARY KEY,
	way_id          INT,
	nexus           nexus_type, -- Тип звена маршрута [станция | переход | переезд]
	step_nomer      INT
);

-- Связующие таблицы

CREATE TABLE IF NOT EXISTS chart_branch (
	id              SERIAL PRIMARY KEY,
	chart_id        INT,
	branch_id       INT
);

CREATE TABLE IF NOT EXISTS branch_station (
	id              SERIAL PRIMARY KEY,
	branch_id       INT,
	station_id      INT
);

CREATE TABLE IF NOT EXISTS railway (
	id              SERIAL PRIMARY KEY,
	from_id         INT,
	to_id           INT,
	duration        TIME
);

CREATE TABLE IF NOT EXISTS station_transition (
	id              SERIAL PRIMARY KEY,
	station_id      INT,
	transition_id   INT
);

CREATE TABLE IF NOT EXISTS way_item_station (
	id              SERIAL PRIMARY KEY,
	way_item_id     INT,
	station_id      INT
);

CREATE TABLE IF NOT EXISTS way_item_railway (
	id              SERIAL PRIMARY KEY,
	way_item_id     INT,
	railway_id      INT
);

CREATE TABLE IF NOT EXISTS way_item_transition (
	id              SERIAL PRIMARY KEY,
	way_item_id     INT,
	transition_id   INT
);