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

// Getting the columns from the information schema with conn.GetSchema("Columns") will do us no good
// for materialized views, since they are not included in the information schema
// (no idea why. postgres docs say so)

// so instead, we do it the "hard" way by querying the pg_catalog
let getColumns (conn: Npgsql.NpgsqlConnection) =
    let sqlQuery =
        """
        SELECT 
            pg_namespace.nspname AS table_schema,
            pg_class.relname AS table_name, 
            pg_class.relkind, 
            pg_attribute.attname AS column_name,
            pg_attribute.attnum AS ordinal_position,
            pg_type.typname AS data_type,
            pg_attribute.attnotnull AS not_null
        FROM pg_class 
        INNER JOIN pg_namespace on (pg_class.relnamespace = pg_namespace.oid) 
        INNER JOIN pg_attribute on (pg_class.oid = pg_attribute.attrelid)
        INNER JOIN pg_type on (pg_attribute.atttypid = pg_type.oid)
        WHERE 
            -- get ordinary tables (r), views (v), and materialized views (m)
            relkind in ('r', 'v', 'm') AND
            -- filter out any "weird" columns 
            pg_attribute.attnum >= 1 AND
            -- filter out internal schemas
            pg_namespace.nspname not in ('pg_catalog', 'information_schema')
        ORDER BY 
            table_schema, 
            table_name, 
            ordinal_position
        ;
        """

    use cmd = new Npgsql.NpgsqlCommand(sqlQuery, conn)
    use rdr = cmd.ExecuteReader()

    [ while rdr.Read() do
          {|
             // the pg_catalog doesn't give us the database (catalog) name :(
             // TableCatalog = ""
             TableSchema = rdr.["TABLE_SCHEMA"] :?> string
             TableName = rdr.["TABLE_NAME"] :?> string
             ColumnName = rdr.["COLUMN_NAME"] :?> string
             ProviderTypeName = rdr.["DATA_TYPE"] :?> string
             OrdinalPosition = rdr.["ORDINAL_POSITION"] :?> int16 |> int
             IsNullable = rdr.["NOT_NULL"] :?> bool |> not |} ]
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
            // c.TableCatalog = mViewInfo.Catalog &&
            c.TableSchema = mViewInfo.Schema && c.TableName = mViewInfo.Name)

    printfn "\nMaterialized view columns?"
    mViewColumns |> Seq.iter (printfn "%A")

    0
