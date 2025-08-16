# DESIGN.MD


## Game Overview
A Balloon Popping Game where players tap on balloons to pop them, earn points, and avoid bombs. The game features various balloon types with different effects, a scoring system.
Players pop balloons before they escape, chaining pops and power-ups for higher scores.

---
**Target Platform:** iOS & Android (using .NET MAUI)

**Target Session Length:** 1-5 minutes

**Monetization:** Ads (banner, interstitial, rewarded video)

**Audience:** Casual players of all ages, satisfying quick gameplay sessions.

**Rating Target:** E for Everyone

---

## Player Goal and Core Loop
**Primary Goal:** Pop as many balloons as possible while managing hazards and timers. Build combos for high scores.

**Core Loop:**

1. **Spawn:** Wave of balloons with rules per mode.
2. **Action:** Player taps balloons to pop them.
3. **Evaluate:** award points, combo multiplier, time/health changes.
4. **Escalate:** Increase difficulty, spawn more balloons, add special balloons/hazards.
5. **Repeat:** Continue until time runs out or player loses all health.

**Failure:** Timer expires (Time Attack/Challenge) or health reaches 0 (Arcade/Endless).

---

## Controls & Input
- **Tap a balloon:** Pop it.
- **Haptic feedback** (mobile): light tap on pop, stronger on combo milestones (optional in settings).

---

## Game Modes

---

## Balloon Types & Rules

---

## Scoring, Combos & Multipliers

---

## Power-Ups

---

## Difficulty Progression

---

## UI/UX & Flow

---


