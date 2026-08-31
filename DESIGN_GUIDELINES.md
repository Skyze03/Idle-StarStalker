# Idle StarStalker — Confirmed Design Guidelines

## Development priority

Implement and verify gameplay functionality before investing heavily in final art,
animation, visual effects, or polish. Placeholder presentation is acceptable, but
the UI must remain readable, responsive, and capable of communicating every
important game-state change.

## Core combat identity

- Combat is automatic after it starts.
- Player skill is expressed through cultivation and pre-battle build decisions,
  not through timing inputs during battle.
- Ultimate, equipment, and any future talent system are selected before battle.
- The complete build is locked into a combat snapshot when battle starts.
- Battle cannot be exited and other game functions cannot be used until it ends.
- The game should expose objective mechanics and results without recommending a
  best build, predicting victory, or explaining how to counter an enemy.

## Edit Build and equipment navigation

- Equipment should use a dedicated paged inventory experience rather than a long
  list of cycle buttons.
- The character view should show the six body-related equipment slots, plus the
  optional Accessory slot used by the current prototype.
- Selecting a slot should open a grid of compatible owned items.
- Items should visually read as individual inventory cells.
- Equipment, Ultimate selection, and My Build should share one coherent
  `Edit Build` flow rather than feeling like unrelated screens.
- All equipped items act together, allowing pure and hybrid builds without
  hard-coded class restrictions.

## Battle presentation

- Replace separate player/enemy action bars with one shared action timeline.
- Each combatant should have a portrait, icon, or arrow marker showing its
  current position on that timeline.
- The shared timeline must make the effect of Agility immediately understandable:
  faster characters visibly lap slower characters and act more often.
- Player and enemy HP, Rage, Ultimate, traits, and recent actions must remain
  objectively readable.

## Rewards and action feedback

Every meaningful gain must have visible feedback. A number silently changing is
not sufficient.

- Battle completion should open a reward/result panel.
- First-clear rewards, normal rewards, equipment, and Ultimate unlocks should be
  clearly distinguished.
- Meditation and Collection should show immediate lightweight feedback, such as
  floating text or a short notification.
- Sweeps, crafting, upgrades, and future reward sources should use the same
  feedback language.
- Large reward moments may use a panel; frequent small gains should use concise,
  non-blocking feedback.

## Global UI rules

- Primary target is portrait mobile, including different phone and tablet ratios.
- Respect safe areas and prevent overlap, clipping, illegible scaling, and tiny
  touch targets.
- Contextual global controls such as Save and Load should not remain visible when
  they cannot be used, especially during battle.
- Prototype UI may be visually simple, but hierarchy, spacing, typography, and
  interaction feedback are part of functional correctness.

## Deferred presentation work

The following are intentionally deferred until the feature foundation is stable:

- Final character and enemy artwork
- Finished icons and item illustrations
- Attack, hit, and Ultimate animation
- Particle and screen effects
- Final sound and music
- Final visual theme and polish

Deferring these items must not prevent the project from establishing the correct
screen structure and interaction model now.
