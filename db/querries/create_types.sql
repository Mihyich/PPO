CREATE OR REPLACE FUNCTION recreate_enum_type(enum_name TEXT, enum_values TEXT[])
RETURNS VOID AS $$
DECLARE
    enum_value TEXT;
BEGIN
    IF EXISTS (
        SELECT 1
        FROM pg_type
        WHERE typname = enum_name
    ) THEN
        EXECUTE format('DROP TYPE %I', enum_name);
    END IF;

    enum_value := array_to_string(ARRAY(SELECT quote_literal(value) FROM unnest(enum_values) AS value), ', ');
    EXECUTE format('CREATE TYPE %I AS ENUM (%s)', enum_name, enum_value);
END;
$$ LANGUAGE plpgsql;

SELECT recreate_enum_type('roletype', ARRAY['UNSIGNED', 'SIGNED', 'DUTY']);
SELECT recreate_enum_type('accesstype', ARRAY['ACCESSIBLE', 'INACCESSIBLE']);
SELECT recreate_enum_type('wayitemtype', ARRAY['STATION', 'TRANSITION', 'RAILWAY']);