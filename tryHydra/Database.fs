module Database

open Npgsql
open SqlHydra.Query
open Games.DbTypes

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

        let cmd = new NpgsqlCommand("SELECT * FROM games.ratings", conn)
        use! reader = cmd.ExecuteReaderAsync()
        let output = ResizeArray()

        while! reader.ReadAsync() do
            reader.GetOrdinal "my_rating"
            |> reader.GetFieldValue<games.rating>
            |> output.Add

        return output.ToArray()
    }
