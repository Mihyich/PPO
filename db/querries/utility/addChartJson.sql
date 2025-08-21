-- Фунция сохранения схемы в json формате
CREATE OR REPLACE FUNCTION add_chart_json(
    p_json_data TEXT
) RETURNS INT AS $$
DECLARE
    v_json_data JSONB := p_json_data::JSONB;

    v_chart_id      INT;   -- айди созданной записи схемы
    v_branch_id     INT;   -- айди созданной записи ветки
    v_station_id    INT;   -- айди созданной записи станции
    v_transition_id INT;   -- айди созданной записи перехода

    v_branch_item     RECORD;
    v_station_item    RECORD;
    v_railway_item    RECORD;
    v_transition_item RECORD;
BEGIN
    IF p_json_data IS NULL OR NOT (p_json_data ~ '^[\s]*\{.*\}[\s]*$') THEN
        RAISE EXCEPTION 'Некорректный JSON: входные данные пусты или не объект';
    END IF;

    INSERT INTO chart(city, title)
    VALUES(
        v_json_data->>'city',
        v_json_data->>'title'
    )
    RETURNING id INTO v_chart_id;

    FOR v_branch_item IN
        SELECT * FROM jsonb_array_elements(v_json_data->'branches')
    LOOP

        INSERT INTO branch(title, color, access)
        VALUES(
            (v_branch_item.value)->>'title',
            hex_color_to_int((v_branch_item.value)->>'color'),
            ((v_branch_item.value)->>'accesstype')::access_type
        )
        RETURNING id INTO v_branch_id;

        INSERT INTO chart_branch(chart_id, branch_id)
        VALUES(
            v_chart_id,
            v_branch_id
        );

        FOR v_station_item IN
            SELECT * FROM jsonb_array_elements((v_branch_item.value)->'stations')
        LOOP

            INSERT INTO station(title, occupancy, access, open_time, close_time)
            VALUES(
                (v_station_item.value)->>'title',
                ((v_station_item.value)->>'occupancy')::SMALLINT,
                ((v_station_item.value)->>'accesstype')::access_type,
                ((v_station_item.value)->>'opentime')::TIME,
                ((v_station_item.value)->>'closetime')::TIME
            )
            RETURNING id INTO v_station_id;

            INSERT INTO branch_station(branch_id, station_id)
            VALUES(
                v_branch_id,
                v_station_id
            );

        END LOOP;

        FOR v_railway_item IN
            SELECT * FROM jsonb_array_elements((v_branch_item.value)->'railways')
        LOOP

            INSERT INTO railway(from_id, to_id, duration)
            VALUES(
                get_station_id_by_branch_id_station_title(v_branch_id, (v_railway_item.value)->>'from'),
                get_station_id_by_branch_id_station_title(v_branch_id, (v_railway_item.value)->>'to'),
                ((v_railway_item.value)->>'duration')::TIME
            );

        END LOOP;

    END LOOP;

    FOR v_transition_item IN
        SELECT * FROM jsonb_array_elements(v_json_data->'transitions')
    LOOP

        INSERT INTO transition(occupancy, access, duration, open_time, close_time)
        VALUES(
            ((v_transition_item.value)->>'occupancy')::SMALLINT,
            ((v_transition_item.value)->>'accesstype')::access_type,
            ((v_transition_item.value)->>'duration')::TIME,
            ((v_transition_item.value)->>'opentime')::TIME,
            ((v_transition_item.value)->>'closetime')::TIME
        )
        RETURNING id INTO v_transition_id;

        INSERT INTO station_transition(station_id, transition_id)
        VALUES (
            get_station_id_by_branch_title_station_title((v_transition_item.value)->>'branchsrc', (v_transition_item.value)->>'stationsrc'),
            v_transition_id
        );

        INSERT INTO station_transition(station_id, transition_id)
        VALUES (
            get_station_id_by_branch_title_station_title((v_transition_item.value)->>'branchdst', (v_transition_item.value)->>'stationdst'),
            v_transition_id
        );

    END LOOP;

    RETURN v_chart_id;
END;
$$ LANGUAGE plpgsql;