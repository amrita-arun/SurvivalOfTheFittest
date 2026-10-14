# Survival of the Fittest — Setup

## Project facts

- Unity **6000.3** (Unity 6), **2D URP**
- **Input System** only (`activeInputHandler: 1`)
- No external art packs required — sprites/UI are code-generated
- The game **auto-bootstraps** when you press Play (even on the default `SampleScene`)

## Quick start (recommended)

1. Open the project in Unity 6.
2. Open `Assets/Scenes/SampleScene.unity` (or any scene).
3. Press **Play**.
4. Click **START DATING (THE MAZE)** on the tutorial panel.
5. Play with **WASD / Arrows**, **Shift/Space** to dash. **R** restarts. **Q / Esc** quits.
6. Flags sit on side paths. Pass all **3** dating gates to reach the EXIT. Timer starts at **90s**, and there is no full-maze reveal. Procedural SFX play automatically (optional clips can still be assigned on `GameAudio`).

Optional: menu **Survival of the Fittest → Setup Game Scene** adds an explicit `GameBootstrap` object and configures the camera. This is not required because `GameBootstrap` auto-creates at runtime.

## What the bootstrap creates

| Object | Role |
|--------|------|
| `GameBootstrap` | Builds / rebuilds the run |
| `Systems` | `GameManager`, `DiceRoller`, `EncounterManager`, `GameAudio`, `RevealMeter` |
| `World` | Maze tiles, player, gems, encounters, fog |
| `GameCanvas` | Tutorial, HUD, encounter panel, win/loss |
| `EventSystem` | UI clicks via `InputSystemUIInputModule` |

### Camera

- Orthographic, black clear color
- Positioned at maze center
- Size fits the full fixed maze

### Player

- Tag: `Player`
- `Rigidbody2D` (Dynamic, gravity 0, freeze rotation)
- `CircleCollider2D` (solid)
- `PlayerController` + cyan circle sprite

### Walls / Exit / Gems / Encounters

- Walls: solid `BoxCollider2D`
- Exit / Gems / Encounters: trigger colliders
- Maze layout is the fixed string grid in `MazeData.cs` (not random)

## Optional audio

Select the `Systems` object → `GameAudio` and assign clips to:

- Gem Collect, Encounter Open, Dice Roll, Success, Failure, Win, Loss

Leaving them empty is valid — the game never requires clips.

## Edit Mode tests

1. Open **Window → General → Test Runner**
2. Select **Edit Mode**
3. Run all `SurvivalOfTheFittest.Tests`

Covered: gem tiers, risky vs safe thresholds, UI string parity, flag totals, die fairness, Random usage audit, encounter gates.

## Changing the maze

Edit the string array in `Assets/Scripts/Maze/MazeData.cs`:

- `#` wall · `.` floor · `P` player · `X` exit · `G` gem
- `A` Ex-Situationship · `B` Dating App · `C` Mixed Signals

Do **not** generate this layout randomly.
