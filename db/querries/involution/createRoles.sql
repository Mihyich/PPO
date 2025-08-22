REVOKE CREATE ON SCHEMA public FROM PUBLIC;

-- UNSIGNED CLIENT --

CREATE ROLE unsigned_client WITH LOGIN PASSWORD 'qwer-tyui';

ALTER ROLE unsigned_client 
    WITH NOCREATEDB NOCREATEROLE NOREPLICATION NOINHERIT
    VALID UNTIL 'infinity';

GRANT USAGE ON SCHEMA public TO unsigned_client;

GRANT SELECT ON
    client,
    chart,
    chart_branch,
    branch,
    branch_station,
    station,
    station_transition,
    transition,
    railway
TO unsigned_client;

GRANT EXECUTE ON FUNCTION
    get_chart_json_by_id(INT),
    get_station_id_by_branch_id_station_title(INT, TEXT),
    get_station_id_by_branch_title_station_title(TEXT, TEXT),
    hex_color_to_int(TEXT),
    int_color_to_hex(INT)
TO unsigned_client;

-- SIGNED CLIENT --

CREATE ROLE signed_client WITH LOGIN PASSWORD 'asdf-ghjk';

ALTER ROLE signed_client 
    WITH NOCREATEDB NOCREATEROLE NOREPLICATION INHERIT
    VALID UNTIL 'infinity';

GRANT unsigned_client TO signed_client;

GRANT SELECT, INSERT, UPDATE, DELETE ON
    client,
    way,
    way_item,
    way_item_railway,
    way_item_station,
    way_item_transition
TO signed_client;

GRANT EXECUTE ON FUNCTION
    add_chart_json(TEXT),
    add_route_json(INT, INT, TEXT),
    get_route_json_by_id(INT)
TO signed_client;

-- DUTY --

CREATE ROLE duty WITH LOGIN PASSWORD 'zxcv-bnml';

ALTER ROLE duty 
    WITH NOCREATEDB NOCREATEROLE NOREPLICATION INHERIT
    VALID UNTIL 'infinity';

GRANT signed_client TO duty;

GRANT UPDATE ON
    station,
    transition,
    railway
TO signed_client;