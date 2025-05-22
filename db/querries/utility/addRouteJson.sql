-- Фунция сохранения маршрута p_json_data для пользователя p_client_id для схемы p_chart_id
CREATE OR REPLACE FUNCTION add_route_json(
    p_client_id INT,
    p_chart_id INT,
    p_json_data JSONB
) RETURNS INT AS $$
DECLARE
    v_way_id INT;          -- айди созданной записи маршрута
    v_item RECORD;         -- обход массива звеньев маршрута  
    v_way_item_id INT;     -- айди созданного звена
    v_step_nomer INT := 1; -- номер звена маршрута
    v_branch_id INT;       -- айди найденной ветки
    v_station_id INT;      -- айди найденной станции
    v_station_from_id INT; -- айди станции: <ОТ>
    v_station_to_id INT;   -- айди станции: <НА>
    v_railway_id INT;      -- айди найденного переезда
    v_transition_id INT;   -- айди найденного перехода
BEGIN
    -- Создать запись нового маршрута
    INSERT INTO way (client_id, chart_id, title, init_date)
    VALUES (
        p_client_id,
        p_chart_id,
        p_json_data->>'Title',
        NOW()
    )
    RETURNING id INTO v_way_id;

    -- Вставка звеньев маршрута
    FOR v_item IN SELECT * FROM jsonb_array_elements(p_json_data->'RouteItems')
    LOOP
        -- Создать запись о звенье маршрута
        INSERT INTO way_item (way_id, nexus, step_nomer)
        VALUES (
            v_way_id,
            CASE 
                WHEN (v_item.value)->>'$type' = 'station' THEN 'STATION'::nexus_type
                WHEN (v_item.value)->>'$type' = 'railway' THEN 'RAILWAY'::nexus_type
                WHEN (v_item.value)->>'$type' = 'transition' THEN 'TRANSITION'::nexus_type
            END,
            v_step_nomer
        )
        RETURNING id INTO v_way_item_id;

        -- Конкретизация и связывание звяна с реальными станциями, переездами и перехода схемы
        CASE
            -- Станция
            WHEN (v_item.value)->>'$type' = 'station' THEN
                -- Поиск связываемой ветки (обновлять айди нужно только на каждую станцию и переход)
                SELECT
                    b.id INTO v_branch_id
                FROM 
                    branch AS b
                WHERE
                    b.title = (v_item.value)->>'BranchTitle' AND
                    EXISTS(
                        SELECT
                            1
                        FROM
                            chart_branch AS cb
                        WHERE
                            cb.chart_id = p_chart_id AND cb.branch_id = b.id
                    );

                -- Поиск связываемой станции
                SELECT
                    s.id INTO v_station_id
                FROM
                    station AS s
                WHERE
                    s.title = (v_item.value)->>'Title' AND
                    EXISTS(
                        SELECT
                            1
                        FROM
                            branch_station AS bs
                        WHERE
                            bs.branch_id = v_branch_id
                    );

                -- Вставка звена маршрута <Станция>
                INSERT INTO way_item_station (way_item_id, station_id)
                VALUES (v_way_item_id, v_station_id);

            -- Переезд
            WHEN (v_item.value)->>'$type' = 'railway' THEN
                -- Поиск отправной станции
                SELECT
                    s.id INTO v_station_from_id
                FROM
                    station AS s
                WHERE
                    s.title = (v_item.value)->>'FromStationTitle' AND
                    EXISTS(
                        SELECT
                            1
                        FROM
                            branch AS b
                        WHERE
                            b.id = v_branch_id
                    );
                
                -- Поиск станции назначения
                SELECT
                    s.id INTO v_station_to_id
                FROM
                    station AS s
                WHERE
                    s.title = (v_item.value)->>'ToStationTitle' AND
                    EXISTS(
                        SELECT
                            1
                        FROM
                            branch AS b
                        WHERE
                            b.id = v_branch_id
                    );

                -- Поиск связываемого переезда
                SELECT
                    r.id INTO v_railway_id
                FROM
                    railway AS r
                WHERE
                    r.from_id = v_station_from_id AND r.to_id = v_station_to_id;

                IF v_railway_id IS NULL THEN
                    RAISE EXCEPTION 'На шаге % не найден переезд между станциями % и % на ветке %',
                        v_step_nomer, v_station_from_id, v_station_to_id, v_branch_id;
                END IF;

                -- Вставка звена маршрута <Переезд>
                INSERT INTO way_item_railway (way_item_id, railway_id)
                VALUES (v_way_item_id, v_railway_id);
            
            -- Переход
            WHEN (v_item.value)->>'$type' = 'transition' THEN               
                -- Поиск первой связанной станции
                SELECT
                    s.id INTO v_station_from_id
                FROM
                    station AS s
                WHERE
                    s.Title = (v_item.value)->>'FromStationTitle' AND
                    EXISTS(
                        SELECT
                            1
                        FROM
                            branch AS b
                        WHERE
                            b.title = (v_item.value)->>'FromBranchTitle'
                    );
                
                -- Поиск второй связанной станции
                SELECT
                    s.id INTO v_station_to_id
                FROM
                    station AS s
                WHERE
                    s.Title = (v_item.value)->>'ToStationTitle' AND
                    EXISTS(
                        SELECT
                            1
                        FROM
                            branch AS b
                        WHERE
                            b.title = (v_item.value)->>'ToBranchTitle'
                    );
                
                -- Поиск связываемого перехода
                SELECT
                    st.transition_id INTO v_transition_id
                FROM
                    station_transition AS st
                WHERE
                    (st.station_id = v_station_from_id OR st.station_id = v_station_to_id) AND
                    EXISTS(
                        SELECT
                            1
                        FROM
                            branch_station AS bs1
                        WHERE
                            bs1.branch_id = v_branch_id AND bs1.station_id = st.station_id AND
                            EXISTS(
                                SELECT
                                    1
                                FROM
                                    branch_station AS bs2
                                WHERE
                                    bs2.branch_id != bs1.branch_id AND
                                    (bs2.station_id = v_station_from_id OR bs2.station_id = v_station_to_id)
                            )
                    );

                -- Вставка звена маршрута <Переход>
                INSERT INTO way_item_transition (way_item_id, transition_id)
                VALUES (v_way_item_id, v_transition_id);

        END CASE;

        v_step_nomer := v_step_nomer + 1;
    END LOOP;

    RETURN v_way_id;
END;
$$ LANGUAGE plpgsql;