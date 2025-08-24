WITH
    b_cnt AS (
        SELECT
            c.id,
            count(cb.id) AS branch_count
        FROM
            chart AS c
        JOIN
            chart_branch AS cb ON cb.chart_id = c.id
        GROUP BY
            c.id
    ),
    s_cnt AS (
        SELECT
            bs.branch_id,
            count(bs.id) AS station_count
        FROM
            branch_station AS bs
        JOIN
            chart_branch AS cb ON cb.branch_id = bs.branch_id
        GROUP BY
            bs.branch_id
    ),
    r_cnt AS (
        SELECT
            bs.branch_id,
            count(r.id) AS railway_count
        FROM
            railway AS r
        JOIN
            branch_station AS bs ON bs.station_id = r.from_id
        GROUP BY
            bs.branch_id
    ),
    t_cnt AS (
        SELECT
            cb.chart_id,
            cb.branch_id,
            st.station_id,
            st.transition_id
        FROM
            chart_branch AS cb
        JOIN
            branch_station AS bs ON bs.branch_id = cb.branch_id
        JOIN
            station_transition AS st ON st.station_id = bs.station_id
        GROUP BY
            cb.chart_id, cb.branch_id, st.station_id, st.transition_id
    )
-- SELECT s.branch_id, s.station_count, r.railway_count, t.transition_count
-- FROM s_cnt AS s
-- JOIN r_cnt AS r ON s.branch_id = r.branch_id
-- JOIN t_cnt as t ON s.branch_id = t.branch_id
SELECT *
FROM t_cnt