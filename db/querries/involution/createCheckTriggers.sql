-- Проверка корректного добавления связи Схема--Ветка
CREATE OR REPLACE FUNCTION ck_chart_branch_unique_ref() RETURNS TRIGGER
AS $$
BEGIN
    IF EXISTS (
        SELECT
            1
        FROM
            chart_branch
        WHERE
            chart_id <> NEW.chart_id AND branch_id = NEW.branch_id
    ) THEN
        RAISE EXCEPTION
            'Попытка связать ветку (%), уже связанную со схемой (%), со схемой (%).',
            branch_id, chart_id, NEW.chart_id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


-- Проверка корректного добавления связи Ветка--Станция
CREATE OR REPLACE FUNCTION ck_branch_station_unique_ref() RETURNS TRIGGER
AS $$
BEGIN
    IF EXISTS (
        SELECT
            1
        FROM
            branch_station
        WHERE
            branch_id <> NEW.branch_id AND station_id = NEW.station_id
    ) THEN
        RAISE EXCEPTION
            'Попытка связать станцию (%), уже связанную с веткой (%), с веткой (%).',
            station_id, branch_id, NEW.branch_id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


-- Проверка дублирования переездов между станциями
CREATE OR REPLACE FUNCTION ck_railway_unique_station_ref() RETURNS TRIGGER
AS $$
BEGIN
    IF EXISTS (
        SELECT
            1
        FROM
            railway
        WHERE
            from_id = NEW.to_id AND to_id = NEW.from_id
    ) THEN
        RAISE EXCEPTION
            'Дублирование переезда между станциями. Станции (%, %) уже соединены переездом: (%, %).',
            NEW.from_id, NEW.to_id, from_id, to_id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


-- Проверка связывания станций переездом в рамках одной ветки
CREATE OR REPLACE FUNCTION ck_railway_same_branch_of_stations_ref() RETURNS TRIGGER
AS $$
DECLARE
    branch1_id INT;
    branch2_id INT;
BEGIN
    SELECT
        branch_id INTO branch1_id
    FROM
        branch_station AS bs
    WHERE
        NEW.from_id = bs.station_id
    LIMIT 1; -- Необязательно, но пусть будет

    SELECT
        branch_id INTO branch2_id
    FROM
        branch_station AS bs
    WHERE
        NEW.to_id = bs.station_id
    LIMIT 1; -- Необязательно, но пусть будет

    IF branch1_id IS NULL THEN
        RAISE EXCEPTION
            'Станция % не привязана ни к одной ветке',
            NEW.from_id;
    ELSIF branch2_id IS NULL THEN
        RAISE EXCEPTION
            'Станция % не привязана ни к одной ветке',
            NEW.to_id;
    ELSIF branch1_id <> branch2_id THEN
        RAISE EXCEPTION
            'Попытка соединения переездом станций (%, %), принадлежащие разным веткам: (%, %).',
            NEW.from_id, NEW.to_id, branch1_id, branch2_id;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;