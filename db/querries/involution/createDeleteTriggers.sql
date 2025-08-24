-- Удаление всех branch, которые были связаны с удаляемым chart
CREATE OR REPLACE FUNCTION delete_orphaned_branches_before_chart_delete()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM branch
    WHERE id IN (
        SELECT cb.branch_id
        FROM chart_branch AS cb
        WHERE cb.chart_id = OLD.id
    );
    
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;


-- Удаление всех Station, которые были связаны с удаляемым Branch
CREATE OR REPLACE FUNCTION delete_orphaned_stations_before_branch_delete()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM station
    WHERE id IN (
        SELECT bs.station_id
        FROM branch_station AS bs
        WHERE bs.branch_id = OLD.id
    );
    
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;


-- Удаление всех Transition, которые были связаны с удаляемым Station
CREATE OR REPLACE FUNCTION delete_orphaned_transitions_before_station_delete()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM transition
    WHERE id IN (
        SELECT st.transition_id
        FROM station_transition AS st
        WHERE st.station_id = OLD.id
    );
    
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;


-- Создание триггеров

CREATE OR REPLACE TRIGGER trg_delete_orphaned_branches_before_chart_delete
BEFORE DELETE ON chart
FOR EACH ROW EXECUTE FUNCTION delete_orphaned_branches_before_chart_delete();


CREATE OR REPLACE TRIGGER trg_delete_orphaned_stations_before_branch_delete
BEFORE DELETE ON branch
FOR EACH ROW EXECUTE FUNCTION delete_orphaned_stations_before_branch_delete();


CREATE OR REPLACE TRIGGER trg_delete_orphaned_transitions_before_station_delete
BEFORE DELETE ON station
FOR EACH ROW EXECUTE FUNCTION delete_orphaned_transitions_before_station_delete();