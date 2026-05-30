# Nupjuk Roulette

## Overview

Nupjuk Roulette is a command-line Russian roulette game where the user plays against Nupjuk, an enemy player. The game uses a six-chamber revolver and includes four special actions: Spin, Lock, Peek, and Load.

This project is implemented in F# and requires the .NET 10 SDK.

## How to Run

First, clone this repository and move into the project directory:

```bash
git clone <repository-url>
cd <repository-folder>
```

Then run the project with the following command:

```bash
dotnet run
```

For example, if the repository URL is `https://github.com/seoujn/nupjuk-roulette.git`, you can run:

```bash
git clone https://github.com/seoujn/nupjuk-roulette.git
cd nupjuk-roulette
dotnet run
```

## How to Play

Each turn displays the current revolver state, both players' lives, both players' tickets, and whose turn it is. The user starts with 2 lives and 0 tickets, and Nupjuk also starts with 2 lives and 0 tickets. Tickets are capped at 3.

On the user's turn, if the user has at least 1 ticket, the user can choose `S` to use a Special Action or `D` to make a Decision. If the user has no tickets, the game moves directly to the Decision phase. 

When the game asks for an input, the user should enter the abbreviation shown below, such as `S`, `D`, `SP`, `SS`, or `PS`.

The special actions are:

* `SP`: Spin the revolver and randomly change the current chamber.
* `L`: Lock the revolver and block Spin for the current turn and the next turn.
* `P`: Peek at the current chamber. When the user peeks, the chamber is shown in the terminal.
* `LD`: Load one bullet into a random empty chamber.

The decisions are:

* `SS`: Self-shoot. If the current chamber contains a bullet, the shooter loses 1 life. After choosing Self-shoot, the shooter gains 1 ticket, up to the maximum of 3.
* `PS`: Pass. The player skips shooting for now, but must choose Self-shoot as their next Decision.

The game ends when either the user or Nupjuk reaches 0 lives.

## Notes for Playing

For the best experience, run the game in a terminal window with enough width to display the revolver state clearly.

## Implementation Notes

Invalid inputs are retried until the player enters a valid input. Once a valid action category or decision is chosen, it cannot be undone.

After a Self-shoot decision, the current chamber advances, as in an actual revolver. If a bullet is fired, that chamber becomes empty.

Nupjuk chooses its actions using a probability model based on the current bullet risk.

## Requirement Changes

The original proposal stated that a bullet would be added at the end of every turn whenever an empty chamber exists. In this implementation, automatic bullet addition happens only at the end of the first two turns.

This change was made because adding a bullet after every turn fills the six-chamber revolver too quickly and makes later choices almost deterministic. Limiting automatic bullet addition to the opening turns keeps the early pressure while preserving playable decision-making.

The implementation also clarifies that after a Self-shoot decision, the current chamber advances, as in an actual revolver. This behavior was not explicitly stated in the original requirements, so it is documented here to make the final behavior clear.

## LLM Usage

I used Codex to draft the overall code structure, discuss implementation details, analyze possible design choices, and organize the README format. I did not use the generated code without review. Instead, I manually inspected the implementation, tested the behavior, and revised the parts that did not match my intended game rules.

The main issue was that Codex sometimes produced code that compiled and ran, but did not fully match my interpretation of the requirements. For example, after a Pass, Codex initially implemented the next user turn as if the user had to immediately Self-shoot and could not use Special Actions. My intended rule was that the user may still use Special Actions on that turn, but once the user enters the Decision phase, the Decision must be Self-shoot. After I identified this mismatch, I revised the design with Codex and updated the implementation accordingly. Similar interpretation mistakes appeared multiple times even after corrections, so I had to repeatedly review the behavior manually and refine the prompts or code.

I also used Codex to help identify structural issues, such as where to separate decision handling, special actions, enemy probability logic, and console I/O. The final implementation was manually reviewed and adjusted to match the intended gameplay and project requirements.
