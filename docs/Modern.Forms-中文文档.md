# Modern.Forms 中文技术文档

> 版本：0.3.0 | 目标框架：.NET 8.0 | 许可证：MIT

---

## 目录

1. [项目概述](#1-项目概述)
2. [快速入门](#2-快速入门)
3. [架构概览](#3-架构概览)
4. [核心基类详解](#4-核心基类详解)
5. [控件库参考](#5-控件库参考)
6. [布局系统](#6-布局系统)
7. [主题与样式系统](#7-主题与样式系统)
8. [渲染系统](#8-渲染系统)
9. [事件系统](#9-事件系统)
10. [平台抽象层](#10-平台抽象层)
11. [示例应用解析](#11-示例应用解析)

---

## 1. 项目概述

### 1.1 Modern.Forms 是什么

Modern.Forms 是一个开源的跨平台 GUI 框架，是 Windows Forms (WinForms) 的精神继承者。它面向 .NET 8.0，支持 Windows、macOS 和 Linux 三个平台。

**核心特点：**

- **跨平台**：基于 SkiaSharp 自绘渲染，不依赖原生控件，在所有平台上呈现一致的外观
- **WinForms 风格 API**：保留 WinForms 开发者熟悉的编程模式（Dock/Anchor 布局、事件驱动、控件命名），无需学习 XAML
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

Form 不是 Control 的子类，而是通过 ControlAdapter 管理子控件。TitleBar 作为隐式控件添加到 Controls 集合中，但不参与 Dock 布局分配空间：

- `ControlAdapter.DisplayRectangle` 返回的区域已排除 TitleBar 高度（约 40 像素）
- 所有 Dock 和 Anchor 布局基于 DisplayRectangle 计算，因此子控件不会被 TitleBar 遮挡
- TitleBar 定位在 DisplayRectangle 之上（Y=0 到 Y=TitleBar.Height），宽度撑满容器
- 非 Dock.None 的控件从 Y=TitleBar.Height 开始布局
- Dock.None 的控件也基于 DisplayRectangle 定位，不会与 TitleBar 重叠

---

## 5. 控件库参考

### 5.1 基础控件

#### Button

按钮控件，支持文本 + 图片 + 悬停样式。

```csharp
var button = new Button
{
    Text = "确定",
    Location = new Point(10, 10),
    Size = new Size(100, 35)
};
button.Click += (s, e) => { /* 处理点击 */ };
Controls.Add(button);
```

**关键属性：** `Text`, `Image`, `ImageAlign`, `TextAlign`, `TextImageRelation`

#### Label

文本标签控件。

```csharp
var label = new Label { Text = "用户名：" };
```

**关键属性：** `Text`, `TextAlign`

#### LinkLabel

超链接标签控件。

```csharp
var link = new LinkLabel { Text = "访问官网" };
link.LinkClicked += (s, e) => { /* 打开链接 */ };
```

**关键属性：** `Text`, `LinkArea`, `LinkBehavior`

#### TextBox

文本输入框，支持单行和多行模式。

```csharp
var textBox = new TextBox
{
    Location = new Point(80, 10),
    Size = new Size(200, 30),
    PlaceholderText = "请输入..."
};
```

**关键属性：** `Text`, `PlaceholderText`, `ReadOnly`, `MaxLength`, `SelectedText`, `SelectionStart`, `SelectionLength`

#### CheckBox

复选框控件。

```csharp
var checkBox = new CheckBox { Text = "记住密码" };
checkBox.CheckedChanged += (s, e) => { /* 处理变更 */ };
```

**关键属性：** `Text`, `Checked`, `CheckState`, `ThreeState`

#### RadioButton

单选按钮控件。同一容器内的 RadioButton 自动互斥。

```csharp
var radio1 = new RadioButton { Text = "选项A", Location = new Point(10, 10) };
var radio2 = new RadioButton { Text = "选项B", Location = new Point(10, 40) };
```

**关键属性：** `Text`, `Checked`

#### ComboBox

下拉组合框。

```csharp
var combo = new ComboBox();
combo.Items.AddRange("选项1", "选项2", "选项3");
combo.SelectedIndex = 0;
combo.SelectedIndexChanged += (s, e) => { /* 处理选择变更 */ };
```

**关键属性：** `Items`, `SelectedIndex`, `SelectedItem`, `DropDownHeight`

#### ProgressBar

进度条控件。

```csharp
var progress = new ProgressBar
{
    Minimum = 0,
    Maximum = 100,
    Value = 50
};
```

**关键属性：** `Minimum`, `Maximum`, `Value`

#### TrackBar

滑块控件。

```csharp
var trackBar = new TrackBar
{
    Minimum = 0,
    Maximum = 100,
    Value = 50,
    TickStyle = TickStyle.BottomRight
};
```

**关键属性：** `Minimum`, `Maximum`, `Value`, `TickStyle`, `Orientation`

#### PictureBox

图片框控件。

```csharp
var pictureBox = new PictureBox
{
    SizeMode = PictureBoxSizeMode.StretchImage,
    Image = SKBitmap.Decode("image.png")
};
```

**关键属性：** `Image`, `SizeMode`

### 5.2 列表/数据控件

#### ListBox

列表框控件。

```csharp
var listBox = new ListBox();
listBox.Items.Add("项目1");
listBox.Items.Add("项目2");
listBox.SelectedIndexChanged += (s, e) => { /* 处理选择变更 */ };
```

**关键属性：** `Items`, `SelectedIndex`, `SelectedItem`, `SelectionMode`, `Sorted`

#### ListView

列表视图控件，支持多种视图模式。

```csharp
var listView = new ListView { View = View.Details };
listView.Columns.Add("名称", 200);
listView.Columns.Add("大小", 100);
listView.Items.Add(new ListViewItem("文件1", "1KB"));
```

**关键属性：** `Items`, `Columns`, `View`, `SelectedItems`, `MultiSelect`

#### TreeView

树视图控件。

```csharp
var tree = new TreeView();
var root = tree.Items.Add("根节点");
root.Items.Add("子节点1");
root.Items.Add("子节点2");
tree.ItemSelected += (s, e) => { /* 处理选择 */ };
```

**关键属性：** `Items`, `SelectedItem`, `DrawMode`, `ShowLines`

#### DataGridView

数据网格视图控件。

```csharp
var grid = new DataGridView();
grid.Columns.Add(new DataGridViewColumn { HeaderText = "姓名" });
grid.Columns.Add(new DataGridViewColumn { HeaderText = "年龄" });
grid.Rows.Add(new DataGridViewRow { Cells = { ... } });
```

**关键属性：** `Columns`, `Rows`, `SelectionMode`, `AllowUserToAddRows`

### 5.3 容器/布局控件

#### Panel

面板容器，默认使用 Dock+Anchor 布局。

```csharp
var panel = new Panel { Dock = DockStyle.Fill };
panel.Controls.Add(new Button { Text = "面板内按钮" });
```

#### FlowLayoutPanel

流式布局面板，子控件按流方向排列。

```csharp
var flowPanel = new FlowLayoutPanel
{
    FlowDirection = FlowDirection.LeftToRight,
    WrapContents = true
};
flowPanel.Controls.Add(new Button { Text = "按钮1" });
flowPanel.Controls.Add(new Button { Text = "按钮2" });
```

**关键属性：** `FlowDirection`, `WrapContents`

#### TableLayoutPanel

表格布局面板，支持行列定义。

```csharp
var tablePanel = new TableLayoutPanel
{
    ColumnCount = 2,
    RowCount = 2
};
tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
tablePanel.Controls.Add(new Label { Text = "标签1" }, 0, 0);
tablePanel.Controls.Add(new TextBox(), 1, 0);
```

**关键属性：** `ColumnCount`, `RowCount`, `ColumnStyles`, `RowStyles`, `GrowStyle`

#### SplitContainer

分割容器，两个面板中间有可拖拽的分割条。

```csharp
var split = new SplitContainer
{
    Dock = DockStyle.Fill,
    Orientation = Orientation.Vertical,
    SplitterDistance = 200
};
split.Panel1.Controls.Add(new TreeView { Dock = DockStyle.Fill });
split.Panel2.Controls.Add(new ListView { Dock = DockStyle.Fill });
```

**关键属性：** `Panel1`, `Panel2`, `Orientation`, `SplitterDistance`, `SplitterWidth`

#### TabControl

选项卡控件。

```csharp
var tabControl = new TabControl { Dock = DockStyle.Fill };
tabControl.TabPages.Add("常规");
tabControl.TabPages.Add("高级");
tabControl.TabPages[0].Controls.Add(new Label { Text = "常规设置" });
```

**关键属性：** `TabPages`, `SelectedIndex`, `SelectedTab`

#### TabStrip

选项卡条控件，更轻量的选项卡。

```csharp
var tabStrip = new TabStrip();
tabStrip.Items.Add("标签1");
tabStrip.Items.Add("标签2");
```

### 5.4 菜单/工具栏控件

#### Menu

菜单栏控件，通常停靠在窗体顶部。

```csharp
var menu = new Menu { Dock = DockStyle.Top };
var fileItem = menu.Items.Add("文件");
fileItem.DropDown.Items.Add("新建", null, OnNewClick);
fileItem.DropDown.Items.Add("打开", null, OnOpenClick);
fileItem.DropDown.Items.AddSeparator();
fileItem.DropDown.Items.Add("退出", null, OnExitClick);
Controls.Add(menu);
```

#### ContextMenu

上下文菜单（右键菜单）。

```csharp
var contextMenu = new ContextMenu();
contextMenu.Items.Add("复制", null, OnCopyClick);
contextMenu.Items.Add("粘贴", null, OnPasteClick);
control.ContextMenu = contextMenu;
```

#### ToolBar

工具栏控件。

```csharp
var toolbar = new ToolBar { Dock = DockStyle.Top };
toolbar.Items.Add("新建", ImageLoader.Get("new.png"), OnNewClick);
toolbar.Items.Add("打开", ImageLoader.Get("open.png"), OnOpenClick);
Controls.Add(toolbar);
```

#### Ribbon

Ribbon 风格控件（类似 Office 2007+ 的功能区）。

```csharp
var ribbon = new Ribbon();
var homeTab = ribbon.TabPages.Add("主页");
var group = homeTab.Groups.Add("操作");
group.Items.Add("新建", image, OnNewClick);
group.Items.Add("删除", image, OnDeleteClick);
Controls.Add(ribbon);
```

#### StatusBar

状态栏控件，通常停靠在窗体底部。

```csharp
var statusBar = new StatusBar { Dock = DockStyle.Bottom };
statusBar.Items.Add("就绪");
Controls.Add(statusBar);
```

### 5.5 导航/装饰控件

#### NavigationPane

导航面板，类似 Outlook 的侧边栏。

```csharp
var navPane = new NavigationPane { Dock = DockStyle.Left };
navPane.Items.Add("邮件", mailImage);
navPane.Items.Add("日历", calendarImage);
```

#### FormTitleBar

窗体标题栏，由 Form 自动创建和管理。

#### ScrollBar (HorizontalScrollBar / VerticalScrollBar)

滚动条控件，通常由 ScrollableControl 自动管理。

### 5.6 对话框

#### MessageBox

消息框对话框。

```csharp
var result = MessageBox.Show("确认删除？", "确认", MessageBoxButtons.YesNo);
if (result == DialogResult.Yes) { /* 执行删除 */ }
```

#### FileDialog / OpenFileDialog / SaveFileDialog

文件对话框。

```csharp
var dialog = new OpenFileDialog
{
    Title = "选择文件",
    Filter = "文本文件|*.txt|所有文件|*.*"
};
if (dialog.ShowDialog() == DialogResult.OK)
{
    var filePath = dialog.FileName;
}
```

#### FolderBrowserDialog

文件夹浏览对话框。

```csharp
var dialog = new FolderBrowserDialog();
if (dialog.ShowDialog() == DialogResult.OK)
{
    var folderPath = dialog.SelectedPath;
}
```

### 5.7 辅助组件

#### ImageList

图像列表组件，用于统一管理图标。

```csharp
var imageList = new ImageList();
imageList.Images.Add("folder", SKBitmap.Decode("folder.png"));
imageList.Images.Add("file", SKBitmap.Decode("file.png"));
```

#### Timer

计时器组件。

```csharp
var timer = new Timer { Interval = 1000 };
timer.Tick += (s, e) => { /* 每秒执行 */ };
timer.Start();
```

---

## 6. 布局系统

Modern.Forms 的布局系统直接从 .NET WinForms 移植，保持了高度兼容性。

### 6.1 DefaultLayout（Dock + Anchor 布局）

默认布局引擎，所有 Panel 和 Control 默认使用。

**Dock 停靠布局：**

```csharp
// 控件停靠到父容器边缘
var menu = new Menu { Dock = DockStyle.Top };       // 停靠顶部
var status = new StatusBar { Dock = DockStyle.Bottom }; // 停靠底部
var tree = new TreeView { Dock = DockStyle.Left };  // 停靠左侧
var content = new ListView { Dock = DockStyle.Fill };   // 填充剩余空间
```

DockStyle 枚举值：`None`, `Top`, `Bottom`, `Left`, `Right`, `Fill`

**布局顺序：** 先 Dock 后 Anchor，最后 AutoSize。Dock 控件按 Z 序从内到外分配空间。

**Dock 布局顺序模式：**

Modern.Forms 支持两种 Dock 布局顺序，通过 `Form.UseForwardDockOrder` 属性控制：

| 模式 | UseForwardDockOrder | 布局行为 | 适用场景 |
|------|---------------------|---------|---------|
| **正序模式** | `true`（默认） | 先 `Controls.Add` 的控件靠边缘 | AI 代码生成、设计器 |
| **传统模式** | `false` | 后 `Controls.Add` 的控件靠边缘（WinForms 兼容） | 从 WinForms 迁移的旧代码 |

```csharp
// 正序模式（推荐，默认）
this.UseForwardDockOrder = true;
// 添加顺序 = 布局顺序：nav_tree 在最左，email_list 在中间，message 填充剩余
nav_tree_view = Controls.Add(new TreeView { Dock = DockStyle.Left, Width = 225 });
email_list = Controls.Add(new TreeView { Dock = DockStyle.Left, Width = 325 });
message_pane = Controls.Add(new Panel { Dock = DockStyle.Fill });

// 传统模式（WinForms 兼容）
this.UseForwardDockOrder = false;
// 布局顺序与添加顺序相反：最后添加的靠边缘
// 等效于 WinForms 的倒序 z-order 布局
```

**正序模式详细规则：**
- `DockStyle.Top`：先添加的在最上方
- `DockStyle.Bottom`：先添加的在最下方（靠近边缘）
- `DockStyle.Left`：先添加的在最左边
- `DockStyle.Right`：先添加的在最右边（靠近边缘）
- `DockStyle.Fill`：最后填充剩余区域

**传统模式详细规则：**
- 所有方向的 Dock 控件按倒序 z-order 处理
- 最后添加的同方向 Dock 控件最靠近边缘
- 与 WinForms 的默认行为完全一致

**Anchor 锚定布局：**

```csharp
// 控件锚定到父容器边缘，随父容器大小变化
var button = new Button
{
    Anchor = AnchorStyles.Top | AnchorStyles.Right,  // 锚定右上角
    Location = new Point(200, 10)
};
```

AnchorStyles 枚举值：`None`, `Top`, `Bottom`, `Left`, `Right`（可组合）

锚定机制：控件记住与锚定边缘的初始距离，当父容器大小变化时，保持距离不变。

### 6.2 FlowLayout（流式布局）

`FlowLayoutPanel` 使用的布局引擎。

```csharp
var flowPanel = new FlowLayoutPanel
{
    FlowDirection = FlowDirection.LeftToRight,
    WrapContents = true
};
```

**FlowDirection 枚举值：** `LeftToRight`, `TopDown`, `RightToLeft`, `BottomUp`

**FlowBreak 扩展属性：** 可强制某个子控件换行/换列。

### 6.3 TableLayout（表格布局）

`TableLayoutPanel` 使用的布局引擎。

```csharp
var table = new TableLayoutPanel
{
    ColumnCount = 3,
    RowCount = 2,
    Dock = DockStyle.Fill
};

table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));   // 固定宽度
table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));     // 百分比
table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));        // 自动大小

table.Controls.Add(new Label { Text = "姓名：" }, 0, 0);  // 列0, 行0
table.Controls.Add(new TextBox(), 1, 0);                    // 列1, 行0
```

**SizeType 枚举值：** `Absolute`（绝对像素）, `Percent`（百分比）, `AutoSize`（自动大小）

**支持特性：** RowSpan, ColumnSpan, GrowStyle (AddRows/AddColumns/FixedSize)

### 6.4 StackLayoutEngine（堆栈布局）

简单的堆栈布局引擎，水平或垂直排列子控件。

### 6.5 布局事务

```csharp
// 挂起布局（批量修改属性时避免重复布局）
SuspendLayout();

// 修改多个属性...
button1.Location = new Point(10, 10);
button2.Location = new Point(10, 50);
panel.Size = new Size(300, 200);

// 恢复布局（立即执行一次布局）
ResumeLayout();

// 或者恢复但不立即布局
ResumeLayout(performLayout: false);
```

`LayoutTransaction` 是内部使用的布局事务辅助类，确保布局在 using 块结束时执行。

### 6.6 AutoSize 自适应大小

```csharp
var button = new Button
{
    AutoSize = true,
    AutoSizeMode = AutoSizeMode.GrowAndShrink  // 或 GrowOnly
};
```

**AutoSizeMode 枚举值：**
- `GrowAndShrink`：根据内容自动增大和缩小
- `GrowOnly`：只自动增大，不缩小（默认）

---

## 7. 主题与样式系统

### 7.1 Theme 全局主题

`Theme` 是静态类，提供全局主题资源库。

**主题颜色属性：**

| 属性 | 说明 | Light 默认值 | Dark 默认值 |
|------|------|-------------|------------|
| `BackgroundColor` | 窗体/控件默认背景 | #F0F0F0 | #282828 |
| `ForegroundColor` | 文本颜色 | #000000 | #DEDEDE |
| `ForegroundDisabledColor` | 禁用文本颜色 | #AAAAAA | #969696 |
| `ForegroundColorOnAccent` | 强调色上的文本 | White | White |
| `AccentColor` | 主强调色 | #2A8AD0 | #096085 |
| `AccentColor2` | 副强调色 | #0078D4 | #0078D4 |
| `BorderLowColor` | 边框低色 | #ABABAB | #505050 |
| `BorderMidColor` | 边框中色 | #808080 | #808080 |
| `BorderHighColor` | 边框高色 | #333333 | #A0A0A0 |
| `ControlLowColor` | 控件低色 | #FBFBFB | #282828 |
| `ControlMidColor` | 控件中色 | #F3F3F3 | #505050 |
| `ControlMidHighColor` | 控件中高色 | #E1E1E1 | #686868 |
| `ControlHighColor` | 控件高色 | #C2C3C9 | #808080 |
| `ControlVeryHighColor` | 控件极高色 | #686868 | #EFEBEF |
| `TextSelectionBackgroundColor` | 文本选中背景 | #99C9EF | #99C9EF |
| `WarningHighlightColor` | 警告高亮色 | #E81123 | #E81123 |

**字体属性：**

| 属性 | 类型 | 说明 |
|------|------|------|
| `UIFont` | `SKTypeface` | 默认字体 |
| `UIFontBold` | `SKTypeface` | 默认粗体字体 |
| `FontSize` | `int` | 默认字体大小 (14) |
| `ItemFontSize` | `int` | 列表项字体大小 (12) |

**切换主题：**

```csharp
// 切换到深色主题
Theme.SetBuiltInTheme(BuiltInTheme.Dark);

// 切换到浅色主题
Theme.SetBuiltInTheme(BuiltInTheme.Default);
```

**自定义主题颜色：**

```csharp
Theme.BeginUpdate();  // 批量修改，避免频繁重绘
Theme.AccentColor = SKColor.Parse("#FF6600");
Theme.BackgroundColor = SKColor.Parse("#FAFAFA");
Theme.EndUpdate();    // 结束批量修改，触发重绘
```

**ThemeChanged 事件：**

```csharp
Theme.ThemeChanged += (s, e) =>
{
    // 主题变更时更新自定义绘制
};
```

### 7.2 ControlStyle 样式继承链

`ControlStyle` 实现父子链式继承，每个控件有 `Style`（常态）和 `StyleHover`（悬停态）两个样式实例。

```csharp
// 控件定义默认样式
public static readonly ControlStyle DefaultStyle = new ControlStyle(null,
    (style) => {
        style.BackgroundColor = Theme.BackgroundColor;
        style.ForegroundColor = Theme.ForegroundColor;
    });

// 实例样式继承自默认样式
public virtual ControlStyle Style { get; } = new ControlStyle(DefaultStyle);
```

**样式属性：**

| 属性 | 类型 | 说明 |
|------|------|------|
| `BackgroundColor` | `SKColor?` | 背景色 |
| `ForegroundColor` | `SKColor?` | 前景色 |
| `Font` | `SKTypeface?` | 字体 |
| `FontSize` | `int?` | 字体大小 |
| `Border` | `BorderStyle` | 边框样式 |

**计算属性（沿继承链回退）：**

```csharp
style.GetBackgroundColor()  // 本地值 ?? 父级值 ?? Theme 默认值
style.GetForegroundColor()  // 本地值 ?? 父级值 ?? Theme 默认值
style.GetFont()             // 本地值 ?? 父级值 ?? Theme.UIFont
style.GetFontSize()         // 本地值 ?? 父级值 ?? Theme.FontSize
```

**自定义控件样式：**

```csharp
var button = new Button();
button.Style.BackgroundColor = SKColors.LightBlue;
button.Style.ForegroundColor = SKColors.DarkBlue;
button.Style.Border.Color = SKColors.Blue;
button.Style.Border.Width = 2;
button.Style.Border.Radius = 5;  // 圆角
```

**CurrentStyle 自动切换：**

```csharp
// 根据鼠标是否悬停自动切换 Style / StyleHover
public ControlStyle CurrentStyle => IsHovering ? StyleHover : Style;
```

### 7.3 BorderStyle 边框样式

`BorderStyle` 同样采用继承链，支持四边独立设置。

```csharp
// 统一设置
style.Border.Color = SKColors.Gray;
style.Border.Width = 1;
style.Border.Radius = 3;

// 四边独立设置
style.Border.Left.Color = SKColors.Red;
style.Border.Left.Width = 2;
style.Border.Top.Width = 0;    // 无上边框
style.Border.Right.Width = 0;  // 无右边框
style.Border.Bottom.Width = 1;
```

### 7.4 自定义主题

参考 Outlaw 示例中的 `CustomTheme.cs`：

```csharp
public static class CustomTheme
{
    public static void ApplyGreenTheme()
    {
        Theme.BeginUpdate();
        Theme.AccentColor = SKColor.Parse("#2E7D32");
        Theme.AccentColor2 = SKColor.Parse("#1B5E20");
        Theme.BackgroundColor = SKColor.Parse("#F1F8E9");
        Theme.EndUpdate();
    }

    public static void ApplyOrangeTheme()
    {
        Theme.BeginUpdate();
        Theme.AccentColor = SKColor.Parse("#E65100");
        Theme.AccentColor2 = SKColor.Parse("#BF360C");
        Theme.BackgroundColor = SKColor.Parse("#FFF3E0");
        Theme.EndUpdate();
    }
}
```

---

## 8. 渲染系统

### 8.1 Renderer<T> 渲染器基类

```csharp
public abstract class Renderer { }
public abstract class Renderer<T> : Renderer where T : Control
{
    public override Type Type => typeof(T);
    protected abstract void Render(T control, PaintEventArgs e);
}
```

每个控件类型有对应的 `Renderer<T>` 实现，负责使用 SkiaSharp 绘制控件外观。

### 8.2 RenderManager 渲染器注册与查找

`RenderManager` 是静态类，管理所有渲染器的注册和查找。

```csharp
// 注册渲染器（通常在静态构造函数中完成）
RenderManager.SetRenderer<Button>(new ButtonRenderer());

// 获取渲染器
var renderer = RenderManager.GetRenderer<Renderer>(control);

// 渲染控件
RenderManager.Render(control, e);
```

**内置渲染器注册表：**

| 控件 | 渲染器 |
|------|--------|
| Button | ButtonRenderer |
| CheckBox | CheckBoxRenderer |
| ComboBox | ComboBoxRenderer |
| DataGridView | DataGridViewRenderer |
| FormTitleBar | FormTitleBarRenderer |
| Label | LabelRenderer |
| LinkLabel | LinkLabelRenderer |
| ListBox | ListBoxRenderer |
| ListView | ListViewRenderer |
| Menu | MenuRenderer |
| MenuDropDown | MenuDropDownRenderer |
| NavigationPane | NavigationPaneRenderer |
| Panel | PanelRenderer |
| PictureBox | PictureBoxRenderer |
| ProgressBar | ProgressBarRenderer |
| RadioButton | RadioButtonRenderer |
| Ribbon | RibbonRenderer |
| ScrollableControl | ScrollableControlRenderer |
| ScrollBar | ScrollBarRenderer |
| SplitContainer | SplitContainerRenderer |
| Splitter | SplitterRenderer |
| StatusBar | StatusBarRenderer |
| TabControl | TabControlRenderer |
| TabStrip | TabStripRenderer |
| TextBox | TextBoxRenderer |
| ToolBar | ToolBarRenderer |
| TrackBar | TrackBarRenderer |
| TreeView | TreeViewRenderer |

### 8.3 自定义渲染器

```csharp
public class RoundButtonRenderer : Renderer<Button>
{
    protected override void Render(Button control, PaintEventArgs e)
    {
        var style = control.CurrentStyle;
        var bounds = control.ScaledBounds;

        // 绘制圆角背景
        using var paint = new SKPaint
        {
            Color = style.GetBackgroundColor(),
            IsAntialias = true
        };
        e.Canvas.DrawRoundRect(bounds.X, bounds.Y, bounds.Width, bounds.Height,
            15, 15, paint);

        // 绘制文本
        using var textPaint = new SKPaint
        {
            Color = style.GetForegroundColor(),
            TextSize = style.GetFontSize(),
            Typeface = style.GetFont(),
            IsAntialias = true
        };
        e.Canvas.DrawText(control.Text, bounds.Left + 10, bounds.Bottom - 10, textPaint);
    }
}

// 注册自定义渲染器
RenderManager.SetRenderer<Button>(new RoundButtonRenderer());
```

---

## 9. 事件系统

### 9.1 事件属性模式

Modern.Forms 采用 WinForms 的**事件属性模式**（Event Property Pattern），通过 `EventHandlerList` 存储，节省内存：

```csharp
// 静态 key 对象（每个事件一个，不随实例分配）
private static readonly object s_clickEvent = new object();

// 事件定义
public event EventHandler<MouseEventArgs>? Click {
    add => Events.AddHandler(s_clickEvent, value);
    remove => Events.RemoveHandler(s_clickEvent, value);
}

// 事件触发
protected virtual void OnClick(MouseEventArgs e) {
    (Events[s_clickEvent] as EventHandler<MouseEventArgs>)?.Invoke(this, e);
}
```

### 9.2 鼠标事件

| 事件 | 参数类型 | 说明 |
|------|---------|------|
| `Click` | `MouseEventArgs` | 单击 |
| `DoubleClick` | `MouseEventArgs` | 双击 |
| `MouseDown` | `MouseEventArgs` | 鼠标按下 |
| `MouseUp` | `MouseEventArgs` | 鼠标释放 |
| `MouseMove` | `MouseEventArgs` | 鼠标移动 |
| `MouseEnter` | `MouseEventArgs` | 鼠标进入 |
| `MouseLeave` | `EventHandler` | 鼠标离开 |
| `MouseWheel` | `MouseEventArgs` | 鼠标滚轮 |

**MouseEventArgs 属性：** `Button`, `Clicks`, `X`, `Y`, `Delta`, `Location`

### 9.3 键盘事件

| 事件 | 参数类型 | 说明 |
|------|---------|------|
| `KeyDown` | `KeyEventArgs` | 键按下 |
| `KeyUp` | `KeyEventArgs` | 键释放 |
| `KeyPress` | `KeyPressEventArgs` | 字符输入 |

**KeyEventArgs 属性：** `KeyCode`, `KeyData`, `Modifiers`, `Alt`, `Control`, `Shift`, `Handled`

### 9.4 焦点事件

| 事件 | 参数类型 | 说明 |
|------|---------|------|
| `GotFocus` | `EventHandler` | 获得焦点 |
| `LostFocus` | `EventHandler` | 失去焦点 |

### 9.5 属性变更事件

| 事件 | 说明 |
|------|------|
| `TextChanged` | 文本变更 |
| `EnabledChanged` | 启用状态变更 |
| `VisibleChanged` | 可见性变更 |
| `LocationChanged` | 位置变更 |
| `SizeChanged` | 大小变更 |
| `Resize` | 大小调整 |
| `DockChanged` | 停靠方式变更 |
| `CursorChanged` | 光标变更 |
| `ContextMenuChanged` | 上下文菜单变更 |
| `PaddingChanged` | 内边距变更 |
| `MarginChanged` | 外边距变更 |
| `AutoSizeChanged` | 自动大小变更 |
| `TabIndexChanged` | Tab 顺序变更 |
| `TabStopChanged` | Tab 停靠变更 |

### 9.6 控件树事件

| 事件 | 参数类型 | 说明 |
|------|---------|------|
| `ControlAdded` | `EventArgs<Control>` | 子控件添加 |
| `ControlRemoved` | `EventArgs<Control>` | 子控件移除 |
| `ParentChanged` | `EventHandler` | 父控件变更 |

### 9.7 事件路由机制

鼠标和键盘事件从 `WindowBase.OnInput()` 开始，通过 `ControlAdapter` 路由到控件树：

1. 窗口接收平台原始输入事件（来自 Modern.WindowKit）
2. 转换为 Modern.Forms 事件参数
3. 调用 `ControlAdapter.RaiseMouseDown/RaiseClick/RaiseKeyDown` 等
4. 控件通过 `Controls.GetAllControls()` 找到命中测试的子控件
5. 递归向下路由到最内层的命中控件
6. 支持鼠标捕获（`Capture` 属性）

---

## 10. 平台抽象层

### 10.1 Modern.WindowKit 概述

Modern.WindowKit 是从 Avalonia 的核心窗口抽象层剥离出来的独立包，提供：

- `IWindowBaseImpl` / `IWindowImpl`：平台窗口实现接口
- `RawInputEventArgs`：原始输入事件
- `IFramebufferPlatformSurface`：帧缓冲绘制表面
- `Dispatcher.UIThread.MainLoop`：UI 线程主循环

### 10.2 跨平台窗口管理

Modern.WindowKit 支持以下平台后端：
- **Windows**：Win32 API
- **macOS**：Cocoa/AppKit
- **Linux**：X11

### 10.3 输入事件处理

平台原始输入事件通过 `IWindowBaseImpl.Input` 回调传递到 `WindowBase.OnInput()`，然后转换为 Modern.Forms 的事件参数路由到控件树。

### 10.4 DPI 缩放

Modern.Forms 支持 DPI 感知：
- `ScaleFactor` 属性获取当前缩放因子
- `GetScaledBounds()` 方法将未缩放的边界转换为缩放后的边界
- `ScaledBounds` / `ScaledSize` 属性返回缩放后的值
- 布局引擎内部处理 DPI 缩放

---

## 11. 示例应用解析

### 11.1 ControlGallery 控件画廊

最完整的示例，展示每个控件的用法。每个控件有独立的 Panel 展示页面。

**项目结构：**
- `MainForm.cs`：主窗体，左侧导航列表 + 右侧展示区
- `Panels/BasePanel.cs`：展示页面基类
- `Panels/ButtonPanel.cs`、`Panels/TextBoxPanel.cs` 等：各控件展示

**关键学习点：**
- 如何使用 NavigationPane 做侧边导航
- 每个控件的基本用法和属性设置
- 事件处理模式

### 11.2 Explorer 文件浏览器

Windows Explorer 风格应用，展示更复杂的布局。

**关键学习点：**
- Ribbon 控件的使用
- TreeView + ListView 的主从布局
- StatusBar 的使用
- 自定义主题切换

### 11.3 Outlaw 邮件客户端

Outlook 风格应用，展示复杂 UI 组合。

**关键学习点：**
- NavigationPane 做侧边导航
- SplitContainer 分割布局
- 自定义主题 (CustomTheme.cs)
- 复杂控件组合
- **传统 Dock 布局模式**：Outlaw 使用 `UseForwardDockOrder = false`（WinForms 传统模式），Dock.Left 控件的布局顺序与添加顺序相反。例如先添加 email_list (Width=325)，再添加 nav_tree_view (Width=225)，在传统模式下 nav_tree_view 会出现在最左侧

---

## 附录

### A. 常用命名空间

```csharp
using Modern.Forms;              // 核心控件和类
using Modern.Forms.Renderers;    // 渲染器
using Modern.Forms.Layout;       // 布局引擎（内部）
using System.Drawing;            // Point, Size, Rectangle
using SkiaSharp;                 // SKColor, SKBitmap, SKTypeface
```

### B. 从 WinForms 迁移指南

| WinForms | Modern.Forms | 说明 |
|----------|-------------|------|
| `System.Windows.Forms.Form` | `Modern.Forms.Form` | 窗体 |
| `System.Windows.Forms.Button` | `Modern.Forms.Button` | 按钮 |
| `this.Controls.Add(ctrl)` | `Controls.Add(ctrl)` | 返回添加的控件 |
| `Color.Red` | `SKColors.Red` | 颜色使用 SkiaSharp |
| `Font` | `SKTypeface` + `FontSize` | 字体分离为字形+大小 |
| `PaintEventArgs.Graphics` | `PaintEventArgs.Canvas` | SkiaSharp 画布 |
| `MessageBox.Show()` | `MessageBox.Show()` | 消息框 |
| `Application.Run()` | `Application.Run()` | 应用启动 |

### C. 项目状态

Modern.Forms 目前版本 0.3.0，处于早期阶段。以下功能尚未实现：
- 数据绑定 (Data Binding)
- 无障碍访问 (Accessibility)
- 右到左布局 (RTL)
- 可视化设计器 (Visual Designer)
- 拖放 (Drag & Drop)
- 打印 (Printing)
