open Database
open Npgsql
open Games.DbTypes

// Option 2 - needed for raw connections
//NpgsqlConnection.GlobalTypeMapper.MapEnum<games.rating>("games.rating")

task {
    let! ratings = getRatings ()
    printfn $"Found {Seq.length ratings} entries in the games info materialized view."

    for rating in ratings do
        printfn $"Rating is {rating}"
}
|> _.Wait()
