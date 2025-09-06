CREATE OR REPLACE FUNCTION get_route_json_by_id(way_id INT)
RETURNS JSONB AS $$
DECLARE
    result_json JSONB;
BEGIN
    SELECT
        jsonb_build_object(
            'City', c.city,
            'ChartTitle', c.title,
            'Title', w.title,
            'Duration', w.duration,
            'RouteItems', COALESCE((
                WITH
                    wi AS (
                        SELECT
                            way_item.id,
                            way_item.way_id,
                            way_item.nexus,
                            way_item.step_nomer
                        FROM
                            way_item
                        WHERE
                            way_item.way_id = w.id
                    ),
                    ris AS (
                        SELECT
                            wis.way_item_id,
                            wis.station_id,
                            bs.branch_id,
                            s.title AS station_title,
                            b.title AS branch_title,
                            s.occupancy AS station_occupancy,
                            s.access AS station_access,
                            s.open_time AS station_open_time,
                            s.close_time AS station_close_time,
                            wi.step_nomer
                        FROM
                            wi
                        INNER JOIN
                            way_item_station AS wis ON wi.id = wis.way_item_id
                        INNER JOIN
                            station AS s ON s.id = wis.station_id
                        INNER JOIN
                            branch_station AS bs ON bs.station_id = s.id
                        INNER JOIN
                            branch AS b ON b.id = bs.branch_id
                    ),
                    rir AS (
                        SELECT
                            way_item_id,
                            branch_title,
                            from_station_title,
                            to_station_title,
                            railway_duration,
                            step_nomer
                        FROM (
                            SELECT
                                wir.way_item_id,
                                wi.nexus,
                                LAG(ris.branch_title) OVER (ORDER BY wi.step_nomer) AS branch_title,
                                LAG(ris.station_title) OVER (ORDER BY wi.step_nomer) AS from_station_title,
                                LEAD(ris.station_title) OVER (ORDER BY wi.step_nomer) AS to_station_title,
                                r.duration AS railway_duration,
                                wi.step_nomer
                            FROM
                                wi
                            LEFT JOIN
                                ris ON wi.id = ris.way_item_id
                            LEFT JOIN
                                way_item_railway AS wir ON wi.id = wir.way_item_id
                            LEFT JOIN
                                railway AS r ON wir.railway_id = r.id
                        )
                        WHERE
                            nexus = 'RAILWAY'::nexus_type
                    ),
                    rit AS (
                        SELECT
                            way_item_id,
                            from_station_title,
                            from_branch_title,
                            to_station_title,
                            to_branch_title,
                            transition_occupancy,
                            transition_access,
                            transition_duration,
                            transition_open_time,
                            transition_close_time,
                            step_nomer
                        FROM (
                            SELECT
                                wit.way_item_id,
                                wi.nexus,
                                LAG(ris.station_title) OVER (ORDER BY wi.step_nomer) AS from_station_title,
                                LAG(ris.branch_title) OVER (ORDER BY wi.step_nomer) AS from_branch_title,
                                LEAD(ris.station_title) OVER (ORDER BY wi.step_nomer) AS to_station_title,
                                LEAD(ris.branch_title) OVER (ORDER BY wi.step_nomer) AS to_branch_title,
                                t.occupancy AS transition_occupancy,
                                t.access AS transition_access,
                                t.duration AS transition_duration,
                                t.open_time AS transition_open_time,
                                t.close_time AS transition_close_time,
                                wi.step_nomer
                            FROM
                                wi
                            LEFT JOIN
                                ris ON wi.id = ris.way_item_id
                            LEFT JOIN
                                way_item_transition AS wit ON wi.id = wit.way_item_id
                            LEFT JOIN
                                transition AS t ON wit.transition_id = t.id
                            WHERE
                                wi.nexus != 'RAILWAY'::nexus_type
                        )
                        WHERE
                            nexus = 'TRANSITION'::nexus_type
                    ),
                    res AS (
                        SELECT
                            wi.step_nomer,
                            wi.nexus,
                            CASE wi.nexus
                                WHEN 'STATION'::nexus_type THEN ris.station_title
                                ELSE NULL
                            END AS station_title,
                            CASE wi.nexus
                                WHEN 'STATION'::nexus_type THEN ris.branch_title
                                WHEN 'RAILWAY'::nexus_type THEN rir.branch_title
                                ELSE NULL
                            END AS branch_title,
                            CASE wi.nexus
                                WHEN 'STATION'::nexus_type THEN ris.station_occupancy
                                ELSE NULL
                            END AS station_occupancy,
                            CASE wi.nexus
                                WHEN 'STATION'::nexus_type THEN ris.station_access
                                ELSE NULL
                            END AS station_access,
                            CASE wi.nexus
                                WHEN 'STATION'::nexus_type THEN ris.station_open_time
                                ELSE NULL
                            END AS station_open_time,
                            CASE wi.nexus
                                WHEN 'STATION'::nexus_type THEN ris.station_close_time
                                ELSE NULL
                            END AS station_close_time,
                            CASE wi.nexus
                                WHEN 'RAILWAY'::nexus_type THEN rir.from_station_title
                                WHEN 'TRANSITION'::nexus_type THEN rit.from_station_title
                                ELSE NULL
                            END AS from_station_title,
                            CASE wi.nexus
                                WHEN 'RAILWAY'::nexus_type THEN rir.to_station_title
                                WHEN 'TRANSITION'::nexus_type THEN rit.to_station_title
                                ELSE NULL
                            END AS to_station_title,
                            CASE wi.nexus
                                WHEN 'RAILWAY'::nexus_type THEN rir.railway_duration
                                ELSE NULL
                            END AS railway_duration,
                            CASE wi.nexus
                                WHEN 'TRANSITION'::nexus_type THEN rit.from_branch_title
                                ELSE NULL
                            END AS from_branch_title,
                            CASE wi.nexus
                                WHEN 'TRANSITION'::nexus_type THEN rit.to_branch_title
                                ELSE NULL
                            END AS to_branch_title,
                            CASE wi.nexus
                                WHEN 'TRANSITION'::nexus_type THEN rit.transition_occupancy
                                ELSE NULL
                            END AS transition_occupancy,
                            CASE wi.nexus
                                WHEN 'TRANSITION'::nexus_type THEN rit.transition_access
                                ELSE NULL
                            END AS transition_access,
                            CASE wi.nexus
                                WHEN 'TRANSITION'::nexus_type THEN rit.transition_duration
                                ELSE NULL
                            END AS transition_duration,
                            CASE wi.nexus
                                WHEN 'TRANSITION'::nexus_type THEN rit.transition_open_time
                                ELSE NULL
                            END AS transition_open_time,
                            CASE wi.nexus
                                WHEN 'TRANSITION'::nexus_type THEN rit.transition_close_time
                                ELSE NULL
                            END AS transition_close_time
                        FROM
                            wi
                        LEFT JOIN
                            ris ON wi.id = ris.way_item_id
                        LEFT JOIN
                            rir ON wi.id = rir.way_item_id
                        LEFT JOIN
                            rit ON wi.id = rit.way_item_id
                        ORDER BY
                            wi.step_nomer
                    )
                SELECT
                    jsonb_agg(
                        CASE res.nexus
                            WHEN 'STATION'::nexus_type THEN
                                jsonb_build_object(
                                    '$type', 'station',
                                    'Title', res.station_title,
                                    'BranchTitle', res.branch_title,
                                    'Occupancy', res.station_occupancy,
                                    'Access', res.station_access,
                                    'OpenTime', res.station_open_time,
                                    'CloseTime', res.station_close_time
                                )
                            WHEN 'RAILWAY'::nexus_type THEN
                                jsonb_build_object(
                                    '$type', 'railway',
                                    'BranchTitle', res.branch_title,
                                    'FromStationTitle', res.from_station_title,
                                    'ToStationTitle', res.to_station_title,
                                    'Duration', res.railway_duration
                                )
                            WHEN 'TRANSITION'::nexus_type THEN
                                jsonb_build_object(
                                    '$type', 'transition',
                                    'FromBranchTitle', res.from_branch_title,
                                    'FromStationTitle', res.from_station_title,
                                    'ToBranchTitle', res.to_branch_title,
                                    'ToStationTitle', res.to_station_title,
                                    'Occupancy', res.transition_occupancy,
                                    'Access', res.transition_access,
                                    'Duration', res.transition_duration,
                                    'OpenTime', res.transition_open_time,
                                    'CloseTime', res.transition_close_time
                                )
                            ELSE
                                jsonb_build_object('$type', 'unknown')
                        END
                    )
                FROM
                    res
            ), '[]'::JSONB)
        ) INTO result_json
    FROM
        way AS w
    INNER JOIN
        chart AS c ON w.chart_id = c.id
    WHERE
        w.id = way_id;


    RETURN result_json;
END;
$$ LANGUAGE plpgsql