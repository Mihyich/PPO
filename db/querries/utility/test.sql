-- 266 - chart

-- 2642 - 2625 branch

-- Перебор всех айди переходов определенной схемы
WITH
  local_t AS (SELECT DISTINCT st.transition_id FROM station_transition AS st
  INNER JOIN branch_station AS bs ON bs.station_id = st.station_id
  INNER JOIN chart_branch AS cb ON cb.branch_id = bs.branch_id
  INNER JOIN chart AS c ON c.id = cb.chart_id
  WHERE c.city = 'Минск')
SELECT * FROM transition AS t
INNER JOIN local_t AS lt ON lt.transition_id = t.id;

-- Получить все станции определенной схемы
select s.id, s.duty_id, s.title FROM station AS s
INNER JOIN branch_station AS bs ON bs.station_id = s.id
INNER JOIN chart_branch AS cb ON cb.branch_id = bs.branch_id
INNER JOIN chart AS c ON c.id = cb.chart_id
WHERE c.city = 'Минск';

WITH
  local_stations AS (SELECT s.id, s.duty_id, s.title FROM station AS s
    INNER JOIN branch_station AS bs ON bs.station_id = s.id
    INNER JOIN chart_branch AS cb ON cb.branch_id = bs.branch_id
    INNER JOIN chart AS c ON c.id = cb.chart_id
    WHERE c.city = 'Минск')
SELECT r.Id, r.duration, r.from_id, r.to_id FROM railway AS r
RIGHT JOIN local_stations AS ls ON r.to_id = ls.Id
RIGHT JOIN local_stations AS ols ON r.from_id = ols.Id
WHERE r.Id IS NOT NULL;



UPDATE station SET duty_id = 189
WHERE id BETWEEN 46073 AND 46108;

UPDATE transition SET duty_id = 189
WHERE id BETWEEN 17143 AND 17145;

