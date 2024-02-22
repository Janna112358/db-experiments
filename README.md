# db-experiments

Trying some stuff out interfacing with a database in fsharp

## database setup

Create the local database with:

```
psql -d postgres -U [username] -f sql/create_db.sql
``` 

Set an environment variable named "CONNSTRING_GAMES_LOCAL".


## SQLHydra

We use SqlHydra to automatically generate a "database types" module `DbTypes.fs` (in the repo). Regenerate the hydra file by running: 

```shell
dotnet sqlhydra npgsql --project db-experiments.fsproj
```

The first time you do this, you will be promted for some input parameters. These parameters are then saved in a file called `sqlhydra-npgsql.toml` so you don't have to give the input every time. If you want to change the params, such as which database is being used, you can edit the `.toml` file directly.

Choose/fill in the following parameters when prompted:

```shell
> Enter a database Connection String: Server=localhost;Database=games;Username=<username>;Password=<password>
> Enter an Output Filename (...): DbTypes.fs
> Enter a Namespace (...): Games.DbTypes
> Select a use case: Sqlhydra.Query integration (default)
```