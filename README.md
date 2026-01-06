# Ntech.RobotMonitoring

This repository contains the source code for the Robot Monitoring application, featuring both the original WPF implementation and the converted .NET MAUI version for cross-platform compatibility.

## Overview

The Robot Animation application simulates a robot arm handling wafers between modules. It demonstrates UI animation techniques in .NET, showcasing how to migrate and adapt complex WPF animations to the cross-platform .NET MAUI framework.

## Projects

The solution consists of two main projects:

*   **Ntech.WPF.RobotMonitoring**: The original Windows Presentation Foundation (WPF) application.
*   **Ntech.MAUI.RobotMonitoring**: The modern .NET Multi-platform App UI (MAUI) version.

## Key Features

*   **Interactive Simulation**:
    *   **Rotate Robot**: Controls to rotate the robot (0°, 90°, 180°, 270°) to face different processing modules.
    *   **Wafer Handling**: Pick up (▲) and put down (▼) wafers.
*   **Visuals**:
    *   Smooth animations for rotation and arm extension.
    *   State tracking (wafer presence, robot position).
    *   Status indicators for processing modules.

## Getting Started

### Prerequisites

*   **.NET 8.0 SDK**
*   **Visual Studio 2022** (17.8 or later) with the following workloads:
    *   .NET Desktop Development (for WPF)
    *   .NET Multi-platform App UI development (for MAUI)

### Building and Running

1.  Clone the repository.
2.  Open `Ntech.RobotMonitoring/Ntech.RobotMonitoring.sln` in Visual Studio.

#### Running the MAUI App

Select `Ntech.MAUI.RobotMonitoring` as the startup project. You can run it on various platforms:

*   **Windows**: Select `Windows Machine`.
*   **Android**: Select an Android Emulator or connected device.
*   **iOS/macOS**: Requires a connected Mac build host.

Or via command line:

```bash
# Windows
dotnet build -t:Run -f net8.0-windows10.0.19041.0 -p:Ntech.MAUI.RobotMonitoring

# Android
dotnet build -t:Run -f net8.0-android -p:Ntech.MAUI.RobotMonitoring
```

#### Running the WPF App

Select `Ntech.WPF.RobotMonitoring` as the startup project and run it on Windows.

## Project Structure

*   `Ntech.RobotMonitoring/`
    *   `Ntech.MAUI.RobotMonitoring/`: MAUI source code (MainPage, App, Resources).
    *   `Ntech.WPF.RobotMonitoring/`: WPF source code (MainWindow, App).
    *   `readme.md`: Detailed documentation on the WPF to MAUI conversion process.