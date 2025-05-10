SELECT 
    n.nspname AS "Schema",
    t.typname AS "Domain Name",
    t.typnotnull AS "Not Null"
FROM 
    pg_catalog.pg_type AS t
JOIN 
    pg_catalog.pg_namespace AS n ON n.oid = t.typnamespace
LEFT JOIN 
    pg_catalog.pg_constraint AS c ON c.contypid = t.oid
WHERE 
    t.typtype = 'd' AND n.nspname = 'public' -- 'd' для DOMAIN
ORDER BY 
    "Schema", "Domain Name";