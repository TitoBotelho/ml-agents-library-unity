# Brick - Environment & Model Documentation

## Overview
2D grid environment (default 8x8) inspired by Unity ML-Agents "Basic", extended to allow movement on X and Z and two reward targets (small and large) placed on the grid. The agent has 5 discrete actions: stay, left, right, forward (+Z), back (-Z). Observations are a tri one-hot encoding of agent and goal positions.

## Main Components
- `BrickController.cs`: Manages integer grid state (positionX, positionZ), goal positions (smallX/smallZ, largeX/largeZ), rewards, and episode termination.
- `BrickActuatorComponent.cs` / `BrickActuator`: 1 discrete branch with 5 actions (0=stay, 1=left, 2=right, 3=forward, 4=back).
- `BrickSensorComponent.cs` / `BrickSensor`: Concatenates three one-hot maps (agent, big, small) → vector of size `gridSizeX * gridSizeY * 3` (e.g., 8*8*3 = 192).
- `brick_config.yaml`: PPO training configuration used for this environment.

## Observations & Actions
- Observation: Vector, 3 one-hot channels flattened. Order: [agent | big | small].
- Default size: 192 for 8x8 grid. If you change `gridSizeX`/`gridSizeY`, the observation size changes → retrain required.
- Grid mapping: world (x,z) → grid via `origin` (default 0,0) and `cellSize` (default 1). Keep goals and agent aligned to the grid.
- Actions: Discrete, 1 branch, size 5.

## Rewards
- Per step: -0.01 (time penalty)
- Small goal reached (positionX==smallX && positionZ==smallZ): +0.1 and ends episode
- Large goal reached (positionX==largeX && positionZ==largeZ): +1.0 and ends episode

## Behavior Parameters (Inspector)
- Behavior Name: `BrickAgent` (must match YAML)
- Behavior Type: Default (training) / Heuristic Only (keyboard) / Inference Only (run model)
- Model: None during training; drag the ONNX after training
- Vector Observation: auto from sensor (don’t set manually)
- Actions: Discrete → Branches: 1, Branch Sizes: 5
- Use Child Sensors: enabled (sensor on same GO or child)

## Training (PPO)
Summary of `brick_config.yaml`:
- Algorithm: PPO, hidden_units: 256, num_layers: 2
- hyperparameters: batch_size: 1024, buffer_size: 32768, learning_rate: 3e-4, beta: 5e-4, epsilon: 0.2, lambd: 0.95, num_epoch: 3
- time_horizon: 128, max_steps: 500k, summary_freq: 10k

Commands (Linux):
- Activate env: `conda activate mlagents30`
- Train new run: `mlagents-learn brick_config.yaml --run-id=brick8x8_goals2d_v1 --time-scale=20 --no-graphics`
- Resume existing: `mlagents-learn brick_config.yaml --run-id=brick8x8_goals2d_v1 --resume --time-scale=20 --no-graphics`
- Start Unity Play when prompted.

TensorBoard:
- `tensorboard --logdir results/brick8x8_goals2d_v1 --port 6006`

## Using Trained Models
- Final model is saved under `results/<run-id>/BrickAgent/BrickAgent.onnx`.
- Copy to `Assets/Examples/Brick/TFModels/` and assign in Behavior Parameters.
- Switch Behavior Type to Inference Only.

## Build
- Add scene `Assets/Examples/Brick/Scenes/Brick.unity` to Build Settings.
- Set Behavior Type = Inference Only and ensure a model is assigned.
- Build to desired target (e.g., WebGL or Standalone).

## Versions / Dependencies
- Unity: 2022.3 LTS
- ML-Agents (Unity package): 2.0.1
- Barracuda: 3.0.0
- Python: mlagents 0.30.0 (Communicator API 1.5.0)

## Notes / Best Practices
- Keep train/inference observation shapes identical.
- Don’t add extra sensors unless you intend to retrain.
- If changing grid size or stacking, retrain the model.


---
## Open Source Usage
May be used by anyone for any purpose of interest.
For questions or contact: tito_faria@hormail.com


_Last updated: 2025-10-14_
