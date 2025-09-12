# Mixed Reality Robot Simulation

This project is a **Mixed Reality (MR) robot simulation and defect detection system** developed with **Unity 6000.0.50f1 LTS** for the **Meta Quest 3** headset. It includes simulation of a UR10e robotic arm, user interface design, defect detection algorithms, gesture-based interactions, and automated testing.

## Target Platform
- **Meta Quest 3**

## Requirements & Dependencies
- **Unity 6000.0.50f1 LTS**
- **Android Build Support**
- **Meta XR All-in-One SDK**
- **Unity OpenXR Meta**
- **XR Interaction Toolkit**
- **XR Hands**
- **OpenXR Plugin**
- **Unity Input System**
- **Unity Test Framework**

## Installation
1. Install **Unity 6000.0.50f1 LTS** via Unity Hub.
2. Ensure **Android Build Support**, SDK/NDK, and OpenJDK are installed.
3. Clone the repository:
   ```bash
   git clone https://github.com/username/mr-robot-simulation.git
4. Open the project in Unity.
5. In **Build Settings**, set the platform to Android.
6. Build the project and deploy it to a Meta Quest 3 device.

## Features
- **XR and Meta Integration**
    - XR Interaction Toolkit and Meta OpenXR support
    - Controller and hand tracking integration


- **Robot Simulation and Defect Detection System**
    - Surface scanning and defect marking
    - Forward and Inverse Kinematics for realistic animation
    - Task definitions with start/pause/stop functionality
    - "Re-check" and "Confirm" functionality for quality control
    - Tooltip system (triggered when looking a defect and doing poke hand gesture)


- **User Interface**
    - Separate panels for Operator and Administrator roles
    - Task list, progress tracking, and log viewer
    - UI interaction through hand gestures (grab, poke, pinch)


- **Testing & Quality Assurance**
    - Automated unit, functional, and performance tests with Unity Test Framework
    - Logging system for tracking events and debugging


- **Build & Deployment**
    - Optimized Android APK build pipeline for Meta Quest 3
    - Vulkan (primary) and OpenGL ES3 (fallback) graphics support