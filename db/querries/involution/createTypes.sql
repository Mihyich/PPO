CREATE TYPE role_type AS ENUM ('UNSIGNED', 'SIGNED', 'DUTY');
CREATE TYPE access_type AS ENUM ('ACCESSIBLE', 'INACCESSIBLE');
CREATE TYPE nexus_type AS ENUM ('STATION', 'TRANSITION', 'RAILWAY');

CREATE DOMAIN login_inst AS VARCHAR(255)
CHECK(
    LENGTH(VALUE) >= 6
);

CREATE DOMAIN password_inst AS VARCHAR(255)
CHECK(
    VALUE ~ '[A-ZА-Я]' AND
    VALUE ~ '[a-zа-я]' AND
    VALUE ~ '\d' AND
    VALUE ~ '^[A-ZА-Яa-zа-я0-9_]+$' AND
    LENGTH(VALUE) >= 6
);

CREATE DOMAIN mail_inst AS VARCHAR(255)
CHECK(
    VALUE ~ '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$'
);

CREATE DOMAIN occupancy_level AS SMALLINT
CHECK(
    VALUE BETWEEN 0 AND 10
);

CREATE DOMAIN decimal_hexcolor AS INT
CHECK(
    VALUE BETWEEN 0 AND 16777215
);