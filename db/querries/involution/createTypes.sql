-- Active: 1755762981286@@127.0.0.1@5432
CREATE DOMAIN role_type AS VARCHAR(8)
CHECK (
    VALUE IN ('UNSIGNED', 'SIGNED', 'DUTY')
    AND VALUE IS NOT NULL
);

CREATE DOMAIN access_type AS VARCHAR(12)
CHECK (
    VALUE IN ('ACCESSIBLE', 'INACCESSIBLE')
    AND VALUE IS NOT NULL
);

CREATE DOMAIN nexus_type AS VARCHAR(10)
CHECK (
    VALUE IN ('STATION', 'TRANSITION', 'RAILWAY')
    AND VALUE IS NOT NULL
);

CREATE DOMAIN login_inst AS VARCHAR(255)
CHECK(
    LENGTH(VALUE) >= 0
);

CREATE DOMAIN password_inst AS VARCHAR(255)
CHECK(
    VALUE ~ '[A-ZА-Я]' AND
    VALUE ~ '[a-zа-я]' AND
    VALUE ~ '\d' AND
    VALUE ~ '^[A-ZА-Яa-zа-я0-9_!@#$%^&*()\-+=\[\]{}|:;,.?<>~`"]+$' AND
    LENGTH(VALUE) >= 6
);

CREATE DOMAIN mail_inst AS VARCHAR(255)
CHECK(
    VALUE ~ '^[a-zA-Z0-9!#$%&''*+/=?^_`{|}~-]+(\.[a-zA-Z0-9!#$%&''*+/=?^_`{|}~-]+)*@([a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$'
);

CREATE DOMAIN occupancy_level AS SMALLINT
CHECK(
    VALUE BETWEEN 0 AND 10
);

CREATE DOMAIN decimal_hexcolor AS INT
CHECK(
    VALUE BETWEEN 0 AND 16777215
);