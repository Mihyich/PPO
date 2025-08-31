CREATE OR REPLACE FUNCTION get_route_json_by_id(way_id INT)
RETURNS JSONB AS $$
DECLARE
    result_json JSONB;
BEGIN
    SELECT
        jsonb_build_object(
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
                            from_station_title,
                            to_station_title,
                            step_nomer
                        FROM (
                            SELECT
                                wir.way_item_id,
                                wi.nexus,
                                LAG(ris.station_title) OVER (ORDER BY wi.step_nomer) AS from_station_title,
                                LEAD(ris.station_title) OVER (ORDER BY wi.step_nomer) AS to_station_title,
                                wi.step_nomer
                            FROM
                                wi
                            LEFT JOIN
                                ris ON wi.id = ris.way_item_id
                            LEFT JOIN
                                way_item_railway AS wir ON wi.id = wir.way_item_id
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
                            step_nomer
                        FROM (
                            SELECT
                                wit.way_item_id,
                                wi.nexus,
                                LAG(ris.station_title) OVER (ORDER BY wi.step_nomer) AS from_station_title,
                                LAG(ris.branch_title) OVER (ORDER BY wi.step_nomer) AS from_branch_title,
                                LEAD(ris.station_title) OVER (ORDER BY wi.step_nomer) AS to_station_title,
                                LEAD(ris.branch_title) OVER (ORDER BY wi.step_nomer) AS to_branch_title,
                                wi.step_nomer
                            FROM
                                wi
                            LEFT JOIN
                                ris ON wi.id = ris.way_item_id
                            LEFT JOIN
                                way_item_transition AS wit ON wi.id = wit.way_item_id
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
                                ELSE NULL
                            END AS branch_title,
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
                                WHEN 'TRANSITION'::nexus_type THEN rit.from_branch_title
                                ELSE NULL
                            END AS from_branch_title,
                            CASE wi.nexus
                                WHEN 'TRANSITION'::nexus_type THEN rit.to_branch_title
                                ELSE NULL
                            END AS to_branch_title
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
                                    'BranchTitle', res.branch_title
                                )
                            WHEN 'RAILWAY'::nexus_type THEN
                                jsonb_build_object(
                                    '$type', 'railway',
                                    'FromStationTitle', res.from_station_title,
                                    'ToStationTitle', res.to_station_title
                                )
                            WHEN 'TRANSITION'::nexus_type THEN
                                jsonb_build_object(
                                    '$type', 'transition',
                                    'FromBranchTitle', res.from_branch_title,
                                    'FromStationTitle', res.from_station_title,
                                    'ToBranchTitle', res.to_branch_title,
                                    'ToStationTitle', res.to_station_title
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
    WHERE
        w.id = way_id;


    RETURN result_json;
END;
$$ LANGUAGE plpgsql