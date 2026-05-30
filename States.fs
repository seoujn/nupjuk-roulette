module States

type Player = User | Enemy

type PlayerState = {
    Life: int
    Tickets: int
    MustSelfShoot: bool
}

type GameState = {
    Chambers: bool array // true = bullet, false = empty
    CurrentIndex: int
    User: PlayerState
    Enemy: PlayerState
    Turn: Player
    SpinLockTurns: int
    AutoLoadsRemaining: int
}

let getPlayerState (player: Player) (state: GameState): PlayerState =
    match player with
    | User -> state.User
    | Enemy -> state.Enemy

let setPlayerState (player: Player) (playerState: PlayerState) (state: GameState): GameState =
    match player with
    | User -> { state with User = playerState }
    | Enemy -> { state with Enemy = playerState }
