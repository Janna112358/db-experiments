open Database

printfn "Hello!"
getInfo () |> Async.RunSynchronously |> Seq.tryHead |> printfn "Output: %A"
