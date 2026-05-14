# Modern.Forms

> **This framework is currently in its early stages. Use at your own risk.**

`Modern.Forms` is an open-source cross-platform spiritual successor to WinForms for .NET 8, supporting Windows, macOS, and Linux.

If you are looking for an open-source cross-platform spiritual successor to WPF, see [Avalonia](https://github.com/AvaloniaUI/Avalonia).

## Motivation

The goal is to create a spiritual successor to WinForms that is:

- **Cross-platform** — Windows / macOS / Linux
- **Familiar for WinForms developers** — No XAML, code-behind model
- **Great for LOB applications and quick apps** — Simple and productive
- **Modern controls and aesthetics** — Built-in Light/Dark themes, renderer-based architecture

```csharp
// A simple Modern.Forms application
using Modern.Forms;

public class MainForm : Form
{
    public MainForm ()
    {
        var button = new Button { Text = "Click Me", Left = 10, Top = 10 };
        button.Click += (s, e) => MessageBox.Show ("Hello!", "Greeting");
        Controls.Add (button);
    }
}

class Program
{
    static void Main (string [] args) => Application.Run (new MainForm ());
}
```

## Features

- **Rich control set** — 30+ controls: Button, CheckBox, RadioButton, Label, TextBox, ComboBox, ListBox, ListView, TreeView, DataGridView, TabControl, SplitContainer, Ribbon, NavigationPane, and more
- **Layout system** — Dock/Anchor layout, FlowLayoutPanel, TableLayoutPanel
- **Theme system** — Built-in Light and Dark themes with runtime switching and full customization
- **Renderer architecture** — Logic/rendering separation; each control type has an independent `Renderer<T>` that can be replaced
- **Dialog support** — OpenFileDialog, SaveFileDialog, FolderBrowserDialog, MessageBox, ContextMenu
- **Cross-platform rendering** — GPU-accelerated 2D rendering via SkiaSharp, no native control dependencies
- **Platform abstraction** — Window system abstracted via Modern.WindowKit; core library has no direct platform API dependencies

## Controls

| Category | Controls |
|----------|----------|
| Basic | Button, CheckBox, RadioButton, Label, LinkLabel, TextBox, ComboBox |
| Lists | ListBox, ListView, TreeView, DataGridView |
| Containers | Panel, FlowLayoutPanel, TableLayoutPanel, SplitContainer, TabControl, TabStrip |
| Advanced | Ribbon, NavigationPane, ToolBar, StatusBar, ProgressBar, TrackBar, ScrollBar |
| Display | PictureBox, ImageList |
| Dialogs | OpenFileDialog, SaveFileDialog, FolderBrowserDialog, MessageBox |
| Other | Timer, PopupWindow, ContextMenu, Menu, MenuItem |

## Getting Started

### From Template (Recommended)

```bash
dotnet new --install ModernForms.Templates
dotnet new modernforms
dotnet run
```

### From Scratch

**1. Create the project file**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Modern.Forms" Version="0.3.0" />
  </ItemGroup>
</Project>
```

**2. Create a Form**

```csharp
using Modern.Forms;

public class MainForm : Form
{
}
```

**3. Run the application**

```csharp
using Modern.Forms;

class Program
{
    static void Main (string [] args)
    {
        Application.Run (new MainForm ());
    }
}
```

## Sample Applications

| Sample | Description | Path |
|--------|-------------|------|
| **ControlGallery** | Gallery of all available controls | [`samples/ControlGallery`](samples/ControlGallery) |
| **Explorer** | Windows Explorer clone | [`samples/Explorer`](samples/Explorer) |
| **Outlaw** | Microsoft Outlook clone, demonstrating complex app development | [`samples/Outlaw`](samples/Outlaw) |
| **DockLayoutTest** | Dock layout feature test | [`samples/DockLayoutTest`](samples/DockLayoutTest) |

### Running Samples

```bash
# Clone the repository
git clone https://github.com/modern-forms/Modern.Forms.git
cd Modern.Forms

# Run ControlGallery
cd samples/ControlGallery
dotnet run

# Or run Explorer
cd samples/Explorer
dotnet run
```

## Building

```bash
# Build the solution
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Pack NuGet
dotnet pack src/Modern.Forms/Modern.Forms.csproj --configuration Release

# Verify code formatting
dotnet format --verify-no-changes --verbosity diagnostic
```

## Build Status

[![.NET Build](https://github.com/modern-forms/Modern.Forms/actions/workflows/dotnet.yml/badge.svg)](https://github.com/modern-forms/Modern.Forms/actions/workflows/dotnet.yml)

CI builds on macOS, Windows, and Ubuntu. Tests and NuGet packaging run on Windows.

## Tech Stack

| Technology | Purpose |
|------------|---------|
| C# / .NET 8 | Language and runtime |
| SkiaSharp | Cross-platform GPU-accelerated 2D graphics rendering |
| HarfBuzzSharp | High-quality text shaping engine |
| Topten.RichTextKit | Rich text rendering |
| Modern.WindowKit | Cross-platform windowing toolkit (derived from Avalonia) |
| xUnit v3 | Unit testing framework |

## Architecture

```
Modern.Forms (Core Library)
├── Control base class     # Base for all controls: layout, events, state management
├── Renderers/             # Renderers: one per control type (SkiaSharp-based)
├── Layout/                # Layout engines (Dock/Anchor, Flow, Table)
├── Theme                  # Theme system: centralized color/font management
├── Extensions/            # Extension methods (Skia, drawing, math, etc.)
└── Modern.WindowKit       # Platform abstraction (window creation, input handling)
```

**Core design principles:**

- **Rendering/logic separation** — Control classes handle state/events/behavior; renderer classes handle drawing, bridged via `RenderManager`
- **WinForms API compatibility** — Namespaces, class names, properties, and event models align with `System.Windows.Forms`
- **Customizable themes** — All visual properties managed through the `Theme` static class, modifiable at runtime with immediate effect

## Third-Party Sources

This project includes code from the following MIT-licensed projects:

- [Avalonia](https://github.com/AvaloniaUI/Avalonia) — Found in `src/Modern.Forms/Avalonia`
- [Mono Winforms](https://github.com/mono/mono/tree/master/mcs/class/System.Windows.Forms) — Distributed throughout the codebase; individual files retain original license headers
- [Microsoft Winforms](https://github.com/dotnet/winforms) — Distributed throughout the codebase; individual files retain original license headers

See [third-party-licenses.md](third-party-licenses.md) for details.

## License

[MIT License](license.md) - Copyright (c) Jonathan Pobst
