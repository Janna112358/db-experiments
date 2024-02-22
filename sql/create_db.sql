CREATE DATABASE games owner postgres;

\connect games; 

CREATE SCHEMA "people";
CREATE SCHEMA "games";

\i sql/create_tables.sql
\i sql/insert_data.sql
\i sql/create_views.sql