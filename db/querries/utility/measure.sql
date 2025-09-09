-- Active: 1755762981286@@127.0.0.1@5432@metro_test

CREATE OR REPLACE FUNCTION measure_size_mb()
RETURNS NUMERIC AS $$
DECLARE
    total_bytes BIGINT;
    total_mb NUMERIC;
BEGIN
    SELECT
        pg_indexes_size('chart') +
        pg_indexes_size('branch') +
        pg_indexes_size('station') +
        pg_indexes_size('railway') +
        pg_indexes_size('transition') +
        pg_indexes_size('chart_branch') +
        pg_indexes_size('branch_station') +
        pg_indexes_size('station_transition')
    INTO total_bytes;

    total_mb := total_bytes / 1024.0 / 1024.0;

    RETURN ROUND(total_mb, 2);
END;
$$ LANGUAGE plpgsql;

SELECT * FROM measure_size_mb();


CREATE OR REPLACE FUNCTION add_chart_json_with_timing(p_json_data TEXT)
RETURNS TABLE(chart_id INT, exec_time INTERVAL) AS $$
DECLARE
    start_ts TIMESTAMP;
    end_ts TIMESTAMP;
    v_chart_id INT;
BEGIN
    start_ts := clock_timestamp();
    v_chart_id := add_chart_json(p_json_data);
    end_ts := clock_timestamp();

    chart_id := v_chart_id;
    exec_time := end_ts - start_ts;

    RETURN NEXT;
END;
$$ LANGUAGE plpgsql;

CREATE INDEX IF NOT EXISTS idx_branch_station_branch_id ON branch_station(branch_id);
CREATE INDEX IF NOT EXISTS idx_station_title ON station(title);

CREATE INDEX IF NOT EXISTS idx_branch_station_branch_id_station_id ON branch_station(branch_id, station_id);

CREATE INDEX IF NOT EXISTS idx_branch_title ON branch(title);

CREATE INDEX IF NOT EXISTS idx_chart_branch_chart_id ON chart_branch(chart_id);
CREATE INDEX IF NOT EXISTS idx_chart_branch_branch_id ON chart_branch(branch_id);

CREATE INDEX IF NOT EXISTS idx_branch_station_station_id ON branch_station(station_id);

CREATE INDEX IF NOT EXISTS idx_station_transition_station_id ON station_transition(station_id);
CREATE INDEX IF NOT EXISTS idx_station_transition_transition_id ON station_transition(transition_id);

CREATE INDEX IF NOT EXISTS idx_railway_from_id ON railway(from_id);
CREATE INDEX IF NOT EXISTS idx_railway_to_id ON railway(to_id);

CREATE INDEX IF NOT EXISTS idx_chart_city ON chart(city);
CREATE INDEX IF NOT EXISTS idx_chart_title ON chart(title);

CREATE INDEX IF NOT EXISTS idx_station_occupancy ON station(occupancy);
CREATE INDEX IF NOT EXISTS idx_station_open_time ON station(open_time);
CREATE INDEX IF NOT EXISTS idx_station_close_time ON station(close_time);

CREATE INDEX IF NOT EXISTS idx_transition_duration ON transition(duration);
CREATE INDEX IF NOT EXISTS idx_transition_access ON transition(access);
CREATE INDEX IF NOT EXISTS idx_transition_open_time ON transition(open_time);
CREATE INDEX IF NOT EXISTS idx_transition_close_time ON transition(close_time);

SELECT
    schemaname AS schema,
    tablename AS table,
    indexname AS index,
    pg_size_pretty(pg_relation_size(indexname::regclass)) AS index_size,
    pg_size_pretty(pg_total_relation_size(tablename::regclass)) AS table_size
FROM
    pg_indexes
WHERE
    tablename IN (
        'chart', 'branch', 'station', 'railway',
        'transition', 'chart_branch', 'branch_station', 'station_transition'
    )
ORDER BY
    pg_relation_size(indexname::regclass) DESC;