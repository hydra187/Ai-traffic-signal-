AI Traffic Signal Controller - college demonstration

Open Assets/Scenes/SampleScene and press Play. The intersection is made from basic Unity 3D objects at runtime. Cars arrive from four directions. Red-signal cars wait; green-signal cars cross. The panel shows completed cars, total waiting seconds, and signal changes. Use the button to compare Q-learning with fixed alternation; changing modes resets the run.

Syllabus links:
Week 1: Unity scene, cubes, materials, directional light, 3D camera.
Week 2: C# variables, methods, loops, if statements, UI buttons, moving objects.
Week 3: tabular Q-learning (same idea as the Taxi exercise). The state is low/medium/high queue length on north-south and east-west roads. The two actions choose which road gets green. The reward penalizes remaining queues and unnecessary switching. 4,000 short simulated training episodes run when Play starts, then the learned Q table controls the visible intersection.

This is a classroom-scale Q-learning demonstration. It does not use ML-Agents PPO, Cardboard VR, Blender, or outside assets because those are unnecessary for the assigned traffic signal task.
