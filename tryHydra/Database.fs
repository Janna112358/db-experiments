module Database

open Npgsql
open SqlHydra.Query
open Games.DbTypes

let connectionString =
    match System.Environment.GetEnvironmentVariable "CONNSTRING_GAMES_LOCAL" with
    | null -> failwith "Could not find local db connection string"
    | con -> con

let openContext () =
    let compiler = SqlKata.Compilers.PostgresCompiler()
    let conn = new NpgsqlConnection(connectionString)
    conn.Open()
    new QueryContext(conn, compiler)

let getInfo () =
    async {
        use ctx = openContext ()

        let! gamesInfo =
            selectAsync HydraReader.Read (Shared ctx) {
                // want this to use the materialized view, table<games.games_with_info> instead
                for row in table<games.games> do
                    select row
            }

        return gamesInfo
    }
