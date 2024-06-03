open Database
open Npgsql
open Games.DbTypes

task {
    printfn "\n---- Task 1: select from table ----\n"

    let! gameRatings = getRatings ()
    printfn $"Found {Seq.length gameRatings} entries in ratings table"

    match (Seq.tryHead gameRatings) with
    | Some game -> printfn $"The first game has ratings: \n{game}"
    | None -> printfn "No entries found"

}
|> _.Wait()

task {
    printfn "\n---- Task 2: select from mat. view ----\n"

    let! gameInfo = getInfo ()
    printfn $"Found {Seq.length gameInfo} entries in materialized view"

    match (Seq.tryHead gameInfo) with
    | Some game -> printfn $"The first game has info: \n{game}"
    | None -> printfn "No entries found"

}
|> _.Wait()
