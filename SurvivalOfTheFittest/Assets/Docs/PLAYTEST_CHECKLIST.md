# Playtest Checklist

## Movement & maze

- [ ] WASD moves the player
- [ ] Arrow keys move the player
- [ ] Walls block movement
- [ ] Diagonal movement is not faster than cardinal movement
- [ ] Maze layout is identical after pressing R

## Fog & Clarity

- [ ] Maze starts mostly dark
- [ ] Soft circular visibility follows the player
- [ ] Leaving an area returns it to darkness (no permanent uncover)
- [ ] Collecting 3 gems shows **CLARITY ACTIVATED!** and reveals the maze ~2s
- [ ] Clarity meter resets; lifetime gem count does **not**

## Gems & odds

- [ ] Gems increase score / lifetime total
- [ ] HUD shows Confidence tier (Unprepared / Confident / Thriving)
- [ ] 0–2 gems: safe needs 5–6
- [ ] 3–5 gems: safe needs 4–6
- [ ] 6+ gems: safe needs 3–6
- [ ] Risky choices need one higher than safe
- [ ] Encounter UI odds match the actual threshold

## Encounters & dice

- [ ] Touching an encounter pauses movement and timer
- [ ] Two actions show success ranges and % before choosing
- [ ] Choosing an action rolls once (no double-roll)
- [ ] Die animates then shows 1–6
- [ ] Result text: “You rolled X. You needed Y or higher…”
- [ ] Success / failure effects apply correctly
- [ ] Resolved encounter cannot trigger again
- [ ] Bonus gems update lifetime total + Clarity meter + odds UI

## Win / loss / meta

- [ ] Hearts never display below zero
- [ ] Timer does not run during tutorial or encounters
- [ ] Reaching EXIT opens win screen
- [ ] 0 hearts opens loss screen
- [ ] Timer 0 opens loss screen
- [ ] R fully resets maze, encounters, gems, timer, hearts, Clarity
- [ ] Q / Esc quits (stops Play Mode in Editor)
- [ ] Console has no red errors
- [ ] No `UnityEngine.Random` outside `DiceRoller`
