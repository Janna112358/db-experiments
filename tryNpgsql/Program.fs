let getConnectionString () =
    match System.Environment.GetEnvironmentVariable "CONNSTRING_GAMES_LOCAL" with
    | null -> failwith "Could not find environment variable CONNSTRING_GAMES_LOCAL"
    | con -> con

let getMViews (conn: Npgsql.NpgsqlConnection) =
    // below is from the hydra code for regular views
    //     let sViews = conn.GetSchema("Views")
    //
    //     let views =
    //         sViews.Rows
    //         |> Seq.cast<System.Data.DataRow>
    //         |> Seq.map (fun tbl ->
    //             {| Catalog = tbl["TABLE_CATALOG"] :?> string
    //                Schema = tbl["TABLE_SCHEMA"] :?> string
    //                Name = tbl["TABLE_NAME"] :?> string
    //                Type = "view" |})

    let sMaterializedViews = conn.GetSchema("MaterializedViews")

    let materializedViews =
        sMaterializedViews.Rows
        |> Seq.cast<System.Data.DataRow>
        |> Seq.map (fun tbl ->
            {| Catalog = tbl["TABLE_CATALOG"] :?> string
               Schema = tbl["TABLE_SCHEMA"] :?> string
               Name = tbl["TABLE_NAME"] :?> string
               Type = "view" |})

    materializedViews

let getColumns (conn: Npgsql.NpgsqlConnection) =
    let sColumns = conn.GetSchema("Columns")

    sColumns.Rows
    |> Seq.cast<System.Data.DataRow>
    |> Seq.map (fun col ->
        {| TableCatalog = col["TABLE_CATALOG"] :?> string
           TableSchema = col["TABLE_SCHEMA"] :?> string
           TableName = col["TABLE_NAME"] :?> string
           ColumnName = col["COLUMN_NAME"] :?> string
           ProviderTypeName = col["DATA_TYPE"] :?> string
           OrdinalPosition = col["ORDINAL_POSITION"] :?> int
           IsNullable =
            match col["IS_NULLABLE"] :?> string with
            | "YES" -> true
            | _ -> false |})
    |> Seq.sortBy (fun column -> column.OrdinalPosition)

let getColumns2 (conn: Npgsql.NpgsqlConnection) =
    let sqlQuery =
        """
        SELECT 
            pg_namespace.nspname AS table_schema, 
            pg_class.relname AS table_name, 
            pg_attribute.attname AS column_name, 
            pg_attribute.attnum AS ordinal_position
        FROM pg_catalog.pg_class
        INNER JOIN pg_catalog.pg_namespace ON pg_class.relnamespace = pg_namespace.oid
        INNER JOIN pg_catalog.pg_attribute ON pg_class.oid = pg_attribute.attrelid
        WHERE 
            pg_attribute.attnum >= 1
        ORDER BY 
            table_schema, 
            table_name, 
            column_name
        """

    use cmd = new Npgsql.NpgsqlCommand(sqlQuery, conn)
    use rdr = cmd.ExecuteReader()

    [ while rdr.Read() do
          {| TableCatalog = "" //col["TABLE_CATALOG"] :?> string
             TableSchema = rdr.["TABLE_SCHEMA"] :?> string
             TableName = rdr.["TABLE_NAME"] :?> string
             ColumnName = rdr.["COLUMN_NAME"] :?> string
             ProviderTypeName = "" // col["DATA_TYPE"] :?> string
             OrdinalPosition = "" // col["ORDINAL_POSITION"] :?> int
          // IsNullable =
          //  match col["IS_NULLABLE"] :?> string with
          //  | "YES" -> true
          //  | _ -> false
          |} ]
    |> Set.ofList

[<EntryPoint>]
let main args =
    let connString = getConnectionString ()
    use conn = new Npgsql.NpgsqlConnection(connString)
    conn.Open()

    let mViewInfo = getMViews conn |> Seq.head
    printf $"first materialized view: {mViewInfo}\n"

    let mViewColumns =
        getColumns conn
        |> Seq.filter (fun c ->
            c.TableCatalog = mViewInfo.Catalog
            && c.TableSchema = mViewInfo.Schema
            && c.TableName = mViewInfo.Name)

    // nothing cause the materialized view columns aren't stored in the Information schema
    // which is apparently where GetSchema("Columns") gets its data from
    printfn "Attempt 1: materialized view columns?"
    mViewColumns |> Seq.iter (printfn "%A")

    let mViewColumns2 =
        getColumns2 conn
        |> Seq.filter (fun c ->
            // c.TableCatalog = mViewInfo.Catalog &&
            c.TableSchema = mViewInfo.Schema && c.TableName = mViewInfo.Name)

    printfn "Attempt 2: materialized view columns?"
    mViewColumns2 |> Seq.iter (printfn "%A")


    0
