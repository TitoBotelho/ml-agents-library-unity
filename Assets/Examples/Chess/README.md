# Chess (Basic) - Environment & Model Documentation

## Overview
Demonstration environment derived from the Unity ML-Agents "Basic" example, adapted into a layout with a custom chess piece (player) and two coins (reward targets) created by the project author. The agent moves along a 1D axis (left / stay / right) seeking two goals:
- Small goal (smallGoal): intermediate reward.
- Large goal (largeGoal): main reward.

## Main Components
- `ChessController.cs`: Manages position, rewards, and episode termination.
- `ChessActuatorComponent.cs` / `ChessActuator`: Converts discrete action into direction {-1, 0, +1}.
- `ChessSensorComponent.cs` / `ChessSensor`: One-hot observation of current position (vector length 20).
- `ModelSwitcher.cs`: Dynamically switches between ONNX models (≈200, 60k, 140k training steps) and a near-untrained state.

## Observations & Actions
- Observation space: One-hot vector (length 20) indicating current position.
- Action space: Discrete (3) -> 0 = stay, 1 = move -1, 2 = move +1.

## Rewards
- Per step: -0.01 (time penalty).
- Small goal reached: +0.1 and ends episode.
- Large goal reached: +1.0 and ends episode.

## Model Architecture (PPO)
Configuration used (summary of `chess_config.yaml`):
- Algorithm: PPO
- Hidden layers: 2 fully connected layers
- Units per layer: 128
- Batch size: 1024
- Buffer size: 10240
- Initial learning rate: 3e-4 (linear decay)
- gamma: 0.99
- time_horizon: 64
- max_steps (planned full training): 500,000

## Exported Models
Three training checkpoints chosen for comparison:
1. ≈200 steps (almost untrained baseline)
2. ≈60,000 steps
3. ≈140,000 steps (converged for the defined objective)

Place the ONNX files under `Assets/Examples/Chess/Models/` (create if needed) and assign them in the `ModelSwitcher` component.

## Using the ModelSwitcher
Inspector fields:
- `model200`, `model60k`, `model140k` (NNModel references)
- UI Buttons wired to: `UseHeuristic()` (200), `Use60k()`, `Use140k()`
Persistence: The last selected model is stored via `PlayerPrefs` and restored on restart.

## Button UI
Three buttons fixed to the bottom bar:
- Left: Initial model (≈200)
- Center: 60k model
- Right: 140k model
The first button starts with a pulsing highlight (optional) to indicate the initial active model.

## Visual / Art Assets
- Player piece and coin goal objects were created by the author of this project.

## Versions / Dependencies
- Unity: 2022.3 LTS
- Unity ML-Agents package: 2.0.1
- Barracuda: 3.0.0
- (Python tooling originally: mlagents 1.1.0; no longer required now that training is finished.)

## Applied Best Practices
- Avoided scene reload for environment reset (performance improvement).
- Clear separation of Sensor, Actuator, and Controller responsibilities.
- Runtime swapping of trained models for side-by-side behavioral comparison.

## Possible Future Extensions
- Steps-to-goal metric in UI
- Rolling average reward graph
- GPU inference (InferenceDevice = GPU) if beneficial
- Multiple simultaneous agents

## Licensing / Notice
This environment conceptually derives from the Unity ML-Agents "Basic" example, adapted and extended. Visual elements (player piece and coins) are original creations of the project author.

---
## Open Source Usage
May be used by anyone for any purpose of interest.
For questions or contact: tito_faria@hormail.com

_Last updated: (fill in date)_
