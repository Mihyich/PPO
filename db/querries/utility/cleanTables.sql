DELETE FROM way_item_transition;
DELETE FROM way_item_railway;
DELETE FROM way_item_station;
DELETE FROM station_transition;
DELETE FROM railway;
DELETE FROM branch_station;
DELETE FROM chart_branch;
DELETE FROM way_item;
DELETE FROM way;
DELETE FROM transition;
DELETE FROM station;
DELETE FROM branch;
DELETE FROM chart;
DELETE FROM client;

TRUNCATE TABLE 
    station_transition,
    railway,
    branch_station,
    chart_branch,
    transition,
    station,
    branch,
    chart;

ALTER SEQUENCE public.client_id_seq RESTART WITH 1;
ALTER SEQUENCE public.chart_id_seq RESTART WITH 1;
ALTER SEQUENCE public.branch_id_seq RESTART WITH 1;
ALTER SEQUENCE public.station_id_seq RESTART WITH 1;
ALTER SEQUENCE public.transition_id_seq RESTART WITH 1;
ALTER SEQUENCE public.way_id_seq RESTART WITH 1;
ALTER SEQUENCE public.way_item_id_seq RESTART WITH 1;
ALTER SEQUENCE public.chart_branch_id_seq RESTART WITH 1;
ALTER SEQUENCE public.branch_station_id_seq RESTART WITH 1;
ALTER SEQUENCE public.railway_id_seq RESTART WITH 1;
ALTER SEQUENCE public.station_transition_id_seq RESTART WITH 1;
ALTER SEQUENCE public.way_item_station_id_seq RESTART WITH 1;
ALTER SEQUENCE public.way_item_railway_id_seq RESTART WITH 1;
ALTER SEQUENCE public.way_item_transition_id_seq RESTART WITH 1;
