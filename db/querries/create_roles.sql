DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_roles WHERE rolname = 'unsigned_client'
    ) THEN
        CREATE ROLE unsigned_client WITH LOGIN PASSWORD 'unsigned_client';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_roles WHERE rolname = 'signed_client'
    ) THEN
        CREATE ROLE signed_client WITH LOGIN PASSWORD 'signed_client';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_roles WHERE rolname = 'duty'
    ) THEN
        CREATE ROLE duty WITH LOGIN PASSWORD 'duty';
    END IF;
END $$;