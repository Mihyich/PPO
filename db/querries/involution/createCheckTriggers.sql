CREATE OR REPLACE FUNCTION ck_branch_unique_chart_ref() RETURNS TRIGGER
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