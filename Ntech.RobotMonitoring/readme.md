# Robot Animation - WPF to MAUI Conversion

## Overview
This is a converted version of the WPF Robot Animation application to .NET MAUI, making it cross-platform compatible.

## Key Changes from WPF to MAUI

### 1. **UI Framework Differences**
- **WPF Storyboards** → **MAUI Animations**: Replaced WPF's `Storyboard` animations with MAUI's built-in animation methods (`RotateTo`, `TranslateTo`)
- **Thickness/Margin Animations** → **Translation Animations**: Used `TranslateTo` instead of animating margins
- **Window** → **ContentPage**: Changed from `Window` to `ContentPage` as the root element

### 2. **Layout Changes**
- **AbsoluteLayout** for the control panel to position buttons in a circular arrangement
- **Grid** remains the primary layout container
- **Border** elements with `StrokeShape="RoundRectangle"` for rounded corners

### 3. **Visual Elements**
- **Ellipse** instead of `Rectangle` with RadiusX/RadiusY for circles
- **BoxView** for simple rectangular shapes (robot arm)
- **RadialGradientBrush** supported in MAUI for wafer and robot body

### 4. **Animation Implementation**
```csharp
// WPF Style
var storyboard = FindResource("PickUpWafer") as Storyboard;
storyboard.Begin();

// MAUI Style
await RobotArm.TranslateTo(40, 0, 500, Easing.CubicOut);
await RobotArea.RotateTo(targetRotation, 1000, Easing.CubicInOut);
```

### 5. **Data Binding**
- Maintained `INotifyPropertyChanged` implementation
- `BindingContext` set in constructor
- Properties bind the same way

### 6. **Missing WPF Features**
The following WPF features were adapted:
- **Window dragging**: Removed (not applicable to mobile/cross-platform)
- **Transparent window with no chrome**: Replaced with standard MAUI page
- **Complex keyframe animations**: Simplified to linear/easing animations

## Project Structure

```
RobotAnimation/
├── MainPage.xaml          # Main UI
├── MainPage.xaml.cs       # Code-behind with animation logic
├── App.xaml               # Application resources
├── App.xaml.cs            # Application entry point
├── RobotAnimation.csproj  # Project file
└── Resources/
    ├── Styles/
    │   ├── Colors.xaml
    │   └── Styles.xaml
    ├── Fonts/
    ├── Images/
    └── Splash/
```

## Running the Application

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 17.8+ or Visual Studio Code with MAUI extensions
- Platform-specific workloads:
  - Windows: Windows 10/11 SDK
  - macOS: Xcode
  - Android: Android SDK
  - iOS: Xcode and Mac build host

### Build & Run
```bash
# Windows
dotnet build -t:Run -f net8.0-windows10.0.19041.0

# Android
dotnet build -t:Run -f net8.0-android

# iOS (requires Mac)
dotnet build -t:Run -f net8.0-ios

# macOS
dotnet build -t:Run -f net8.0-maccatalyst
```

## Features
- **Rotate Robot**: Click the 0°, 90°, 180°, or 270° buttons to rotate the robot to face different modules
- **Pick Up Wafer**: Click ▲ to pick up a wafer from the current module
- **Put Down Wafer**: Click ▼ to place the wafer in the current module
- **Smooth Animations**: All movements are animated with easing functions

## Technical Notes

### Animation Timing
- **Rotation**: 1000ms with CubicInOut easing
- **Arm Extension/Retraction**: 500ms with CubicOut/CubicIn easing
- Animations are sequential using `await` for proper timing

### State Management
- `currentModule`: Tracks which PM module the robot is facing
- `AllowGetWafer`: Enabled when current module contains wafer
- `AllowPutWafer`: Enabled when robot is holding wafer
- `isAnimating`: Prevents animation conflicts

### Color Scheme
- Primary: `#00796B` (Teal)
- Secondary: `#FF9800` (Orange)
- Accent: `#E91E63` (Pink)
- Background: `#1A1A1A` (Dark)

## Future Enhancements
- Add more complex arm animations matching WPF version
- Implement proper wafer grabbing visual feedback
- Add settings for animation speed
- Support for multiple wafers
- Save/load robot positions