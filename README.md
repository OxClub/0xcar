# 0xcar 🏎️

A 2D Android car racing game built with Unity.

## Features
- 3-lane endless racing
- Touch controls (accelerate / brake / left / right)
- Dynamic difficulty scaling
- Local high score (PlayerPrefs)
- Score + speed HUD
- Game over + restart

## Build
1. Open the project in Unity 2022 LTS (2D Core)
2. File → Build Settings → Android → Switch Platform
3. Player Settings: Portrait, IL2CPP, ARM64, Min API 23
4. Build APK

## Controls
- **Accelerate** button → speed up
- **Brake** button → slow down
- **◀ / ▶** → change lane

## Scripts
| File | Purpose |
|------|---------|
| CarController.cs | Player car movement + lanes |
| TouchControls.cs | On-screen mobile buttons |
| RoadScroller.cs | Infinite road scrolling |
| EnemyCar.cs | Enemy car logic + collision |
| EnemySpawner.cs | Spawns enemies with difficulty curve |
| GameManager.cs | Score, state, high score |
| UIManager.cs | HUD + game over panel |

## License
MIT
