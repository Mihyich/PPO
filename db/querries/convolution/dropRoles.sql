ALTER DEFAULT PRIVILEGES IN SCHEMA public REVOKE ALL ON TABLES FROM duty;
ALTER DEFAULT PRIVILEGES IN SCHEMA public REVOKE ALL ON SEQUENCES FROM duty;
ALTER DEFAULT PRIVILEGES IN SCHEMA public REVOKE ALL ON FUNCTIONS FROM duty;
REVOKE unsigned_client FROM duty;
REVOKE signed_client FROM duty;
REVOKE UPDATE ON
    station,
    transition,
    railway
FROM duty;
DROP ROLE IF EXISTS duty;

ALTER DEFAULT PRIVILEGES IN SCHEMA public REVOKE ALL ON TABLES FROM signed_client;
ALTER DEFAULT PRIVILEGES IN SCHEMA public REVOKE ALL ON SEQUENCES FROM signed_client;
ALTER DEFAULT PRIVILEGES IN SCHEMA public REVOKE ALL ON FUNCTIONS FROM signed_client;
REVOKE unsigned_client FROM signed_client;
REVOKE SELECT, INSERT, UPDATE, DELETE ON
    client,
    way,
    way_item,
    way_item_railway,
    way_item_station,
    way_item_transition
FROM signed_client;
REVOKE USAGE ON SEQUENCE
    client_id_seq,
    way_id_seq,
    way_item_id_seq,
    way_item_railway_id_seq,
    way_item_station_id_seq,
    way_item_transition_id_seq
FROM signed_client;
REVOKE EXECUTE ON FUNCTION
    add_chart_json(TEXT),
    add_route_json(INT, INT, TEXT),
    get_route_json_by_id(INT)
FROM signed_client;
DROP ROLE IF EXISTS signed_client;

ALTER DEFAULT PRIVILEGES IN SCHEMA public REVOKE ALL ON TABLES FROM unsigned_client;
ALTER DEFAULT PRIVILEGES IN SCHEMA public REVOKE ALL ON SEQUENCES FROM unsigned_client;
ALTER DEFAULT PRIVILEGES IN SCHEMA public REVOKE ALL ON FUNCTIONS FROM unsigned_client;
REVOKE USAGE ON SCHEMA public FROM unsigned_client;
REVOKE SELECT ON
    client,
    chart,
    chart_branch,
    branch,
    branch_station,
    station,
    station_transition,
    transition,
    railway
FROM unsigned_client;
REVOKE EXECUTE ON FUNCTION
    get_chart_json_by_id(INT),
    get_station_id_by_branch_id_station_title(INT, TEXT),
    get_station_id_by_branch_title_station_title(TEXT, TEXT),
    hex_color_to_int(TEXT),
    int_color_to_hex(INT)
FROM unsigned_client;
DROP ROLE IF EXISTS unsigned_client;