SELECT 
    n.nspname AS "Schema",
    t.typname AS "Type Name",
    (SELECT
        array_agg(e.enumlabel) 
    FROM
        pg_enum AS e 
    WHERE
        e.enumtypid = t.oid
    ) AS "Enum Values",
    t.typnotnull AS "Not Null"
FROM 
    pg_catalog.pg_type AS t
JOIN 
    pg_catalog.pg_namespace AS n ON n.oid = t.typnamespace
WHERE 
    t.typtype = 'e' AND n.nspname = 'public' -- 'e' для ENUM
ORDER BY 
    "Schema", "Type Name";