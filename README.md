# AI Traffic Signal Controller

An intelligent adaptive traffic signal control system built in **Unity 3D** utilizing **Tabular Q-Learning (Reinforcement Learning)** to dynamically optimize traffic flow and minimize vehicle waiting times compared to traditional fixed-time alternating controllers.

---

## Overview

Urban intersections often suffer from congestion due to rigid, pre-timed signal phases that do not adapt to fluctuating traffic densities. This project simulates an autonomous 4-way intersection where incoming vehicles are sensed in real time. An embedded reinforcement learning agent dynamically decides signal transitions based on detected queue lengths on North-South versus East-West approaches.

### Key Capabilities
- **Procedural 3D Environment**: Self-contained runtime construction of asphalt roads, lane markings, pedestrian crosswalks, surrounding buildings, directional sunlight, and 4-way illuminated signal posts without external asset bloat.
- **Dynamic Vehicle Flow**: Realistic continuous vehicle movement, automated spawning, inter-vehicle spacing, deceleration curves, and stop-line detection.
- **Tabular Q-Learning Agent**: An online tabular reinforcement learning model trained over thousands of simulated state-action episodes using the Bellman optimality equation.
- **Live Comparison HUD**: Interactive on-screen display comparing metrics (throughput, total wait time, signal transitions) between **Q-Learning AI** mode and **Fixed Alternation** mode.

---

## Reinforcement Learning Architecture

The intersection is framed as a Markov Decision Process (MDP):

### 1. State Space ($S$)
Queues are observed along North-South ($NS$) and East-West ($EW$) directions within an approach detection zone ($7 < \text{distance} < 22$).
Queue lengths are bucketed into 3 discrete congestion levels:
- **0 (Low)**: 0 – 1 vehicles
- **1 (Medium)**: 2 – 4 vehicles
- **2 (High)**: $\ge 5$ vehicles

State tuple: $s = (\text{bucket}_{NS}, \text{bucket}_{EW}) \in \{0, 1, 2\} \times \{0, 1, 2\}$ (9 unique states).

### 2. Action Space ($A$)
- **Action 0**: Grant **Green** to North-South (East-West turns Red).
- **Action 1**: Grant **Green** to East-West (North-South turns Red).

### 3. Reward Function ($R$)
At each decision step, the reward penalizes congestion and excessive signal toggling:
$$R(s, a) = - (NS_{\text{queue}} + EW_{\text{queue}}) - (0.3 \text{ if signal switched else } 0)$$

### 4. Q-Table Update Rule
The agent trains via the standard tabular Q-learning update:
$$Q(s, a) \leftarrow Q(s, a) + \alpha \left[ R + \gamma \max_{a'} Q(s', a') - Q(s, a) \right]$$
- **Learning Rate ($\alpha$)**: $0.18$
- **Discount Factor ($\gamma$)**: $0.85$
- **Exploration ($\epsilon$)**: $0.18$
- **Training Episodes**: $4,000$ episodes simulated pre-runtime.

---

## Project Structure

```
├── Assets/
│   ├── InputSystem_Actions.inputactions
│   ├── Scenes/
│   │   └── SampleScene.unity
│   ├── Settings/              # Universal Render Pipeline (URP) assets & profiles
│   ├── TrafficProject/
│   │   ├── README.txt         # Course syllabus links and demonstration summary
│   │   └── TrafficGame.cs     # Main controller: simulation, Q-learning, & GUI
│   └── TutorialInfo/
├── Packages/
│   ├── manifest.json          # Unity package dependencies
│   └── packages-lock.json
├── ProjectSettings/           # Unity project & engine configuration
└── README.md
```

---

## Getting Started

### Prerequisites
- **Unity 6** or **Unity 2022.3+ LTS** with the **Universal Render Pipeline (URP)** installed.

### Setup & Execution
1. Clone the repository:
   ```bash
   git clone https://github.com/hydra187/Ai-traffic-signal-.git
   ```
2. Open **Unity Hub** and select **Add project from disk**.
3. Choose the cloned repository folder.
4. Open the project and navigate to `Assets/Scenes/SampleScene.unity`.
5. Press **Play** in the Unity Editor:
   - The scene, roads, buildings, and signal posts generate immediately at runtime.
   - 4,000 simulated training iterations execute before cars begin spawning.
   - Use the on-screen buttons to switch between **Q-learning AI** and **Fixed alternating** modes or pause the simulation.

---

## Controls & GUI

| Control Button | Function |
| :--- | :--- |
| **Use fixed timing / Use Q-learning** | Toggles between AI-driven signal selection and standard timer alternation. Resets performance stats for fair comparison. |
| **Pause / Resume** | Freezes/resumes vehicle physics and simulation timers. |

The GUI dashboard displays real-time performance indicators:
- **Cars passed**: Total number of vehicles that traversed the intersection.
- **Waiting**: Cumulative waiting time across all queued vehicles (in seconds).
- **Signal changes**: Count of phase switches initiated by the controller.

---

## Author
- **Aridaman Singh** ([@hydra187](https://github.com/hydra187))
