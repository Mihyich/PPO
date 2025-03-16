-- Просмотр пользовательских ENUM
SELECT 
    n.nspname AS "Schema",
    p.typname AS "Type Name",
    pg_catalog.format_type(p.typbasetype, NULL) AS "Base Type",
    (SELECT array_agg(e.enumlabel) 
     FROM pg_enum e 
     WHERE e.enumtypid = p.oid) AS "Enum Values"
FROM 
    pg_catalog.pg_type p
JOIN 
    pg_catalog.pg_namespace n ON n.oid = p.typnamespace
WHERE 
    p.typtype = 'e' -- 'e' для ENUM
ORDER BY 
    "Schema", "Type Name";


-- Просмотр ролей
SELECT * FROM pg_user;