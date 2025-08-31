ALTER TABLE client
	ALTER COLUMN client_login SET NOT NULL,
	ALTER COLUMN client_password SET NOT NULL,
	ALTER COLUMN mail SET NOT NULL,
	ALTER COLUMN privilege SET DEFAULT 'UNSIGNED',
	ADD CONSTRAINT uk_client_login
		UNIQUE (client_login),
	ADD CONSTRAINT uk_client_mail
		UNIQUE (mail);


ALTER TABLE chart
	ALTER COLUMN city SET NOT NULL,
	ALTER COLUMN title SET NOT NULL,
	ALTER COLUMN svg_content SET DEFAULT '<svg width="200" height="200"><text y="16">Пустая схема</text></svg>',
	ADD CONSTRAINT ck_valid_svg
		CHECK (svg_content::xml IS DOCUMENT),
	ADD CONSTRAINT uk_chart_city_title
		UNIQUE (city, title); -- Не должно быть такого!!!


ALTER TABLE branch
	ALTER COLUMN title SET NOT NULL,
	ALTER COLUMN color SET DEFAULT 0,
	ALTER COLUMN access SET DEFAULT 'ACCESSIBLE';


ALTER TABLE station
	ALTER COLUMN duty_id DROP NOT NULL,
	ALTER COLUMN title SET NOT NULL,
	ALTER COLUMN occupancy SET DEFAULT 5,
	ALTER COLUMN access SET DEFAULT 'ACCESSIBLE',
	ALTER COLUMN open_time SET NOT NULL,
	ALTER COLUMN close_time SET NOT NULL,
	ADD CONSTRAINT ck_station_different_open_close_time
		CHECK (open_time <> close_time),
	ADD CONSTRAINT fk_station_duty_id
		FOREIGN KEY (duty_id) REFERENCES client (id) ON DELETE SET NULL;


ALTER TABLE transition
	ALTER COLUMN duty_id DROP NOT NULL,
	ALTER COLUMN occupancy SET DEFAULT 5,
	ALTER COLUMN access SET DEFAULT 'ACCESSIBLE',
	ALTER COLUMN duration SET NOT NULL,
	ALTER COLUMN open_time SET NOT NULL,
	ALTER COLUMN close_time SET NOT NULL,
	ADD CONSTRAINT ck_transition_different_open_close_time
		CHECK (open_time <> close_time),
	ADD CONSTRAINT fk_transition_duty_id
		FOREIGN KEY (duty_id) REFERENCES client (id) ON DELETE SET NULL;


ALTER TABLE way
	ALTER COLUMN client_id SET NOT NULL,
	ALTER COLUMN chart_id SET NOT NULL,
	ALTER COLUMN title SET NOT NULL,
	ALTER COLUMN init_date SET NOT NULL,
	ADD CONSTRAINT uk_way_client_id_title
		UNIQUE (client_id, title),
	ADD CONSTRAINT fk_way_client_id
		FOREIGN KEY (client_id) REFERENCES client (id) ON DELETE CASCADE,
	ADD CONSTRAINT fk_way_chart_id
		FOREIGN KEY (chart_id) REFERENCES chart (id) ON DELETE CASCADE;


ALTER TABLE way_item
	ALTER COLUMN way_id SET NOT NULL,
	ALTER COLUMN nexus SET NOT NULL,
	ALTER COLUMN step_nomer SET NOT NULL,
	ADD CONSTRAINT uk_way_item_way_id_step_nomer
		UNIQUE (way_id, step_nomer),
	ADD CONSTRAINT fk_way_item_way_id
		FOREIGN KEY (way_id) REFERENCES way (id) ON DELETE CASCADE;


ALTER TABLE chart_branch
	ALTER COLUMN chart_id SET NOT NULL,
	ALTER COLUMN branch_id SET NOT NULL,
	ADD CONSTRAINT uk_chart_branch_chart_id_branch_id
		UNIQUE (chart_id, branch_id), -- Ни одна схема не имеет дубликатов веток
	ADD CONSTRAINT uk_chart_branch_branch_id
		UNIQUE (branch_id), -- Каждая ветка связывается со схемой только один раз
	ADD CONSTRAINT fk_chart_branch_chart_id
		FOREIGN KEY (chart_id) REFERENCES chart (id) ON DELETE CASCADE,
	ADD CONSTRAINT fk_chart_branch_branch_id
		FOREIGN KEY (branch_id) REFERENCES branch (id) ON DELETE CASCADE;


ALTER TABLE branch_station
	ALTER COLUMN branch_id SET NOT NULL,
	ALTER COLUMN station_id SET NOT NULL,
	ADD CONSTRAINT uk_branch_station_branch_id_station_id
		UNIQUE (branch_id, station_id), -- Ни одна ветка не имеет дубликатов станций
	ADD CONSTRAINT uk_branch_station_station_id
		UNIQUE (station_id), -- Каждая станция связывается с веткой только один раз
	ADD CONSTRAINT fk_branch_station_branch_id
		FOREIGN KEY (branch_id) REFERENCES branch (id) ON DELETE CASCADE,
	ADD CONSTRAINT fk_branch_station_station_id
		FOREIGN KEY (station_id) REFERENCES station (id) ON DELETE CASCADE;


ALTER TABLE railway
	ALTER COLUMN from_id SET NOT NULL,
	ALTER COLUMN to_id SET NOT NULL,
	ALTER COLUMN duration SET NOT NULL,
	ADD CONSTRAINT uk_railway_from_id_to_id
		UNIQUE (from_id, to_id),
	ADD CONSTRAINT ck_railway_different_stations
		CHECK (from_id <> to_id),
	ADD CONSTRAINT fk_railway_from_id
		FOREIGN KEY (from_id) REFERENCES station (id) ON DELETE CASCADE,
	ADD CONSTRAINT fk_railway_to_id
		FOREIGN KEY (to_id) REFERENCES station (id) ON DELETE CASCADE;


ALTER TABLE station_transition
	ALTER COLUMN station_id SET NOT NULL,
	ALTER COLUMN transition_id SET NOT NULL,
	ADD CONSTRAINT uk_station_transition_station_id_transition_id
		UNIQUE (station_id, transition_id),
	ADD CONSTRAINT fk_station_transition_station_id
		FOREIGN KEY (station_id) REFERENCES station (id) ON DELETE CASCADE,
	ADD CONSTRAINT fk_station_transition_transition_id
		FOREIGN KEY (transition_id) REFERENCES transition (id) ON DELETE CASCADE;


ALTER TABLE way_item_station
	ALTER COLUMN way_item_id SET NOT NULL,
	ALTER COLUMN station_id SET NOT NULL,
	ADD CONSTRAINT uk_way_item_station_way_item_id_station_id
		UNIQUE (way_item_id, station_id),
	ADD CONSTRAINT uk_way_item_station_way_item_id
		UNIQUE (way_item_id),
	ADD CONSTRAINT fk_way_item_station_way_item_id
		FOREIGN KEY (way_item_id) REFERENCES way_item (id) ON DELETE CASCADE,
	ADD CONSTRAINT fk_way_item_station_station_id
		FOREIGN KEY (station_id) REFERENCES station (id) ON DELETE CASCADE;


ALTER TABLE way_item_railway
	ALTER COLUMN way_item_id SET NOT NULL,
	ALTER COLUMN railway_id SET NOT NULL,
	ADD CONSTRAINT uk_way_item_railway_way_item_id_railway_id
		UNIQUE (way_item_id, railway_id),
	ADD CONSTRAINT uk_way_item_railway_way_item_id
		UNIQUE (way_item_id),
	ADD CONSTRAINT fk_way_item_railway_way_item_id
		FOREIGN KEY (way_item_id) REFERENCES way_item (id) ON DELETE CASCADE,
	ADD CONSTRAINT fk_way_item_railway_railway_id
		FOREIGN KEY (railway_id) REFERENCES railway (id) ON DELETE CASCADE;


ALTER TABLE way_item_transition
	ALTER COLUMN way_item_id SET NOT NULL,
	ALTER COLUMN transition_id SET NOT NULL,
	ADD CONSTRAINT uk_way_item_transition_way_item_id_transition_id
		UNIQUE (way_item_id, transition_id),
	ADD CONSTRAINT uk_way_item_transition_way_item_id
		UNIQUE (way_item_id),
	ADD CONSTRAINT fk_way_item_transition_way_item_id
		FOREIGN KEY (way_item_id) REFERENCES way_item (id) ON DELETE CASCADE,
	ADD CONSTRAINT fk_way_item_transition_transition_id
		FOREIGN KEY (transition_id) REFERENCES transition (id) ON DELETE CASCADE;