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
        SELECT
            json_agg(
                json_build_object(
                    'occupancy', t.occupancy,
                    'accesstype', t.access,
                    'duration', t.duration,
                    'opentime', t.open_time,
                    'closetime', t.close_time,
                    'branchsrc', 'Неизвестно',
                    'stationsrc', 'Неизвестно',
                    'branchdst', 'Неизвестно',
                    'stationdst', 'Неизвестно'
                )
            )
        FROM
            transition AS t
        
        )
    ) AS metro_chart
FROM
    chart AS c
WHERE
    c.id = 1;