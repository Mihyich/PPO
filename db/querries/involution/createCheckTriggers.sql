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
            'Попытка связать ветку (%), уже связанную со схемой (%), со схемой (%)',
            branch_id, chart_id, NEW.chart_id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


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
            'Попытка связать станцию (%), уже связанную с веткой (%), с веткой (%)',
            station_id, branch_id, NEW.branch_id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


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