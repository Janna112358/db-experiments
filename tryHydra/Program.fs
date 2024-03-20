open Database

let program () =
    async {
        let! gamesInfo = getInfo ()
        printfn $"Found {Seq.length gamesInfo} entries in the games info materialized view."

        match (Seq.tryHead gamesInfo) with
        | Some game -> printfn $"The first game has info:\n {game}"
        | None -> printfn "No game info found"
    }

program () |> Async.RunSynchronously
