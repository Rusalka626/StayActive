# StayActive

A lightweight Windows tray app that keeps your PC from going idle by simulating small, harmless input at a set interval.

<img width="338" height="396" alt="Animation" src="https://github.com/user-attachments/assets/57dc5fd4-44b7-41b4-9cbf-6e7422a2cdcb" />

## Features

- **Lives in the system tray.** Left-click the icon to turn it on or off; right-click for the menu. The icon changes to show the current state.
- **Four activity modes:**
  - Move the mouse 1 pixel and back (default)
  - Left-click at the current cursor position
  - Press F15, a key that doesn't exist on regular keyboards and won't interfere with what you're doing
  - Press Ctrl + Space, for apps that only react to keyboard input
- **Configurable interval** from 5 to 120 seconds.
- **Schedule mode:** turns itself on and off within a time window (for example 09:00–18:00). Overnight ranges like 22:00–06:00 work too.
- **Start with Windows** option.
- **Remembers your settings** between sessions.

## Download

1. Go to [Releases](https://github.com/Rusalka626/StayActive/releases/latest) and download `StayActive.exe`.
2. Requires the [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (Windows 10 or later).
3. Run it. The icon appears in the system tray (check the hidden icons arrow if you don't see it).

## How it works

- **Input simulation:** calls the Win32 `SendInput` API through P/Invoke. The `INPUT` struct is a C union, so it's mapped with `StructLayout.Explicit` and `FieldOffset` so mouse and keyboard data share the same memory.
- **Keys sent as scan codes, with a real hold time:** some apps (games especially) read low-level input and ignore instant virtual-key events. Sending hardware scan codes with a 60 ms gap between key-down and key-up makes the press look like a physical one.
- **Tray icon:** WPF has no built-in tray icon, so the app uses [H.NotifyIcon.Wpf](https://github.com/HavenDV/H.NotifyIcon).
- **Timers:** a `DispatcherTimer` fires the selected activity; a second one checks the schedule once a minute.
- **Settings:** saved as JSON in `%AppData%\StayActive\settings.json`. "Start with Windows" adds an entry under `HKCU\...\CurrentVersion\Run`, so it needs no admin rights.

## Tech stack

C# · .NET 8 · WPF · Win32 interop (`user32.dll`) · H.NotifyIcon.Wpf · System.Text.Json

## Build from source

Requires the .NET 8 SDK on Windows.

```bash
git clone https://github.com/Rusalka626/StayActive.git
cd StayActive
dotnet build -c Release
```

To produce a single executable:

```bash
dotnet publish StayActive/StayActive.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

## Project structure

```
StayActive/
├── App.xaml(.cs)          # Tray icon, context menu, app lifecycle
├── MainWindow.xaml(.cs)   # Settings window (hides instead of closing)
├── Resources/             # Tray and window icons
└── Services/
    ├── ActivityService.cs # On/off state, interval timer, schedule logic
    ├── InputSimulator.cs  # SendInput P/Invoke: mouse and keyboard
    ├── SettingsService.cs # JSON load/save in %AppData%
    ├── StartupService.cs  # Start with Windows (registry Run key)
    └── AppSettings.cs     # Settings model
```

## Roadmap

- "Keep awake" mode using `SetThreadExecutionState`, which prevents sleep and screen lock without simulating any input
- English UI (the interface is currently in Spanish)
- Manual on/off that isn't overridden by the schedule
- Unit tests for the schedule logic

## License

MIT
