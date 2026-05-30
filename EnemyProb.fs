module EnemyProb

open RandomProvider
open States

let private bulletRisk (state: GameState): float =
    let bulletCount: int =
        state.Chambers
        |> Array.filter id
        |> Array.length

    float bulletCount / float state.Chambers.Length

let private chance (probability: float): bool =
    rng.NextDouble() < probability

let private chooseWeighted (items: (string * int) array): string =
    let totalWeight: int = items |> Array.sumBy snd
    let roll: int = rng.Next totalWeight

    let rec pick (index: int) (acc: int): string =
        let command, weight = items.[index]
        let nextAcc: int = acc + weight

        if roll < nextAcc then command
        else pick (index + 1) nextAcc

    pick 0 0

let shouldUseSpecialAction (state: GameState): bool =
    if state.Enemy.Tickets < 1 then
        false
    else
        let risk: float = bulletRisk state

        if risk <= 0.33 then chance 0.2
        elif risk <= 0.66 then chance 0.5
        else chance 0.8

let chooseDecisionCommand (peekedBullet: bool option) (state: GameState): string =
    if state.Enemy.MustSelfShoot then
        "SS"
    else
        match peekedBullet with
        | Some true -> "PS"
        | Some false -> "SS"
        | None ->
            let risk: float = bulletRisk state
            let selfShootProbability: float = 1.0 - risk

            if chance selfShootProbability then "SS" else "PS"

let chooseSpecialActionCommand (state: GameState): string =
    let risk: float = bulletRisk state
    let baseWeights: (string * int) array =
        if risk <= 0.33 then
            [| "SP", 20; "L", 30; "P", 20; "LD", 30 |]
        elif risk <= 0.66 then
            [| "SP", 30; "L", 20; "P", 40; "LD", 10 |]
        else
            [| "SP", 40; "L", 5; "P", 50; "LD", 5 |]

    let weights: (string * int) array =
        if state.SpinLockTurns > 0 then
            baseWeights |> Array.filter (fun (command, _) -> command <> "SP")
        else
            baseWeights

    chooseWeighted weights
