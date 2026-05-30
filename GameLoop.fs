module GameLoop

open IOHelper
open RandomProvider
open States
open TurnHandling

let private switchTurn (state: GameState): GameState = 
    match state.Turn with
    | User ->
        { state with
            Turn = Enemy
            SpinLockTurns = max 0 (state.SpinLockTurns - 1) }
    | Enemy ->
        { state with
            Turn = User
            SpinLockTurns = max 0 (state.SpinLockTurns - 1) }

let private addBulletIfPossible (state: GameState): GameState =
    if state.AutoLoadsRemaining <= 0 then
        state
    else
        let nextState: GameState =
            { state with AutoLoadsRemaining = state.AutoLoadsRemaining - 1 }

        let emptyIndexes: int array =
            nextState.Chambers
            |> Array.indexed
            |> Array.choose (fun (index: int, hasBullet: bool) ->
                if hasBullet then None else Some index)

        if Array.isEmpty emptyIndexes then
            nextState
        else
            let selectedIndex: int = emptyIndexes.[rng.Next emptyIndexes.Length]
            nextState.Chambers.[selectedIndex] <- true
            printfn "A bullet has been added to an empty chamber."
            nextState

let rec gameLoop (state: GameState): unit =
    pauseBriefly ()
    displayTurnHeader state.Turn
    displayState state

    if state.User.Life <= 0 then
        displayWon Enemy
    elif state.Enemy.Life <= 0 then
        displayWon User
    else
        state
        |> handleTurn
        |> addBulletIfPossible
        |> switchTurn
        |> gameLoop
