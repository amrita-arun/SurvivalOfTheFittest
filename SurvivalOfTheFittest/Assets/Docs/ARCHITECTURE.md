# Architecture — Gem-Based Encounter Odds

## Single randomized system

Only **`DiceRoller`** may call `UnityEngine.Random.Range(1, 7)`.

Everything else is deterministic:

- Fixed maze (`MazeData`)
- Fixed gem / encounter / exit positions
- Gem confidence tiers
- Safe vs risky action modifiers
- Fog, Clarity reveal, timers, UI

This is **output randomness**: the player chooses an action first; the fair d6 then resolves success.

## Gem tiers (lifetime total, not Clarity meter)

| Lifetime gems | Tier | Safe success | Risky success |
|---------------|------|--------------|---------------|
| 0–2 | Unprepared | 5–6 (~33%) | 6 (~17%) |
| 3–5 | Confident | 4–6 (50%) | 5–6 (~33%) |
| 6+ | Thriving | 3–6 (~67%) | 4–6 (50%) |

Risky actions use `safeRequired + 1`, clamped to **2–6**.

Activating Clarity resets the **3-segment meter only**. Lifetime gems (and odds) are unchanged.

## Key types

- `GemOddsCalculator` — pure odds math (unit tested)
- `RevealMeter` — lifetime gems + Clarity segments + score
- `DiceRoller` — fair d6 + unscaled animation + debug override
- `EncounterManager` / `EncounterUI` — choice → one roll → ack
- `GameManager` — centralized state (Tutorial / Playing / Encounter / Clarity / Win / Loss)
- `GameBootstrap` — runtime scene construction + restart rebuild

## Data flow (encounter)

1. Player touches `EncounterTrigger`
2. `GameManager` → `Encounter` state (timer & movement paused)
3. UI shows gem total, tier, exact success ranges for Safe/Risky
4. Player picks an action → `DiceRoller.Roll` (single call)
5. Compare roll ≥ required from `GemOddsCalculator`
6. Apply rewards/penalties; bonus gems go through `RevealMeter.RegisterGem`
7. Player acknowledges → encounter resolved; gameplay resumes
