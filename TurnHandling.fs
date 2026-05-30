module TurnHandling

open Decision
open EnemyProb
open IOHelper
open SpecialActions
open States

let rec private handleUserDecision (state: GameState): GameState =
    if state.User.MustSelfShoot then
        printfn "You must self-shoot this turn."
        pauseBriefly ()
        let result: DecisionResult =
            handleDecisionCommand User "SS" state
            |> Option.get

        printfn "%s" result.Message
        pauseBriefly ()
        result.State
    else
        printf "Choose decision (SS: Self-shoot / PS: Pass): "
        match handleDecisionCommand User (readCommand ()) state with
        | Some result ->
            printfn "%s" result.Message
            result.State
        | None ->
            printfn "Invalid decision. Try again."
            handleUserDecision state

let private printDecisionResult (result: DecisionResult): unit =
    printfn "%s" result.Message

let private printSpecialActionResult (result: SpecialActionResult): unit =
    printfn "%s" result.Message

    if result.ShowPeek then
        printChambers result.State.Chambers true result.State.CurrentIndex

let rec private handleUserSpecialAction (state: GameState): GameState =
    printf "Choose special action (SP: Spin / L: Lock / P: Peek / LD: Load): "
    match handleSpecialActionCommand User (readCommand ()) state with
    | Some result ->
        printSpecialActionResult result
        result.State
    | None ->
        printfn "Invalid special action. Try again."
        handleUserSpecialAction state

let rec private handleUserTurn (state: GameState): GameState =
    if state.User.Tickets > 0 then
        printf "Choose action (S: Special Action / D: Decision): "
        match readCommand () with
        | "D" -> handleUserDecision state
        | "S" ->
            let nextState: GameState = handleUserSpecialAction state
            handleUserTurn nextState
        | _ ->
            printfn "Invalid action. Try again."
            handleUserTurn state
    else
        printfn "No tickets. Choose Decision."
        handleUserDecision state

let private handleEnemyDecision (peekedBullet: bool option) (state: GameState): GameState =
    let command: string = chooseDecisionCommand peekedBullet state
    printfn "Nupjuk chooses %s." (if command = "SS" then "Self-shoot" else "Pass")
    pauseBriefly ()

    let result: DecisionResult =
        handleDecisionCommand Enemy command state
        |> Option.get

    printDecisionResult result
    pauseBriefly ()
    result.State

let private handleEnemySpecialAction (state: GameState): GameState * bool option =
    let command: string = chooseSpecialActionCommand state
    printfn "Nupjuk chooses special action %s." command
    pauseBriefly ()

    match handleSpecialActionCommand Enemy command state with
    | Some result ->
        printSpecialActionResult result
        pauseBriefly ()
        result.State, result.PeekedBullet
    | None -> failwith "Enemy selected an invalid special action."

let private handleEnemyTurn (state: GameState): GameState =
    let afterSpecialAction, peekedBullet =
        if shouldUseSpecialAction state then
            handleEnemySpecialAction state
        else
            state, None

    handleEnemyDecision peekedBullet afterSpecialAction

let handleTurn (state: GameState): GameState =
    match state.Turn with
    | User -> handleUserTurn state
    | Enemy -> handleEnemyTurn state
