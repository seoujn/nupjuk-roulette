module IOHelper

open System
open System.Threading
open States

let readCommand (): string =
    match Console.ReadLine() with
    | null -> ""
    | input -> input.Trim().ToUpperInvariant()

let printChambers (chambers: bool array) (peeked: bool) (idx: int): unit = 
    let c (i: int): string =
        if i = idx && peeked then
            if chambers.[i] then ">[B]" else ">[_]"
        elif i = idx then
            ">[?]"
        else
            " [?]"

    printfn "      %s   %s" (c 0) (c 1)
    printfn "   %s         %s" (c 5) (c 2)
    printfn "      %s   %s" (c 4) (c 3)

let displayState (state: GameState): unit = 
    printfn ""
    printChambers state.Chambers false state.CurrentIndex
    printfn "User  - Life: %d, Tickets: %d" state.User.Life state.User.Tickets
    printfn "Enemy - Life: %d, Tickets: %d" state.Enemy.Life state.Enemy.Tickets
    printfn "Turn: %A" state.Turn
    if state.SpinLockTurns > 0 then printfn "Spin Locked."

let displayWon (player: Player): unit = 
    match player with
    | User -> printfn "You survived Nupjuk Roulette. You win!"
    | Enemy -> printfn "Nupjuk wins. The odds were not in your favor."

let displayTurnHeader (player: Player): unit =
    match player with
    | User -> printfn "\n--- USER TURN ---"
    | Enemy -> printfn "\n--- NUPJUK TURN ---"

let pauseBriefly (): unit =
    Thread.Sleep 500
