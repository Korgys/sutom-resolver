# Sutom Rules

Sutom is a word puzzle where you must find a hidden word in a limited number of tries.
It is based on the famous website https://sutom.nocle.fr/

## Goal

- Find the secret word.
- The word has a fixed length.
- Each guess must respect the known pattern, if one is given.

## How To Play

1. Enter a word guess.
2. The game returns feedback for each letter.
3. Use the feedback to improve your next guess.
4. Repeat until you find the word or run out of attempts.

## Feedback Meaning

- A letter shown as itself is in the correct place.
- `+` means the letter exists in the word but is in the wrong position.
- `_` or `?` means the letter is not in the word.

## Important Rules

- Keep letters in the same positions when they are confirmed.
- Use the `+` markers to move misplaced letters to new positions.
- Do not reuse letters that have already been ruled out.
- If the game gives a starting pattern, treat it as fixed information.

## Example

If the hidden word is `LION` and your guess is `LAMA`:

- `L` is correct if it appears in the first position.
- `A` and `M` are ruled out if they are not in the word.

The exact feedback depends on the hidden word and the letters already used.
