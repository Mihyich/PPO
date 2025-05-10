SELECT 
    n.nspname AS "Schema",
    p.proname AS "Function Name",
    pg_catalog.pg_get_function_result(p.oid) AS "Return Type",
    pg_catalog.pg_get_function_arguments(p.oid) AS "Arguments",
    CASE 
        WHEN p.prokind = 'a' THEN 'Aggregate'
        WHEN p.prokind = 'w' THEN 'Window'
        WHEN p.prokind = 'f' THEN 'Function'
        WHEN p.prokind = 'p' THEN 'Procedure'
    END AS "Type",
    l.lanname AS "Language"
FROM 
    pg_catalog.pg_proc p
LEFT JOIN 
    pg_catalog.pg_namespace n ON n.oid = p.pronamespace
LEFT JOIN 
    pg_catalog.pg_language l ON l.oid = p.prolang
WHERE 
    n.nspname NOT IN ('pg_catalog', 'information_schema')
    AND n.nspname NOT LIKE 'pg_%'
ORDER BY 
    "Schema", "Function Name";