CREATE OR REPLACE FUNCTION hex_color_to_int(hex_color TEXT)
RETURNS INT AS $$
DECLARE
    cleaned_hex TEXT;
    full_hex TEXT;
    color_int INT;
BEGIN
    cleaned_hex := UPPER(TRIM(BOTH '#' FROM hex_color));

    IF LENGTH(cleaned_hex) = 3 THEN
        full_hex := CONCAT(
            SUBSTR(cleaned_hex, 1, 1), SUBSTR(cleaned_hex, 1, 1),
            SUBSTR(cleaned_hex, 2, 1), SUBSTR(cleaned_hex, 2, 1),
            SUBSTR(cleaned_hex, 3, 1), SUBSTR(cleaned_hex, 3, 1)
        );
    ELSIF LENGTH(cleaned_hex) = 6 THEN
        full_hex := cleaned_hex;
    ELSE
        RAISE EXCEPTION 'Неверный формат hex-цвета: %', hex_color;
    END IF;

    EXECUTE 'SELECT ' || quote_literal('x' || full_hex) || '::bit(24)::int'
    INTO color_int;

    RETURN color_int;
END;
$$ LANGUAGE plpgsql IMMUTABLE;


CREATE OR REPLACE FUNCTION int_color_to_hex(int_color INT)
RETURNS TEXT AS $$
DECLARE
    hex_color TEXT;
BEGIN
    IF int_color IS NULL OR int_color < 0 OR int_color > 16777215 THEN
        RAISE EXCEPTION 'Цвет должен быть в диапазоне от 0 до 16777215, получено: %', int_color;
    END IF;

    hex_color := LPAD(TO_HEX(int_color), 6, '0');

    RETURN '#' || LOWER(hex_color);
END;
$$ LANGUAGE plpgsql IMMUTABLE;