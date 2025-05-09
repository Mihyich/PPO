ALTER TABLE client
	ALTER COLUMN client_login SET NOT NULL,
	ALTER COLUMN client_password SET NOT NULL,
	ALTER COLUMN mail SET NOT NULL,
	ALTER COLUMN privilege SET DEFAULT 'UNSIGNED';


ALTER TABLE chart
	ALTER COLUMN city SET NOT NULL,
	ALTER COLUMN title SET NOT NULL,
	ALTER COLUMN svg_content SET DEFAULT '<svg width="200" height="200"><text y="16">Пустая схема</text></svg>',
	ADD CONSTRAINT ck_valid_svg CHECK (svg_content::xml IS DOCUMENT);


ALTER TABLE branch
	ALTER COLUMN title SET NOT NULL,
	ALTER COLUMN color SET DEFAULT 0,
	ALTER COLUMN access SET DEFAULT 'ACCESSIBLE';


ALTER TABLE station
	ALTER COLUMN title SET NOT NULL,
	ALTER COLUMN occupancy SET DEFAULT 5,
	ALTER COLUMN access SET DEFAULT 'ACCESSIBLE',
	ALTER COLUMN open_time SET NOT NULL,
	ALTER COLUMN close_time SET NOT NULL;


ALTER TABLE transition
	ALTER COLUMN occupancy SET DEFAULT 5,
	ALTER COLUMN access SET DEFAULT 'ACCESSIBLE',
	ALTER COLUMN duration SET NOT NULL,
	ALTER COLUMN open_time SET NOT NULL,
	ALTER COLUMN close_time SET NOT NULL;


ALTER TABLE way
	ALTER COLUMN client_id SET NOT NULL,
	ALTER COLUMN chart_id SET NOT NULL,
	ALTER COLUMN title SET NOT NULL,
	ALTER COLUMN init_date SET NOT NULL,
	ADD CONSTRAINT uk_way_client_id_title UNIQUE (client_id, title);


ALTER TABLE way_item
	ALTER COLUMN way_id SET NOT NULL,
	ALTER COLUMN nexus SET NOT NULL,
	ALTER COLUMN step_nomer SET NOT NULL,
	ADD CONSTRAINT uk_way_item_way_id_step_nomer UNIQUE (way_id, step_nomer);


ALTER TABLE chart_branch
	ALTER COLUMN chart_id SET NOT NULL,
	ALTER COLUMN branch_id SET NOT NULL,
	ADD CONSTRAINT uk_chart_branch_chart_id_branch_id UNIQUE (chart_id, branch_id);


ALTER TABLE branch_station
	ALTER COLUMN branch_id SET NOT NULL,
	ALTER COLUMN station_id SET NOT NULL,
	ADD CONSTRAINT uk_branch_station_branch_id_station_id UNIQUE (branch_id, station_id);


ALTER TABLE railway
	ALTER COLUMN from_id SET NOT NULL,
	ALTER COLUMN to_id SET NOT NULL,
	ALTER COLUMN duration SET NOT NULL,
	ADD CONSTRAINT uk_railway_from_id_to_id UNIQUE (from_id, to_id),
	ADD CONSTRAINT ck_railway_different_stations CHECK (from_id <> to_id);


ALTER TABLE station_transition
	ALTER COLUMN station_id SET NOT NULL,
	ALTER COLUMN transition_id SET NOT NULL,
	ADD CONSTRAINT uk_station_transition_station_id_transition_id UNIQUE (station_id, transition_id);


ALTER TABLE way_item_station
	ALTER COLUMN way_item_id SET NOT NULL,
	ALTER COLUMN station_id SET NOT NULL,
	ADD CONSTRAINT uk_way_item_station_way_item_id_station_id UNIQUE (way_item_id, station_id),
	ADD CONSTRAINT uk_way_item_station_way_item_id UNIQUE (way_item_id);


ALTER TABLE way_item_railway
	ALTER COLUMN way_item_id SET NOT NULL,
	ALTER COLUMN railway_id SET NOT NULL,
	ADD CONSTRAINT uk_way_item_railway_way_item_id_railway_id UNIQUE (way_item_id, railway_id),
	ADD CONSTRAINT uk_way_item_railway_way_item_id UNIQUE (way_item_id);


ALTER TABLE way_item_transition
	ALTER COLUMN way_item_id SET NOT NULL,
	ALTER COLUMN transition_id SET NOT NULL,
	ADD CONSTRAINT uk_way_item_transition_way_item_id_transition_id UNIQUE (way_item_id, transition_id),
	ADD CONSTRAINT uk_way_item_transition_way_item_id UNIQUE (way_item_id);