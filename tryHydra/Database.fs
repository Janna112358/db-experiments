module Database

open Npgsql
open SqlHydra.Query
open Games.DbTypes

let private connectionString =
    match System.Environment.GetEnvironmentVariable "CONNSTRING_GAMES_LOCAL" with
    | null -> "User ID=postgres;Password=mysecretpassword;Host=localhost;Database=games;"
    | conFromEnvironment -> conFromEnvironment

let private dataSourceWithEnumMappings =
    let dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString)
    dataSourceBuilder.MapEnum<games.rating>("games.rating") |> ignore
    dataSourceBuilder.Build()

let private compiler = SqlKata.Compilers.PostgresCompiler()

let openContext () =
    task {
        let! conn = dataSourceWithEnumMappings.OpenConnectionAsync()
        let ctx = new QueryContext(conn, compiler)
        ctx.Logger <- printfn "SQL query for debugging: %O"
        return ctx
    }


let getRatings () =
    task {
        let ctx = ContextType.CreateTask openContext

        let! ratings =
            selectTask HydraReader.Read ctx {
                for row in games.ratings do
                    // select row.geek_rating // works, numeric column
                    // select row.my_rating // does not work, enum column
                    select row // works, all columns
            }

        return ratings
    }

let getInfo () =
    task {
        let ctx = ContextType.CreateTask openContext

        let! info =
            selectTask HydraReader.Read ctx {
                for row in games.games_with_info do
                    // select row.geek_rating // works, numeric column
                    // select row.my_rating // does not work, enum column
                    select row // works, but missing enum column
            }

        return info
    }
