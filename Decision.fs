module Decision

open States

type DecisionResult = {
    State: GameState
    Message: string
}

let private clampTickets (tickets: int): int =
    min 3 tickets

let private selfShootMessage (player: Player) (hasBullet: bool): string =
    match player, hasBullet with
    | User, true -> "Bang! You lose 1 life."
    | User, false -> "Click. You are safe."
    | Enemy, true -> "Bang! Nupjuk loses 1 life."
    | Enemy, false -> "Click. Nupjuk is safe."

let private passMessage (player: Player): string =
    match player with
    | User -> "You passed. On your next turn, you must self-shoot."
    | Enemy -> "Nupjuk passes. On Nupjuk's next turn, it must self-shoot."

let private selfShoot (player: Player) (state: GameState): DecisionResult =
    let hasBullet: bool = state.Chambers.[state.CurrentIndex]
    let playerState: PlayerState = getPlayerState player state
    let nextLife: int =
        if hasBullet then playerState.Life - 1
        else playerState.Life

    if hasBullet then
        state.Chambers.[state.CurrentIndex] <- false

    let nextPlayerState: PlayerState =
        { playerState with
            Life = nextLife
            Tickets = clampTickets (playerState.Tickets + 1)
            MustSelfShoot = false }
    let nextIndex: int = (state.CurrentIndex + 1) % state.Chambers.Length

    { State = { setPlayerState player nextPlayerState state with CurrentIndex = nextIndex }
      Message = selfShootMessage player hasBullet }

let private pass (player: Player) (state: GameState): DecisionResult =
    let playerState: PlayerState = getPlayerState player state
    let nextPlayerState: PlayerState = { playerState with MustSelfShoot = true }

    { State = setPlayerState player nextPlayerState state
      Message = passMessage player }

let handleDecisionCommand (player: Player) (command: string) (state: GameState): DecisionResult option =
    match command with
    | "SS" -> Some (selfShoot player state)
    | "PS" -> Some (pass player state)
    | _ -> None
