open RandomProvider
open States
open GameLoop

let initialPlayer =
    { Life = 2
      Tickets = 0
      MustSelfShoot = false }

let initialChambers: bool array =
    let chambers = Array.create 6 false
    chambers.[rng.Next chambers.Length] <- true
    chambers

let initialState =
    { Chambers = initialChambers
      CurrentIndex = 0
      User = initialPlayer
      Enemy = initialPlayer
      Turn = User
      SpinLockTurns = 0
      AutoLoadsRemaining = 2 }

gameLoop initialState
