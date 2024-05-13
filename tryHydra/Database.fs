module Database

open Npgsql
open SqlHydra.Query
open Games.DbTypes
open System.Data

let connectionString =
    "User ID=postgres;Password=mysecretpassword;Host=localhost;Database=games;"

let openContext () =
    let compiler = SqlKata.Compilers.PostgresCompiler()
    let conn = new NpgsqlConnection(connectionString)
    conn.Open()
    new QueryContext(conn, compiler)

let createConnectionWithMappings () =
    let dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString)
    dataSourceBuilder.MapEnum<games.rating>("games.rating") |> ignore
    let dataSource = dataSourceBuilder.Build()
    dataSource

let getRatings () =
    task {
        // Option 1 - use the dataSourceBuilder
        use datasource = createConnectionWithMappings ()
        use! conn = datasource.OpenConnectionAsync()

        // Option 2 - raw connection + global type mapper
        // use conn = new NpgsqlConnection(connectionString)
        // do! conn.OpenAsync()

        // use ctx =
        //     let compiler = SqlKata.Compilers.PostgresCompiler()
        //     new QueryContext(conn, compiler)

        use cmd = new NpgsqlCommand("SELECT * FROM games.ratings", conn)
        use reader = cmd.ExecuteReader(CommandBehavior.Default)
        let hReader = HydraReader reader

        let output = ResizeArray()

        while! reader.ReadAsync() do
            output.Add(hReader.``games.ratings``.Read())

        return output.ToArray()
    }
