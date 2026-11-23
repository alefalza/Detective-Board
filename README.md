# Crónica de una Masacre

A narrative detective game made in Unity, inspired by Rodolfo Walsh's *Operación Masacre*.

**▶ Play it on itch.io:** [ITCH_IO_LINK](ITCH_IO_LINK)

![Gameplay](docs/gameplay.gif)

## About

*Buenos Aires, June 1956.* You are a journalist covering small crimes nobody reads about. Every newspaper repeats the official story: two subversives killed in a shootout with the police. Case closed. Then an anonymous envelope arrives with a single line: *"That's not how it happened."*

Inspect the scene, pin the evidence on your corkboard, connect it with thread, confirm your deductions with a phone call to your source, and write the real story on a typewriter before someone comes looking for you.

## Gameplay

- **Scenery inspection:** explore the scene, find inspectable objects and unlock new evidence.
- **Corkboard deductions:** drag evidence cards onto the board and connect them with thread. Each truth rests on three pieces of evidence that have to be linked together.
- **Phone confirmation:** call your source to validate each deduction and unlock keywords.
- **Typewriter article:** build the final article from the keywords you uncovered and choose its tone (neutral, protest or sensationalist), which changes the ending.

## Technical highlights

- **Data-driven design:** evidence, deductions, inspectables, articles and tones are ScriptableObjects. Timings and tuning values live in dedicated `*Settings` assets, so content and balance can be adjusted without touching code.
- **Deduction system:** `DeductionResolver` scores each group of connected evidence against every `DeductionRule`, and returns full, partial or invalid results with hints for partial matches.
- **Systems split by feature:** Corkboard, Deductions, Scenery, Phone, Typewriter, Narrative, HUD, Core. Each one is self-contained and communicates through a small set of managers.
- **View management:** `GameViewManager` switches between the Corkboard, Scenery, Phone and Typewriter views, with lighting and vignette transitions.
- **Polish:** typewriter text effects with randomized key sounds, interactive TextMeshPro keywords, dynamic 2D lighting, a contextual cursor and hint indicators for pending actions.

## Project structure

```
Assets/Scripts/
├── Core/          Singleton, view and lighting managers, cursor, audio settings, drag & drop
├── Corkboard/     Evidence items, spawning, overlap resolution, threads
├── Deductions/    Rules, resolver, results, phone bridge
├── Scenery/       Inspectables, inspection window, clue sequence, progress
├── Phone/         Phone view and call sequence
├── Typewriter/    Article building, keywords, validation
├── Narrative/     Intro, prologue and ending screens
├── HUD/           View buttons and pending-action hints
├── Menu/          Title screen and audio options
├── Text/          Typewriter text effects and sounds
├── Ambience/      Animated lights and floating elements
└── Debug/         Debug panel and tools
```

## Tech

- Unity 6 (6000.3), URP 2D
- C#, TextMeshPro, Input System
- iTween

## Credits

- **Idea & development:** Alejandro Falzarano
- **Art & script collaboration:** Natasha Laitano

Made for *Diseño Lúdico I*, part of the Bachelor's in Video Game Development and Production at Universidad de la Ciudad de Buenos Aires.

Fonts: Caveat, Courier Prime, Cutive Mono, Just Another Hand and Special Elite (SIL Open Font License).
