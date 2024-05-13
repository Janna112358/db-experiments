module Database

open Npgsql
open SqlHydra.Query
open Games.DbTypes

let connectionString =
    match System.Environment.GetEnvironmentVariable "CONNSTRING_GAMES_LOCAL" with
    | null -> "User ID=postgres;Password=mysecretpassword;Host=localhost;Database=games;"
    | conFromEnvironment -> conFromEnvironment

let openContextWithMappings () =
    task {
        let dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString)
        dataSourceBuilder.MapEnum<games.rating>("games.rating") |> ignore
        let dataSource = dataSourceBuilder.Build()
        let compiler = SqlKata.Compilers.PostgresCompiler()

        let! conn = dataSource.OpenConnectionAsync()
        let ctx = new QueryContext(conn, compiler)
        ctx.Logger <- printfn "SQL query: %O"
        return ctx
    }


let getRatings () =
    task {
        let ctx = ContextType.CreateTask openContextWithMappings

        let! ratings =
            selectTask HydraReader.Read ctx {
                for rating in games.ratings do
                    // select rating.geek_rating // works, numeric column
                    // select rating.my_rating // does not work, enum column
                    select rating // works, all columns
            }

        return ratings
    }

let getInfo () =
    task {
        let ctx = ContextType.CreateTask openContextWithMappings

        let! info =
            selectTask HydraReader.Read ctx {
                for info in games.games_with_info do
                    // select info.geek_rating // works, numeric column
                    // select info.my_rating // does not work, enum column
                    select info // does not work, all columns
            }

        return info
    }
