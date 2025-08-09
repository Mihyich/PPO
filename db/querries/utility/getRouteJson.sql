CREATE OR REPLACE FUNCTION get_route_json_by_id(way_id INT)
RETURNS JSONB AS $$
DECLARE
    v_station_id INT;
    result_json JSONB;
BEGIN
    SELECT
        json_build_object(
            'Title', w.title,
            'Duration', w.duration,
            'RouteItems', (
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
                        ORDER BY
                            way_item.step_nomer
                    ),
                    ris AS (
                        SELECT
                            wis.way_item_id,
                            wis.station_id,
                            bs.branch_id,
                            s.title,
                            b.title AS branchtitle,
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
                        ORDER BY
                            wi.step_nomer
                    ),
                    rir AS (
                        SELECT
                            wir.way_item_id,
                            fs.title AS fromstationtitle,
                            ts.title AS tostationtitle,
                            wi.step_nomer
                        FROM
                            wi
                        INNER JOIN
                            way_item_railway AS wir ON wi.id = wir.way_item_id
                        INNER JOIN
                            ris AS fs ON fs.step_nomer = wi.step_nomer - 1
                        INNER JOIN
                            ris AS ts ON ts.step_nomer = wi.step_nomer + 1
                        ORDER BY
                            wi.step_nomer
                    ),
                    rit AS (
                        SELECT
                            wit.way_item_id,
                            fs.branchtitle AS frombranchtitle,
                            fs.title AS fromstationtitle,
                            ts.branchtitle AS tobranchtitle,
                            ts.title AS tostationtitle,
                            wi.step_nomer
                        FROM
                            wi
                        INNER JOIN
                            way_item_transition AS wit ON wi.id = wit.way_item_id
                        INNER JOIN
                            ris AS fs ON fs.step_nomer = wi.step_nomer - 1
                        INNER JOIN
                            ris AS ts ON ts.step_nomer = wi.step_nomer + 1
                        ORDER BY
                            wi.step_nomer
                    )
                SELECT
                    json_agg(
                        CASE wi.nexus
                            WHEN 'STATION'::nexus_type THEN
                                json_build_object(
                                    '$type', 'station',
                                    'Title', (SELECT ris.title FROM ris WHERE ris.step_nomer = wi.step_nomer),
                                    'BranchTitle', (SELECT ris.branchtitle FROM ris WHERE ris.step_nomer = wi.step_nomer)
                                )
                            WHEN 'RAILWAY'::nexus_type THEN
                                json_build_object(
                                    '$type', 'railway',
                                    'FromStationTitle', (SELECT rir.fromstationtitle FROM rir WHERE rir.step_nomer = wi.step_nomer),
                                    'ToStationTitle', (SELECT rir.tostationtitle FROM rir WHERE rir.step_nomer = wi.step_nomer)
                                )
                            WHEN 'TRANSITION'::nexus_type THEN
                                json_build_object(
                                    '$type', 'transition',
                                    'FromBranchTitle', (SELECT rit.frombranchtitle FROM rit WHERE rit.step_nomer = wi.step_nomer),
                                    'FromStationTitle', (SELECT rit.fromstationtitle FROM rit WHERE rit.step_nomer = wi.step_nomer),
                                    'ToBranchTitle', (SELECT rit.tobranchtitle FROM rit WHERE rit.step_nomer = wi.step_nomer),
                                    'ToStationTitle', (SELECT rit.tostationtitle FROM rit WHERE rit.step_nomer = wi.step_nomer)
                                )
                            ELSE
                                json_build_object('$type', 'unknown')
                        END
                    )
                FROM
                    wi
            )
        ) INTO result_json
    FROM
        way AS w
    WHERE
        w.id = way_id;


    RETURN result_json;
END;
$$ LANGUAGE plpgsql