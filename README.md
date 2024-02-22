# db-experiments

Trying some stuff out interfacing with a database in fsharp

## database setup

Create the local database with:

```
psql -d postgres -U [username] -f sql/create_db.sql
``` 

Set an environment variable named "CONNSTRING_GAMES_LOCAL".