module SpecialActions

open RandomProvider
open States

type SpecialActionResult = {
    State: GameState
    Message: string
    ShowPeek: bool
    PeekedBullet: bool option
}

let private actorName (player: Player): string =
    match player with
    | User -> "You"
    | Enemy -> "Nupjuk"

let private spendTicket (player: Player) (state: GameState): GameState =
    let playerState: PlayerState = getPlayerState player state
    setPlayerState player { playerState with Tickets = playerState.Tickets - 1 } state

let private noTicketMessage (player: Player): string =
    match player with
    | User -> "You need at least 1 ticket to perform a special action."
    | Enemy -> "Nupjuk needs at least 1 ticket to perform a special action."

let private spinRevolver (player: Player) (state: GameState): SpecialActionResult =
    let nextIndex: int = rng.Next state.Chambers.Length
    { State = { spendTicket player state with CurrentIndex = nextIndex }
      Message = sprintf "%s spin%s the revolver." (actorName player) (if player = User then "" else "s")
      ShowPeek = false
      PeekedBullet = None }

let private lockRevolver (player: Player) (state: GameState): SpecialActionResult =
    { State = { spendTicket player state with SpinLockTurns = 2 }
      Message = sprintf "%s lock%s the revolver. Spin is blocked for this turn and the next turn." (actorName player) (if player = User then "" else "s")
      ShowPeek = false
      PeekedBullet = None }

let private peekChamber (player: Player) (state: GameState): SpecialActionResult =
    { State = spendTicket player state
      Message = sprintf "%s peek%s at the current chamber." (actorName player) (if player = User then "" else "s")
      ShowPeek = (player = User)
      PeekedBullet = Some state.Chambers.[state.CurrentIndex] }

let private loadBullet (player: Player) (state: GameState): SpecialActionResult =
    let nextState: GameState = spendTicket player state
    let emptyIndexes: int array =
        nextState.Chambers
        |> Array.indexed
        |> Array.choose (fun (index: int, hasBullet: bool) ->
            if hasBullet then None else Some index)

    if Array.isEmpty emptyIndexes then
        { State = nextState
          Message = "No empty chamber is available."
          ShowPeek = false
          PeekedBullet = None }
    else
        let selectedIndex: int = emptyIndexes.[rng.Next emptyIndexes.Length]
        nextState.Chambers.[selectedIndex] <- true
        { State = nextState
          Message = sprintf "%s load%s a bullet into an unknown empty chamber." (actorName player) (if player = User then "" else "s")
          ShowPeek = false
          PeekedBullet = None }

let handleSpecialActionCommand (player: Player) (command: string) (state: GameState): SpecialActionResult option =
    if (getPlayerState player state).Tickets < 1 then
        Some
            { State = state
              Message = noTicketMessage player
              ShowPeek = false
              PeekedBullet = None }
    else
        match command with
        | "SP" when state.SpinLockTurns > 0 -> None
        | "SP" -> Some (spinRevolver player state)
        | "L" -> Some (lockRevolver player state)
        | "P" -> Some (peekChamber player state)
        | "LD" -> Some (loadBullet player state)
        | _ -> None
