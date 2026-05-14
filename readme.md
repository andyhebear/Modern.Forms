# Modern.Forms

> **此框架目前处于早期开发阶段，使用风险自负。**

`Modern.Forms` 是一个开源的、跨平台的 WinForms 精神继任者，面向 .NET 8，支持 Windows、macOS 和 Linux。
`Modern.Forms` 是一个AI布局友好支持的跨平台界面库。
如果你在寻找一个开源的、跨平台的 WPF 精神继任者，请参阅 [Avalonia](https://github.com/AvaloniaUI/Avalonia)。



## 1. 项目概述

### 1.1 Modern.Forms 是什么

Modern.Forms 是一个开源的跨平台 GUI 框架，是 Windows Forms (WinForms) 的精神继承者。它面向 .NET 8.0，支持 Windows、macOS 和 Linux 三个平台。

**核心特点：**

- **跨平台**：基于 SkiaSharp 自绘渲染，不依赖原生控件，在所有平台上呈现一致的外观
- **WinForms 风格 API**：保留 WinForms 开发者熟悉的编程模式（Dock/Anchor 布局、事件驱动、控件命名），无需学习 XAML
- **适用于 AI 代码生成或其他按自然顺序构建 UI 的场景**：AI布局友好支持，使用AI生成界面，设置Form.UseForwardDockOrder=true（正序 Dock 布局标志）
- **现代美学**：内置现代化控件外观，支持 Light/Dark 主题
- **渲染器分离**：控件逻辑与视觉呈现完全解耦，可自定义渲染器
- **LOB 友好**：适合业务线应用和快速开发

### 1.2 设计动机与目标

| 目标 | 说明 |
|------|------|
| 跨平台 | Windows / Mac / Linux 一套代码运行 |
| 熟悉感 | WinForms 开发者无需学习曲线即可上手 |
| 现代 UI | 更新的控件和现代美学设计 |
| 快速开发 | 适合 LOB 应用和快速原型 |

如果你在寻找 WPF 的跨平台精神继承者，请参考 [Avalonia](https://github.com/AvaloniaUI/Avalonia)。

### 1.3 与 WinForms 的关系和差异

**相同点：**
- 相似的控件命名（Button、Label、TextBox 等）
- Dock/Anchor 布局系统
- 事件驱动模型
- `Application.Run()` 启动模式
- `SuspendLayout()/ResumeLayout()` 布局事务

**关键差异：**

| 特性 | WinForms | Modern.Forms |
|------|----------|-------------|
| 渲染方式 | 原生控件 + GDI+ | SkiaSharp 自绘 |
| 跨平台 | 仅 Windows | Windows/Mac/Linux |
| 窗口抽象 | Win32 HWND | Modern.WindowKit（Avalonia 衍生） |
| 样式系统 | Flat 属性 | 样式继承链 + 主题系统 |
| 渲染器 | 不可替换 | Renderer 模式，可替换 |
| 数据绑定 | 完整支持 | 尚未实现 |
| 设计器 | Visual Studio 集成 | 尚未实现 |
| 目标框架 | .NET Framework / .NET | .NET 8.0+ |

### 1.4 技术栈

```
Modern.Forms
├── .NET 8.0                    # 运行时
├── Modern.WindowKit 0.4.0      # 跨平台窗口抽象层（Avalonia 衍生）
├── SkiaSharp                   # 2D 图形渲染引擎
├── HarfBuzzSharp 8.3.1         # Unicode 文字塑形引擎
└── Topten.RichTextKit 0.4.167  # 富文本渲染
```

---

## 2. 快速入门

### 2.1 环境要求

- .NET 8.0 SDK 或更高版本
- 支持的操作系统：Windows / macOS / Linux

### 2.2 从模板创建项目

最简单的方式是使用 NuGet 上的 dotnet 模板：

```bash
# 安装模板
dotnet new --install ModernForms.Templates

# 创建项目
dotnet new modernforms

# 运行
dotnet run
```

### 2.3 从零创建项目

**1. 创建控制台项目**

```bash
dotnet new console -n MyModernApp
cd MyModernApp
```

**2. 修改项目文件 (MyModernApp.csproj)**

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
</PropertyGroup>

<ItemGroup>
    <PackageReference Include="Modern.Forms" Version="0.3.0" />
</ItemGroup>
```

**3. 创建窗体类 (MainForm.cs)**

```csharp
using Modern.Forms;

public class MainForm : Form
{
    public MainForm()
    {
        Text = "我的第一个 Modern.Forms 应用";

        var button = Controls.Add(new Button
        {
            Text = "点击我",
            Location = new System.Drawing.Point(10, 10),
            Size = new System.Drawing.Size(120, 35)
        });

        button.Click += (s, e) =>
        {
            MessageBox.Show("你好，Modern.Forms！", "提示");
        };
    }
}
```

**4. 修改入口 (Program.cs)**

```csharp
using Modern.Forms;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.Run(new MainForm());
    }
}
```

**5. 运行**

```bash
dotnet run
```

### 2.4 基本概念

Modern.Forms 的核心编程模型与 WinForms 高度一致：

- **Form**：顶层窗口，所有控件的容器
- **Control**：所有控件的基类
- **Controls.Add()**：向容器添加子控件
- **Dock/Anchor**：布局方式
- **事件**：Click、TextChanged、SelectedIndexChanged 等

---

## 3. 架构概览

### 3.1 分层架构

```
┌─────────────────────────────────────────┐
│            Application 层               │  应用生命周期管理
├─────────────────────────────────────────┤
│            Window 层                    │  WindowBase → Form / PopupWindow
├─────────────────────────────────────────┤
│            Control 层                   │  Control → 各种具体控件
├─────────────────────────────────────────┤
│          Layout Engine 层               │  DefaultLayout / FlowLayout / TableLayout
├─────────────────────────────────────────┤
│          Renderer 层                    │  Renderer<T> → 各控件渲染器
├─────────────────────────────────────────┤
│          Theme/Style 层                 │  Theme + ControlStyle + BorderStyle
├─────────────────────────────────────────┤
│        Platform Abstraction 层          │  Modern.WindowKit (Avalonia 衍生)
├─────────────────────────────────────────┤
│          Rendering 层                   │  SkiaSharp + HarfBuzzSharp + RichTextKit
└─────────────────────────────────────────┘
```

### 3.2 关键设计模式

#### 3.2.1 渲染器分离模式 (Renderer Pattern)

控件的视觉呈现与逻辑完全分离。每个控件类型有对应的 `Renderer<T>` 实现，通过 `RenderManager` 静态注册和查找。

```csharp
// 控件在 OnPaint 中委托给渲染器
protected override void OnPaint(PaintEventArgs e)
{
    base.OnPaint(e);
    RenderManager.Render(this, e);
}

// 可以替换渲染器实现自定义外观
RenderManager.SetRenderer<Button>(new MyCustomButtonRenderer());
```

#### 3.2.2 样式继承链模式 (Style Inheritance Chain)

`ControlStyle` 采用父子链式继承。每个控件有 `Style` 和 `StyleHover` 两个实例，通过 `_parent` 引用静态的 `DefaultStyle`。计算属性时沿链向上查找。

```csharp
// 样式继承链
实例 Style → DefaultStyle → Theme 默认值

// 计算属性回退
public SKColor GetBackgroundColor() => BackgroundColor ?? _parent?.GetBackgroundColor() ?? Theme.ControlMidColor;
```

#### 3.2.3 布局引擎模式 (Layout Engine Pattern)

从 WinForms 移植，每个控件通过 `LayoutEngine` 属性返回其布局引擎。

```csharp
// 默认 Dock+Anchor 布局
public virtual LayoutEngine LayoutEngine => DefaultLayout.Instance;

// FlowLayoutPanel 使用 FlowLayout
public override LayoutEngine LayoutEngine => FlowLayout.Instance;

// TableLayoutPanel 使用 TableLayout
public override LayoutEngine LayoutEngine => TableLayout.Instance;
```

#### 3.2.4 PropertyStore 优化模式

使用 `PropertyStore` 替代大量实例字段，减少每个控件实例的内存开销。不常用的属性按需存储在 PropertyStore 中。

```csharp
// PropertyStore 使用静态 key 访问
private static readonly int s_cursorProperty = PropertyStore.CreateKey();

public Cursor? Cursor {
    get => (Cursor?)Properties.GetObject(s_cursorProperty);
    set => Properties.SetObject(s_cursorProperty, value);
}
```

#### 3.2.5 事件属性模式 (Event Properties)

使用 `EventHandlerList` 配合静态 key 对象来存储事件委托，避免为每个事件分配字段。

```csharp
private static readonly object s_clickEvent = new object();

public event EventHandler<MouseEventArgs>? Click {
    add => Events.AddHandler(s_clickEvent, value);
    remove => Events.RemoveHandler(s_clickEvent, value);
}

protected virtual void OnClick(MouseEventArgs e) {
    (Events[s_clickEvent] as EventHandler<MouseEventArgs>)?.Invoke(this, e);
}
```

#### 3.2.6 ControlAdapter 适配器模式

`ControlAdapter` 是 `ScrollableControl` 的内部子类，作为 Window 和 Control 树之间的桥梁。它管理焦点和绘制坐标转换。

#### 3.2.7 ControlBehaviors 标志模式

使用 `[Flags]` 枚举声明控件的行为特征，替代传统 WinForms 的 `ControlStyles`：

```csharp
[Flags]
public enum ControlBehaviors
{
    Selectable = 0x01,
    Hoverable = 0x02,
    Transparent = 0x04,
    ReceivesMouseEvents = 0x08,
    // ...
}
```

### 3.3 类继承层次

```
Component
  ├── WindowBase (abstract)
  │     ├── Form
  │     └── PopupWindow
  └── Control (partial)
        ├── ScrollableControl
        │     ├── Panel
        │     │     ├── FlowLayoutPanel
        │     │     └── TableLayoutPanel
        │     └── ControlAdapter (internal sealed)
        ├── Button
        ├── Label / LinkLabel
        ├── TextBox
        ├── CheckBox / RadioButton
        ├── ComboBox
        ├── ProgressBar / TrackBar
        ├── PictureBox
        ├── ListBox / ListView / TreeView / DataGridView
        ├── Menu / MenuDropDown
        ├── ToolBar / Ribbon / StatusBar
        ├── TabControl / TabStrip / SplitContainer / NavigationPane
        ├── ScrollBar (HorizontalScrollBar / VerticalScrollBar)
        ├── FormTitleBar / SizeGrip / Splitter
        └── ...
```

---

## 4. 核心基类详解

### 4.1 Component 基类

`Component` 是所有控件和非可视化组件的基类，提供：
- `Events` (EventHandlerList)：事件委托存储
- `Properties` (PropertyStore)：属性值存储
- `Site`：设计时站点
- `Dispose()`：资源释放
- `Disposed` 事件

### 4.2 Control 控件基类

`Control` 是所有可视化控件的基类，实现 `ILayoutable`、`IArrangedElement`、`IDisposable` 接口。

**核心属性：**

| 属性 | 类型 | 说明 |
|------|------|------|
| `Text` | `string` | 控件文本 |
| `Location` | `Point` | 控件位置（相对于父容器） |
| `Size` | `Size` | 控件大小 |
| `Width` / `Height` | `int` | 宽/高 |
| `Left` / `Top` / `Right` / `Bottom` | `int` | 边界坐标 |
| `Bounds` | `Rectangle` | 完整边界 |
| `ClientRectangle` | `Rectangle` | 客户区（减去边框） |
| `ClientSize` | `Size` | 客户区大小 |
| `Visible` | `bool` | 是否可见 |
| `Enabled` | `bool` | 是否启用 |
| `Dock` | `DockStyle` | 停靠方式 |
| `Anchor` | `AnchorStyles` | 锚定方式 |
| `Margin` | `Padding` | 外边距 |
| `Padding` | `Padding` | 内边距 |
| `TabIndex` | `int` | Tab 顺序 |
| `TabStop` | `bool` | 是否参与 Tab 导航 |
| `Cursor` | `Cursor?` | 鼠标光标 |
| `ContextMenu` | `ContextMenu?` | 上下文菜单 |
| `Controls` | `ControlCollection` | 子控件集合 |
| `Parent` | `Control?` | 父控件 |
| `Style` | `ControlStyle` | 常态样式 |
| `StyleHover` | `ControlStyle` | 悬停样式 |
| `CurrentStyle` | `ControlStyle` | 当前样式（根据悬停状态自动切换） |
| `Name` | `string` | 控件名称 |
| `Tag` | `object?` | 用户自定义数据 |

**核心方法：**

| 方法 | 说明 |
|------|------|
| `Controls.Add(control)` | 添加子控件 |
| `Controls.Remove(control)` | 移除子控件 |
| `BringToFront()` | 移到 Z 序最前 |
| `SendToBack()` | 移到 Z 序最后 |
| `Invalidate()` | 请求重绘 |
| `Show()` | 显示控件 |
| `Hide()` | 隐藏控件 |
| `Focus()` | 获取焦点 |
| `Contains(control)` | 检查是否包含子控件 |
| `GetPreferredSize(proposedSize)` | 获取期望大小 |
| `SetBounds(x, y, w, h)` | 设置边界 |
| `SuspendLayout()` | 挂起布局 |
| `ResumeLayout()` | 恢复布局 |
| `PerformLayout()` | 立即执行布局 |
| `OnPaint(e)` | 绘制（可重写） |
| `Dispose()` | 释放资源 |

**控件生命周期：**

```
构造 → 添加到父控件 → CreateControl() → OnCreateControl()
  → 可见/交互 → Invalidate() → OnPaint()
  → 属性变更 → 布局更新
  → Dispose() → 释放
```

### 4.3 ScrollableControl 可滚动控件

继承自 `Control`，添加滚动支持：

- `AutoScroll`：是否自动显示滚动条
- `AutoScrollPosition`：自动滚动位置
- `HorizontalScroll` / `VerticalScroll`：滚动属性

### 4.4 WindowBase / Form / PopupWindow 窗体体系

**WindowBase** 是所有窗口的抽象基类：
- 通过 `IWindowBaseImpl` 抽象平台差异
- 处理原始输入事件（鼠标、键盘）
- 使用 SkiaSharp Framebuffer 进行绘制
- 管理 DPI 缩放

**Form** 是顶层窗口：
- 自绘标题栏 (`FormTitleBar`)
- 自绘边框（可拖拽调整大小）
- `UseSystemDecorations` 切换系统/自绘装饰
- `UseForwardDockOrder` 控制 Dock 布局顺序（默认 true，正序；设 false 兼容 WinForms 传统倒序）
- `ShowDialog()` 模态对话框
- `WindowState` (Normal/Minimized/Maximized)
- `MinimumSize` / `MaximumSize`
- `StartPosition` (CenterScreen/CenterParent)
- `Closing` 事件（可取消关闭）
- `Text` 设置标题栏文本

**PopupWindow** 是弹出窗口，用于 ComboBox 下拉等场景。

### 4.5 ControlAdapter 窗口-控件适配器

`ControlAdapter` 是 `ScrollableControl` 的内部子类，作为 Window 和 Control 树之间的桥梁：
- 管理 `SelectedControl`（焦点控件）
- 处理从窗口到控件的绘制坐标转换
- 路由输入事件到控件树
- `DisplayRectangle` 属性已排除 TitleBar 高度，确保 Dock 和 Anchor 布局从 TitleBar 下方开始

**TitleBar 与布局的关系：**

Form 不是 Control 的子类，而是通过 ControlAdapter 管理子控件。TitleBar 作为隐式控件添加到 Controls 集合中：

- 所有 Dock 和 Anchor 布局基于 DisplayRectangle 计算，因此子控件不会被 TitleBar 遮挡
- TitleBar 定位在 DisplayRectangle 之上（Y=0 到 Y=TitleBar.Height），宽度撑满容器
- 非 Dock.None 的控件从 Y=0 开始布局，在放置在窗口中的控件需要考虑Form.TitleBar的高端，防止被标题栏遮挡
- 主持传统winform的布局与AI友好布局

---

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
git clone https://github.com/andyhebear/Modern.Forms.git
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

[![.NET Build](https://github.com/andyhebear/Modern.Forms/actions/workflows/dotnet.yml/badge.svg)](https://github.com/andyhebear/Modern.Forms/actions/workflows/dotnet.yml)

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
