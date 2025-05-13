SELECT
    json_build_object(
        'city', c.city,
        'title', c.title,
        'branches', (
        SELECT
            json_agg(
                json_build_object(
                    'title', b.title,
                    'color', b.color,
                    'accesstype', b.access,
                    'stations', (
                    SELECT
                        json_agg(
                            json_build_object(
                                'title', s.title,
                                'occupancy', s.occupancy,
                                'accesstype', s.access,
                                'opentime', s.open_time,
                                'closetime', s.close_time
                            )
                        )
                    FROM
                        station AS s
                    WHERE
                        EXISTS (
                            SELECT
                                1
                            FROM
                                branch_station AS bs
                            WHERE
                                bs.station_id = s.id AND bs.branch_id = b.id
                        )
                    ),
                    'railways', (
                    SELECT
                        json_agg(
                            json_build_object(
                                'from', (
                                SELECT
                                    s.title
                                FROM
                                    station AS s
                                WHERE
                                    s.id = r.from_id
                                ),
                                'to', (
                                SELECT
                                    s.title
                                FROM
                                    station AS s
                                WHERE
                                    s.id = r.to_id
                                ),
                                'duration', r.duration
                            )
                        )
                    FROM
                        railway AS r
                    WHERE
                        EXISTS (
                            SELECT
                                1
                            FROM
                                branch_station AS bs
                            JOIN
                                station AS s ON b.id = bs.branch_id AND s.id = bs.station_id AND s.id = r.from_id
                        )
                    )
                )
            )
        FROM
            branch AS b
        JOIN
            chart_branch AS cb ON cb.chart_id = c.id AND cb.branch_id = b.id
        ),
        'transitions', (
        WITH
            adj AS (
                SELECT
                    bs.branch_id,
                    bs.station_id,
                    t.id AS transition_id,
                    t.occupancy,
                    t.access,
                    t.duration,
                    t.open_time,
                    t.close_time
                FROM
                    chart_branch AS cb
                JOIN
                    branch_station AS bs ON cb.chart_id = c.id AND cb.branch_id = bs.branch_id
                JOIN
                    station_transition AS st ON st.station_id = bs.station_id
                JOIN
                    transition AS t ON t.id = st.transition_id
            ),
            r_adj AS (
                SELECT
                    adj.branch_id,
                    adj.station_id,
                    adj.transition_id,
                    rank() OVER (PARTITION BY transition_id ORDER BY branch_id, station_id)
                FROM
                    adj
            ),
            d_adj AS (
                SELECT DISTINCT ON (adj.transition_id)
                    *
                FROM
                    adj
                ORDER BY
                    adj.transition_id
            )
        SELECT
            json_agg(
                json_build_object(
                    'occupancy', d_adj.occupancy,
                    'accesstype', d_adj.access,
                    'duration', d_adj.duration,
                    'opentime', d_adj.open_time,
                    'closetime', d_adj.close_time,
                    'branchsrc', (
                        SELECT
                            b.title 
                        FROM
                            r_adj
                        JOIN
                            branch AS b ON r_adj.transition_id = d_adj.transition_id AND b.id = r_adj.branch_id AND r_adj.rank = 1
                    ),
                    'stationsrc', (
                        SELECT
                            s.title 
                        FROM
                            r_adj
                        JOIN
                            station AS s ON r_adj.transition_id = d_adj.transition_id AND s.id = r_adj.station_id AND r_adj.rank = 1
                    ),
                    'branchdst', (
                        SELECT
                            b.title 
                        FROM
                            r_adj
                        JOIN
                            branch AS b ON r_adj.transition_id = d_adj.transition_id AND b.id = r_adj.branch_id AND r_adj.rank = 2
                    ),
                    'stationdst', (
                        SELECT
                            s.title 
                        FROM
                            r_adj
                        JOIN
                            station AS s ON r_adj.transition_id = d_adj.transition_id AND s.id = r_adj.station_id AND r_adj.rank = 2
                    )
                )
            )
        FROM
            d_adj
        )
    ) AS metro_chart
FROM
    chart AS c
WHERE
    c.id = 3;