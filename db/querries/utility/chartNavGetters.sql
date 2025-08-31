CREATE OR REPLACE FUNCTION get_station_id_by_branch_id_station_title(
    p_branch_id INT,
    station_title TEXT
) RETURNS INT AS $$
DECLARE
    v_station_id INT;
BEGIN
    SELECT
        s.id INTO v_station_id
    FROM
        branch_station AS bs
    INNER JOIN
        station AS s ON bs.station_id = s.id
    WHERE
        bs.branch_id = p_branch_id AND
        s.title = station_title;
    
    RETURN v_station_id;
END;
$$ LANGUAGE plpgsql STABLE;


CREATE OR REPLACE FUNCTION get_station_id_by_branch_title_station_title(
    branch_title TEXT,
    station_title TEXT
) RETURNS INT AS $$
DECLARE
    v_station_id INT;
BEGIN
    SELECT
        s.id INTO v_station_id
    FROM
        branch_station AS bs
    INNER JOIN
        branch AS b ON bs.branch_id = b.id
    INNER JOIN
        station AS s ON bs.station_id = s.id
    WHERE
        b.title = branch_title AND
        s.title = station_title;
    
    RETURN v_station_id;
END;
$$ LANGUAGE plpgsql STABLE;