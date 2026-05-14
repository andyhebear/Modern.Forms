# Modern.Forms

> **此框架目前处于早期开发阶段，使用风险自负。**

`Modern.Forms` 是一个开源的、跨平台的 WinForms 精神继任者，面向 .NET 8，支持 Windows、macOS 和 Linux。

如果你在寻找一个开源的、跨平台的 WPF 精神继任者，请参阅 [Avalonia](https://github.com/AvaloniaUI/Avalonia)。

## 动机

本项目的目标是创建一个 WinForms 的精神继任者，具备以下特点：

- **跨平台** — 支持 Windows / macOS / Linux
- **WinForms 开发者友好** — 不使用 XAML，保持熟悉的编程模型
- **适合 LOB 应用和快速开发** — 简洁的代码后置模式
- **现代化控件和美学** — 内置 Light/Dark 主题，渲染与逻辑分离

```csharp
// 一个简单的 Modern.Forms 应用
using Modern.Forms;

public class MainForm : Form
{
    public MainForm ()
    {
        var button = new Button { Text = "点击我", Left = 10, Top = 10 };
        button.Click += (s, e) => MessageBox.Show ("Hello!", "提示");
        Controls.Add (button);
    }
}

class Program
{
    static void Main (string [] args) => Application.Run (new MainForm ());
}
```

## 特性

- **丰富的控件集** — Button、CheckBox、RadioButton、Label、TextBox、ComboBox、ListBox、ListView、TreeView、DataGridView、TabControl、SplitContainer、Ribbon、NavigationPane 等 30+ 种控件
- **布局系统** — Dock/Anchor 布局、FlowLayoutPanel（流式布局）、TableLayoutPanel（表格布局）
- **主题系统** — 内置 Light 和 Dark 主题，支持运行时动态切换和完全自定义
- **渲染器架构** — 控件逻辑与渲染分离，每个控件类型拥有独立的 `Renderer<T>`，可自定义替换
- **对话框支持** — OpenFileDialog、SaveFileDialog、FolderBrowserDialog、MessageBox、ContextMenu
- **跨平台渲染** — 基于 SkiaSharp 的 GPU 加速 2D 渲染，不依赖原生控件
- **平台抽象** — 通过 Modern.WindowKit 抽象窗口系统，核心库不直接依赖平台 API

## 控件一览

| 类别 | 控件 |
|------|------|
| 基础控件 | Button, CheckBox, RadioButton, Label, LinkLabel, TextBox, ComboBox |
| 列表控件 | ListBox, ListView, TreeView, DataGridView |
| 容器控件 | Panel, FlowLayoutPanel, TableLayoutPanel, SplitContainer, TabControl, TabStrip |
| 高级控件 | Ribbon, NavigationPane, ToolBar, StatusBar, ProgressBar, TrackBar, ScrollBar |
| 显示控件 | PictureBox, ImageList |
| 对话框 | OpenFileDialog, SaveFileDialog, FolderBrowserDialog, MessageBox |
| 其他 | Timer, PopupWindow, ContextMenu, Menu, MenuItem |

## 快速开始

### 使用项目模板（推荐）

```bash
dotnet new --install ModernForms.Templates
dotnet new modernforms
dotnet run
```

### 从零开始

**1. 创建项目文件**

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

**2. 创建窗体**

```csharp
using Modern.Forms;

public class MainForm : Form
{
}
```

**3. 启动应用**

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

## 示例应用

| 示例 | 说明 | 路径 |
|------|------|------|
| **ControlGallery** | 所有控件的展示画廊 | [`samples/ControlGallery`](samples/ControlGallery) |
| **Explorer** | Windows 资源管理器克隆 | [`samples/Explorer`](samples/Explorer) |
| **Outlaw** | Microsoft Outlook 克隆，展示复杂应用开发能力 | [`samples/Outlaw`](samples/Outlaw) |
| **DockLayoutTest** | Dock 布局功能测试 | [`samples/DockLayoutTest`](samples/DockLayoutTest) |

### 运行示例

```bash
# 克隆仓库
git clone https://github.com/modern-forms/Modern.Forms.git
cd Modern.Forms

# 运行 ControlGallery 示例
cd samples/ControlGallery
dotnet run

# 或运行 Explorer 示例
cd samples/Explorer
dotnet run
```

## 构建

```bash
# 构建解决方案
dotnet build --configuration Release

# 运行测试
dotnet test --configuration Release

# 打包 NuGet
dotnet pack src/Modern.Forms/Modern.Forms.csproj --configuration Release

# 代码格式化检查
dotnet format --verify-no-changes --verbosity diagnostic
```

## 构建状态

[![.NET Build](https://github.com/modern-forms/Modern.Forms/actions/workflows/dotnet.yml/badge.svg)](https://github.com/modern-forms/Modern.Forms/actions/workflows/dotnet.yml)

CI 在 macOS、Windows、Ubuntu 三个平台上构建，并在 Windows 上运行测试和 NuGet 打包。

## 技术栈

| 技术 | 用途 |
|------|------|
| C# / .NET 8 | 开发语言与运行时 |
| SkiaSharp | 跨平台 GPU 加速 2D 图形渲染 |
| HarfBuzzSharp | 高质量文本整形引擎 |
| Topten.RichTextKit | 富文本渲染 |
| Modern.WindowKit | 跨平台窗口工具包（源自 Avalonia） |
| xUnit v3 | 单元测试框架 |

## 架构

```
Modern.Forms (核心库)
├── Control 基类          # 所有控件的基类，实现布局/事件/状态管理
├── Renderers/            # 渲染器，每个控件对应独立渲染器（基于 SkiaSharp）
├── Layout/               # 布局引擎（Dock/Anchor、Flow、Table）
├── Theme                 # 主题系统，集中管理颜色/字体等视觉属性
├── Extensions/           # 扩展方法（Skia、绘图、数学等）
└── Modern.WindowKit      # 平台抽象层（窗口创建/输入处理）
```

**核心设计原则：**

- **渲染与逻辑分离** — 控件类负责状态/事件/行为，渲染器类负责绘制，通过 `RenderManager` 桥接
- **WinForms API 兼容** — 命名空间、类名、属性名、事件模型尽量与 `System.Windows.Forms` 保持一致
- **主题可定制** — 所有视觉属性通过 `Theme` 静态类集中管理，运行时可修改并立即生效

## 第三方代码来源

本项目包含以下 MIT 许可的开源代码：

- [Avalonia](https://github.com/AvaloniaUI/Avalonia) — 位于 `src/Modern.Forms/Avalonia` 目录
- [Mono Winforms](https://github.com/mono/mono/tree/master/mcs/class/System.Windows.Forms) — 分布于代码库中，各文件保留原始许可头
- [Microsoft Winforms](https://github.com/dotnet/winforms) — 分布于代码库中，各文件保留原始许可头

详细信息请参阅 [third-party-licenses.md](third-party-licenses.md)。

## 许可证

[MIT License](license.md) - Copyright (c) Jonathan Pobst
