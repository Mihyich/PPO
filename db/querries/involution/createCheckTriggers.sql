CREATE OR REPLACE FUNCTION check_branch_unique_chart_ref() RETURNS TRIGGER
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
            'Дублирование переезда между станциями. (%, %) уже соединены переездом.', NEW.to_id, NEW.from_id;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;