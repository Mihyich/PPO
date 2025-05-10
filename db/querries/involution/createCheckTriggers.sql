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


-- Проверка связывания станций переходом в рамках разных веток одной и той же схемы
CREATE OR REPLACE FUNCTION ck_station_transition_different_branch_of_stations_ref() RETURNS TRIGGER
AS $$
DECLARE
    transition_links INT;
    station1_id INT;
    station2_id INT;
    branch1_id INT;
    branch2_id INT;
    chart1_id INT;
    chart2_id INT;
BEGIN
    -- Подсчет количества станций ведущих на один и тот же переход
    SELECT
        count(st.transition_id) INTO transition_links
    FROM
        station_transition AS st
    WHERE
        st.transition_id = NEW.transition_id;

    -- Гарантия, что только две станции ведут на переход
    IF transition_links < 2 THEN
        RETURN NEW; -- переход еще не полностью установлен
    ELSIF transition_links > 2 THEN
        RAISE EXCEPTION
            'Попытка связать переход (%) с больше чем 2-мя станциями',
            NEW.transition_id;
    END IF;

    -- Первая станция уже имеется в новой записи NEW
    SELECT
        NEW.station_id INTO station1_id;

    -- Вторая станция избирается с учетом того, что всего станций 2 и проверкой
    -- на несовпадение с NEW.station_id
    SELECT
        st.station_id INTO station2_id
    FROM
        station_transition AS st
    WHERE
        st.transition_id = NEW.transition_id AND st.station_id <> NEW.station_id
    LIMIT 1; -- Необязательно, но пусть будет

    -- Вторая станция должна быть найдена
    IF station2_id IS NULL THEN
        RAISE EXCEPTION
            'Не удалось определить вторую станцию для перехода %',
            NEW.transition_id;
    END IF;

    -- Поиск ветки и схемы для первой станции
    SELECT
        bs.branch_id, cb.chart_id INTO branch1_id, chart1_id
    FROM
        branch_station bs
    LEFT JOIN
        chart_branch cb ON cb.branch_id = bs.branch_id
    WHERE
        bs.station_id = station1_id
    LIMIT 1;
    
    -- Поиск ветки и схемы для второй станции
    SELECT
        bs.branch_id, cb.chart_id INTO branch2_id, chart2_id
    FROM
        branch_station bs
    LEFT JOIN
        chart_branch cb ON cb.branch_id = bs.branch_id
    WHERE
        bs.station_id = station2_id
    LIMIT 1;

    IF branch1_id IS NULL THEN
        RAISE EXCEPTION
            'Станция (%) не привязана к ветке',
            station1_id;
    ELSIF branch2_id IS NULL THEN
        RAISE EXCEPTION
            'Станция (%) не привязана к ветке',
            station2_id;
    ELSIF chart1_id IS NULL THEN
        RAISE EXCEPTION
            'Ветка (%) не привязана к схеме',
            branch1_id;
    ELSIF chart2_id IS NULL THEN
        RAISE EXCEPTION
            'Ветка (%) не привязана к схеме',
            branch2_id;
    ELSIF branch1_id = branch2_id THEN
        RAISE EXCEPTION
            'Попытка соединения переходом станций (%, %), принадлежащие одной и той же ветке: (%).',
            statoin1_id, station2_id, branch1_id;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


-- Создание триггеров

CREATE OR REPLACE TRIGGER trg_ck_chart_branch_unique_ref
	BEFORE INSERT OR UPDATE ON chart_branch
	FOR EACH ROW
	EXECUTE FUNCTION ck_chart_branch_unique_ref();


CREATE OR REPLACE TRIGGER trg_ck_branch_station_unique_ref
	BEFORE INSERT OR UPDATE ON chart_branch
	FOR EACH ROW
	EXECUTE FUNCTION ck_branch_station_unique_ref();


CREATE OR REPLACE TRIGGER trg_ck_railway_unique_station_ref
	BEFORE INSERT OR UPDATE OF from_id, to_id ON railway
	FOR EACH ROW
	EXECUTE FUNCTION ck_railway_unique_station_ref();


CREATE OR REPLACE TRIGGER trg_ck_railway_same_branch_of_stations_ref
	BEFORE INSERT OR UPDATE OF from_id, to_id ON railway
	FOR EACH ROW
	EXECUTE FUNCTION ck_railway_same_branch_of_stations_ref();


CREATE OR REPLACE TRIGGER trg_ck_station_transition_different_branch_of_stations_ref
	BEFORE INSERT OR UPDATE ON station_transition
	FOR EACH ROW
	EXECUTE FUNCTION ck_station_transition_different_branch_of_stations_ref();