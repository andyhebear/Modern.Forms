# RS.AvaForms 项目开发设计文档

> **定位**：基于 Avalonia 的 Code-only UI 框架，提供类似 WinForms 的简单开发体验。
>
> **核心原则**：不替代 Avalonia，而是在 Avalonia 之上提供"开发体验封装"——直接继承 Avalonia 控件 + 扩展方法，保留 Avalonia 全部能力，同时让用户用 WinForms 的直觉写代码。
>
> **架构决策**：控件直接继承 Avalonia 控件，没有 Core/UI 分离层，没有适配器模式。Avalonia 就是唯一的底层 UI 框架，不做"可替换 UI 框架"的抽象。这样做更简单、更自然、更轻量，也完全兼容 Avalonia 的样式、绑定和主题生态。
>
> **一句话定位**：用 WinForms 的方式写 Avalonia。

---

## 一、项目边界与目标

### 1.1 项目是什么

RS.AvaForms 是一个构建在 Avalonia 之上的 Code-only UI 框架，旨在为现代跨平台桌面应用提供类似 WinForms 的简单开发体验。

它不应该替代 Avalonia，而应该是 Avalonia 之上的一层"开发体验封装"。Avalonia 官方本身支持完全使用 C# 构建UI，也支持控件、布局、样式、绑定和动画的代码式创建，所以 RS.AvaForms 的方向是可行的。设计重点不在底层渲染，而在 API 易用性、控件抽象、布局模型、事件模型、样式系统和项目模板。

### 1.2 三层目标

| 层级 | 目标 | 示例 |
|------|------|------|
| 初级 | 像 WinForms 一样快速创建窗口和控件 | `new Form().SetContent(new Button())` |
| 中级 | 提供流式 API、布局封装和事件封装 | `.Text("OK").OnClick(...)` |
| 高级 | 兼容 MVVM、主题、DI、数据绑定 | `.BindText("UserName")` |

### 1.3 不做什么

- ❌ 不重新实现 UI 渲染引擎
- ❌ 不屏蔽 Avalonia 的原生能力（用户随时可访问底层 Avalonia API）
- ❌ 不完全复刻 WinForms API（绝对定位、控件句柄、GDI 思维、同步 UI 模型不适合照搬）
- ❌ 不做 Core/UI 分离层（不做"可替换 UI 框架"的抽象，Avalonia 就是底层）
- ❌ 不做可视化设计器（第一版）
- ❌ 不做热重载（第一版）
- ❌ 不做完整控件库（第一版只做基础控件）

### 1.4 核心取舍

| 取舍 | 决策 | 理由 |
|------|------|------|
| 继承 vs 包装 Avalonia 控件 | **直接继承 Avalonia 控件** | 简单、兼容性强、样式好接入、绑定天然支持 |
| 适配器 vs 直接继承 | **直接继承，无适配器** | 避免为"可替换 UI 框架"引入的复杂度 |
| 属性式 vs 流式 API | **两种都支持** | 属性式适合 WinForms 用户，流式适合现代 C# 用户 |
| 事件驱动 vs MVVM | **两种都支持** | 事件驱动是默认，MVVM 通过 Avalonia 绑定可选启用 |
| 绝对布局 vs 自适应布局 | **两种都支持** | AbsoluteLayout 兼容旧代码，VStack/HStack/Dock 鼓励新项目 |
| 硬编码样式 vs Classes 样式 | **走 Avalonia Classes** | 不硬编码颜色，用 `.Primary()` 等添加样式类，主题统一控制 |

### 1.5 关键原则

1. **不要完全复刻 WinForms**：WinForms 的绝对定位、控件句柄、GDI 思维、同步 UI 模型都不适合完整照搬。复刻的是"简单直观的开发体验"，不是复刻历史 API。
2. **不要和 Avalonia 对抗**：Avalonia 的布局、样式、绑定和主题是优势，RS.AvaForms 应该包装它们，而不是绕开它们。
3. **默认简单，必要时暴露底层**：用户默认不需要引用 Avalonia 类型，但需要时可以无缝访问。
4. **文档和示例优先**：这类项目能不能被接受，很大程度上取决于用户 5 分钟内能不能写出第一个窗口。

---

## 二、整体架构

### 2.1 项目结构

```
RS.AvaForms/
├── RS.AvaForms/                        # 核心库（直接依赖 Avalonia）
│   ├── RS.AvaForms.csproj
│   ├── Application/                    # 应用启动
│   │   ├── AvaFormsApp.cs             #   静态入口
│   │   ├── AvaFormsAppBuilder.cs      #   Builder 模式
│   │   ├── IAvaFormsApp.cs            #   应用接口
│   │   └── AvaFormsApplication.cs     #   内部 Avalonia Application 子类
│   ├── Controls/                       # 控件（直接继承 Avalonia 控件）
│   │   ├── Form.cs                    #   窗口
│   │   ├── Form{TViewModel}.cs        #   泛型窗体
│   │   ├── Button.cs                  #   按钮
│   │   ├── Label.cs                   #   文本标签
│   │   ├── TextBox.cs                 #   文本输入
│   │   ├── CheckBox.cs                #   复选框
│   │   ├── RadioButton.cs             #   单选按钮
│   │   ├── ComboBox.cs                #   下拉框
│   │   ├── ListBox.cs                 #   列表
│   │   ├── GroupBox.cs                #   分组框
│   │   ├── ProgressBar.cs             #   进度条
│   │   ├── TrackBar.cs                #   滑块
│   │   ├── TabControl.cs              #   选项卡
│   │   ├── PictureBox.cs              #   图片
│   │   ├── Menu.cs                    #   菜单
│   │   ├── ToolBar.cs                 #   工具栏
│   │   ├── StatusBar.cs               #   状态栏
│   │   ├── TreeView.cs                #   树形控件
│   │   ├── DataGridView.cs            #   数据表格
│   │   └── MessageBox.cs             #   消息框
│   ├── Layouts/                        # 布局容器
│   │   ├── VStack.cs                  #   垂直堆叠
│   │   ├── HStack.cs                  #   水平堆叠
│   │   ├── DockLayout.cs             #   停靠布局
│   │   ├── GridForm.cs               #   表单网格
│   │   └── AbsoluteLayout.cs         #   绝对定位
│   ├── Extensions/                     # 流式扩展方法
│   │   ├── ControlExtensions.cs       #   通用扩展（Margin、Size、Name、Tooltip...）
│   │   ├── ButtonExtensions.cs        #   按钮扩展（OnClick、Primary、Danger...）
│   │   ├── TextBoxExtensions.cs       #   文本框扩展（Placeholder、Password...）
│   │   ├── CheckBoxExtensions.cs      #   复选框扩展（OnChanged、Checked...）
│   │   └── FormExtensions.cs          #   窗体扩展（CenterScreen、SetSize...）
│   ├── Themes/                         # 主题系统
│   │   ├── AvaFormsTheme.cs           #   主题枚举
│   │   ├── ThemeManager.cs            #   主题管理器
│   │   └── Styles/                    #   默认样式（axaml 或代码定义）
│   │       ├── BaseStyles.axaml       #   基础样式
│   │       ├── ButtonStyles.axaml     #   按钮样式变体
│   │       └── LightDarkStyles.axaml  #   明暗主题
│   ├── Binding/                        # 绑定辅助
│   │   └── BindingExtensions.cs       #   BindText、BindCommand、BindChecked...
│   └── Diagnostics/                    # 诊断日志
│       ├── IDiagnosticLogger.cs        #   日志接口
│       ├── LogLevel.cs                 #   日志级别
│       ├── DiagnosticLogger.cs         #   默认实现（Debug + Console）
│       ├── NullLogger.cs              #   静默实现
│       └── FileLogger.cs              #   文件日志实现
│
├── RS.AvaForms.Generator/              # JSON → C# 代码生成
│   ├── RS.AvaForms.Generator.csproj
│   ├── Models/                         #   DTO（record 类型）
│   │   ├── FormDefinition.cs
│   │   ├── ControlDefinition.cs
│   │   └── PropertyDefinition.cs
│   ├── JsonParser.cs                   #   JSON 解析器
│   └── CSharpCodeGenerator.cs         #   C# 代码生成器
│
├── RS.AvaForms.Designer/               # 可视化设计器（后期阶段）
│
├── tests/
│   ├── RS.AvaForms.Tests/             # 核心库测试
│   └── RS.AvaForms.Generator.Tests/   # 生成器测试
│
└── samples/
    ├── HelloWorld/                     # 最小示例
    ├── LoginDemo/                      # 登录窗口示例
    └── SettingsDemo/                   # 设置窗口示例
```

### 2.2 依赖关系

```
RS.AvaForms ──依赖──→ Avalonia
                      Avalonia.Desktop
                      Avalonia.Themes.Fluent
                      Avalonia.Fonts.Inter
                      Microsoft.Extensions.DependencyInjection（可选）

RS.AvaForms.Generator ──依赖──→ RS.AvaForms

RS.AvaForms.Designer ──依赖──→ RS.AvaForms + RS.AvaForms.Generator（后期）
```

**核心特征**：只有一个 UI 层，没有 Core/UI 分离。用户代码直接使用 `RS.AvaForms.Controls.Button`，它就是 `Avalonia.Controls.Button` 的子类。

### 2.3 命名空间设计

```csharp
using RS.AvaForms;              // AvaFormsApp、Form
using RS.AvaForms.Controls;     // Button、Label、TextBox、CheckBox...
using RS.AvaForms.Layouts;      // VStack、HStack、DockLayout、GridForm、AbsoluteLayout
using RS.AvaForms.Themes;       // AvaFormsTheme
using RS.AvaForms.Extensions;   // 流式扩展方法
using RS.AvaForms.Binding;      // BindText、BindCommand
using RS.AvaForms.Diagnostics;  // IDiagnosticLogger、FileLogger
```

用户代码应该尽量少引用 Avalonia 类型。不是完全禁止，而是默认场景下不需要。

---

## 三、控件设计

### 3.1 设计原则

控件**直接继承 Avalonia 控件**，添加 WinForms 风格的属性别名和便捷构造函数。流式 API 用扩展方法实现，不在控件类中堆砌。

```csharp
namespace RS.AvaForms.Controls;

public class Button : global::Avalonia.Controls.Button
{
    public Button() { }

    public Button(string text) { Content = text; }

    public string? Text
    {
        get => Content?.ToString();
        set => Content = value;
    }
}
```

### 3.2 控件映射表

| AvaForms 控件 | 继承自 | 主要添加 |
|---------------|--------|---------|
| `Form` | `Window` | `Text`→`Title`、`ShowForm()`、`ShowDialogForm()`、`FormClosing`/`FormClosed` |
| `Button` | `Avalonia.Controls.Button` | `Text`→`Content`、便捷构造函数 |
| `Label` | `TextBlock` | `Text` 统一属性 |
| `TextBox` | `Avalonia.Controls.TextBox` | `Placeholder`→`Watermark`、`Password()` |
| `CheckBox` | `Avalonia.Controls.CheckBox` | `Text`→`Content`、`Checked` 事件 |
| `RadioButton` | `Avalonia.Controls.RadioButton` | `Text`→`Content` |
| `ComboBox` | `Avalonia.Controls.ComboBox` | `Items` 简化 |
| `ListBox` | `Avalonia.Controls.ListBox` | 数据源绑定 |
| `GroupBox` | `Avalonia.Controls.GroupBox` | `Text`→`Header` |
| `ProgressBar` | `Avalonia.Controls.ProgressBar` | `Value`、`Minimum`、`Maximum` |
| `TrackBar` | `Slider` | `Value`、`Minimum`、`Maximum` |
| `TabControl` | `Avalonia.Controls.TabControl` | `TabPages` |
| `PictureBox` | `Image` | 图片显示 |
| `TreeView` | `Avalonia.Controls.TreeView` | 树形控件 |
| `DataGridView` | `DataGrid` | 表格控件 |
| `Menu` | `Avalonia.Controls.Menu` | 菜单栏 |
| `Panel` | `Avalonia.Controls.Panel` | 子控件管理 |
| `StatusBar` | `Border` + `TextBlock` | 状态栏封装 |
| `ToolBar` | `StackPanel` + 按钮 | 工具栏封装 |

### 3.3 Form 设计

Form 是项目的门面类，底层直接继承 Avalonia 的 Window：

```csharp
namespace RS.AvaForms.Controls;

public class Form : Window
{
    public string? Text
    {
        get => Title;
        set => Title = value;
    }

    public Form()
    {
        Closing += OnFormClosing;
        Closed += OnFormClosed;
    }

    public Form CenterScreen()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        return this;
    }

    public Form SetSize(double width, double height)
    {
        Width = width;
        Height = height;
        return this;
    }

    public void ShowForm()
    {
        Show();
    }

    public async Task ShowDialogForm(Window? owner = null)
    {
        if (owner is not null)
            await ShowDialog(owner);
        else
            Show();
    }

    public event EventHandler<CancelEventArgs>? FormClosing;
    public event EventHandler? FormClosed;

    private void OnFormClosing(object? sender, WindowClosingEventArgs e)
    {
        var args = new CancelEventArgs();
        FormClosing?.Invoke(this, args);
        e.Cancel = args.Cancel;
    }

    private void OnFormClosed(object? sender, EventArgs e)
    {
        FormClosed?.Invoke(this, EventArgs.Empty);
    }
}
```

### 3.4 Form\<TViewModel\> 泛型窗体

```csharp
namespace RS.AvaForms.Controls;

public class Form<TViewModel> : Form where TViewModel : class, new()
{
    public TViewModel ViewModel { get; }

    public Form()
    {
        ViewModel = new TViewModel();
        DataContext = ViewModel;
    }

    public Form(TViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
    }
}
```

### 3.5 统一 Text 概念

Avalonia 中不同控件表达文字的属性不同。RS.AvaForms 统一提供 `Text` 属性以降低学习成本：

| 控件 | Text 映射 | 实现方式 |
|------|----------|---------|
| Button | `Content` | `Content?.ToString()` |
| Label | `Text` | 直接一致 |
| TextBox | `Text` | 直接一致 |
| CheckBox | `Content` | `Content?.ToString()` |
| RadioButton | `Content` | `Content?.ToString()` |
| GroupBox | `Header` | `Header?.ToString()` |
| Form | `Title` | 窗口标题 |

### 3.6 各控件详细设计

#### Button

```csharp
public class Button : global::Avalonia.Controls.Button
{
    public Button() { }
    public Button(string text) { Content = text; }

    public string? Text
    {
        get => Content?.ToString();
        set => Content = value;
    }
}
```

#### Label

```csharp
public class Label : TextBlock
{
    public Label() { }
    public Label(string text) { Text = text; }
}
```

#### TextBox

```csharp
public class TextBox : global::Avalonia.Controls.TextBox
{
    public string? Placeholder
    {
        get => Watermark;
        set => Watermark = value;
    }

    public TextBox Password()
    {
        PasswordChar = '●';
        return this;
    }
}
```

#### CheckBox

```csharp
public class CheckBox : global::Avalonia.Controls.CheckBox
{
    public string? Text
    {
        get => Content?.ToString();
        set => Content = value;
    }

    public bool Checked
    {
        get => IsChecked == true;
        set => IsChecked = value;
    }
}
```

#### RadioButton

```csharp
public class RadioButton : global::Avalonia.Controls.RadioButton
{
    public string? Text
    {
        get => Content?.ToString();
        set => Content = value;
    }
}
```

#### GroupBox

```csharp
public class GroupBox : global::Avalonia.Controls.GroupBox
{
    public string? Text
    {
        get => Header?.ToString();
        set => Header = value;
    }
}
```

#### MessageBox

```csharp
public static class MessageBox
{
    public static void Show(string message, string title = "提示");
    public static Task ShowAsync(string message, string title = "提示");
    public static Task<bool> ConfirmAsync(string message, string title = "确认");
    public static Task ShowErrorAsync(string message, string title = "错误");
}
```

---

## 四、布局系统设计

布局是项目成败的关键。WinForms 的绝对定位很简单，但跨平台、高 DPI、不同字体和窗口缩放时容易出问题。RS.AvaForms 顺着 Avalonia 的布局模型设计，提供 WinForms-like 的入口，但底层鼓励更现代的布局方式。

### 4.1 布局容器

| AvaForms 布局 | 适合场景 | 底层 Avalonia | 推荐度 |
|---------------|---------|---------------|--------|
| `VStack` | 垂直表单、设置页 | `StackPanel(Vertical)` | ⭐⭐⭐ 默认推荐 |
| `HStack` | 横向按钮组、工具栏 | `StackPanel(Horizontal)` | ⭐⭐⭐ 默认推荐 |
| `GridForm` | 复杂表单、两列表单 | `Grid` | ⭐⭐⭐ 默认推荐 |
| `DockLayout` | 主窗口（菜单+状态栏+内容区） | `DockPanel` | ⭐⭐ 常用 |
| `AbsoluteLayout` | 兼容 WinForms 绝对定位 | `Canvas` | ⭐ 迁移用 |

**文档建议**：简单迁移可以用绝对布局，新项目优先使用自适应布局。

### 4.2 VStack / HStack

```csharp
namespace RS.AvaForms.Layouts;

public class VStack : StackPanel
{
    public static VStack Create() => new() { Orientation = Orientation.Vertical };

    public VStack Spacing(double value) { Spacing = value; return this; }

    public VStack Padding(double value) { Margin = new Thickness(value); return this; }

    public VStack Padding(double l, double t, double r, double b)
    { Margin = new Thickness(l, t, r, b); return this; }

    public VStack Add(Control control) { Children.Add(control); return this; }
}

public class HStack : StackPanel
{
    public static HStack Create() => new() { Orientation = Orientation.Horizontal };

    public HStack Spacing(double value) { Spacing = value; return this; }

    public HStack Padding(double value) { Margin = new Thickness(value); return this; }

    public HStack Add(Control control) { Children.Add(control); return this; }

    public HStack AlignRight() { HorizontalAlignment = HorizontalAlignment.Right; return this; }

    public HStack AlignCenter() { HorizontalAlignment = HorizontalAlignment.Center; return this; }
}
```

### 4.3 DockLayout

```csharp
namespace RS.AvaForms.Layouts;

public class DockLayout : DockPanel
{
    public static DockLayout Create() => new();

    public DockLayout Add(Control control, Dock dock = Dock.Left)
    {
        DockPanel.SetDock(control, dock);
        Children.Add(control);
        return this;
    }

    public DockLayout LastChildFill(bool value) { LastChildFill = value; return this; }
}
```

### 4.4 GridForm

GridForm 是专门为表单场景设计的布局容器，自动生成"标签+输入"两列网格：

```csharp
namespace RS.AvaForms.Layouts;

public class GridForm : Grid
{
    private double _labelWidth = 120;

    public static GridForm Create(int columns = 1, double labelWidth = 120)
    {
        var grid = new GridForm { _labelWidth = labelWidth };
        for (int i = 0; i < columns; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(labelWidth, GridUnitType.Pixel));
            grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
        }
        return grid;
    }

    public GridForm AddRow(string label, Control control)
    {
        var row = RowDefinitions.Count;
        RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        var textBlock = new TextBlock
        {
            Text = label,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 4)
        };
        Children.Add(textBlock);
        Grid.SetColumn(textBlock, 0);
        Grid.SetRow(textBlock, row);

        Children.Add(control);
        Grid.SetColumn(control, 1);
        Grid.SetRow(control, row);

        return this;
    }

    public GridForm AddRow(Control control)
    {
        var row = RowDefinitions.Count;
        RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Children.Add(control);
        Grid.SetColumnSpan(control, 2);
        Grid.SetRow(control, row);
        return this;
    }
}
```

### 4.5 AbsoluteLayout

```csharp
namespace RS.AvaForms.Layouts;

public class AbsoluteLayout : Canvas
{
    public static AbsoluteLayout Create() => new();

    public AbsoluteLayout Add(Control control, double x, double y, double width, double height)
    {
        control.Width = width;
        control.Height = height;
        SetLeft(control, x);
        SetTop(control, y);
        Children.Add(control);
        return this;
    }

    public AbsoluteLayout Add(Control control, double x, double y)
    {
        SetLeft(control, x);
        SetTop(control, y);
        Children.Add(control);
        return this;
    }
}
```

### 4.6 布局使用示例

```csharp
// 垂直表单布局（最常用）
Content = VStack.Create()
    .Padding(24)
    .Spacing(10)
    .Add(new Label("账号"))
    .Add(new TextBox().Placeholder("请输入账号"))
    .Add(new Label("密码"))
    .Add(new TextBox().Password().Placeholder("请输入密码"))
    .Add(HStack.Create()
        .Spacing(8)
        .AlignRight()
        .Add(new Button("取消").Secondary())
        .Add(new Button("登录").Primary()));

// 表单网格布局
Content = GridForm.Create()
    .AddRow("用户名", new TextBox().Placeholder("请输入"))
    .AddRow("密码", new TextBox().Password().Placeholder("请输入"))
    .AddRow("记住我", new CheckBox { Text = "记住登录状态" }));

// 主窗口 Dock 布局
Content = DockLayout.Create()
    .LastChildFill(true)
    .Add(new Border { Height = 32 }, Dock.Top)
    .Add(new Border { Height = 24 }, Dock.Bottom)
    .Add(new Border { Width = 200 }, Dock.Left)
    .Add(new TextBox { AcceptsReturn = true });

// 绝对定位布局（兼容 WinForms）
Content = AbsoluteLayout.Create()
    .Add(new Label("姓名"), x: 20, y: 20, width: 80, height: 28)
    .Add(new TextBox(), x: 110, y: 20, width: 200, height: 28)
    .Add(new Button("确定"), x: 110, y: 60, width: 90, height: 32);
```

---

## 五、流式扩展方法

### 5.1 设计原则

流式 API 用扩展方法实现，不在控件类中堆砌流式方法。这样控件类保持简洁，扩展方法按功能分组，用户按需引用。

### 5.2 通用扩展

```csharp
namespace RS.AvaForms.Extensions;

public static class ControlExtensions
{
    public static T Margin<T>(this T control, double value) where T : global::Avalonia.Controls.Control
    { control.Margin = new Thickness(value); return control; }

    public static T Margin<T>(this T control, double l, double t, double r, double b)
        where T : global::Avalonia.Controls.Control
    { control.Margin = new Thickness(l, t, r, b); return control; }

    public static T Size<T>(this T control, double width, double height)
        where T : global::Avalonia.Controls.Control
    { control.Width = width; control.Height = height; return control; }

    public static T Width<T>(this T control, double value)
        where T : global::Avalonia.Controls.Control
    { control.Width = value; return control; }

    public static T Height<T>(this T control, double value)
        where T : global::Avalonia.Controls.Control
    { control.Height = value; return control; }

    public static T Name<T>(this T control, string name)
        where T : global::Avalonia.Controls.Control
    { control.Name = name; return control; }

    public static T Tooltip<T>(this T control, string text)
        where T : global::Avalonia.Controls.Control
    { ToolTip.SetTip(control, text); return control; }

    public static T Visible<T>(this T control, bool value)
        where T : global::Avalonia.Controls.Control
    { control.IsVisible = value; return control; }

    public static T Enabled<T>(this T control, bool value)
        where T : global::Avalonia.Controls.Control
    { control.IsEnabled = value; return control; }

    public static T FontSize<T>(this T control, double size)
        where T : global::Avalonia.Controls.Control
    { control.FontSize = size; return control; }

    public static T Foreground<T>(this T control, IBrush brush)
        where T : global::Avalonia.Controls.Control
    { control.Foreground = brush; return control; }

    public static T Background<T>(this T control, IBrush brush)
        where T : global::Avalonia.Controls.Control
    { control.Background = brush; return control; }
}
```

### 5.3 Button 扩展

```csharp
namespace RS.AvaForms.Extensions;

public static class ButtonExtensions
{
    public static Button OnClick(this Button button, Action action)
    { button.Click += (_, _) => action(); return button; }

    public static Button OnClick(this Button button, Func<Task> action)
    { button.Click += async (_, _) => await action(); return button; }

    public static Button OnClick(this Button button, Action<Button> action)
    { button.Click += (_, _) => action(button); return button; }

    public static Button Primary(this Button button)
    { button.Classes.Add("primary"); return button; }

    public static Button Secondary(this Button button)
    { button.Classes.Add("secondary"); return button; }

    public static Button Danger(this Button button)
    { button.Classes.Add("danger"); return button; }

    public static Button Success(this Button button)
    { button.Classes.Add("success"); return button; }

    public static Button Ghost(this Button button)
    { button.Classes.Add("ghost"); return button; }

    public static Button Large(this Button button)
    { button.Classes.Add("large"); return button; }

    public static Button Small(this Button button)
    { button.Classes.Add("small"); return button; }

    public static Button Icon(this Button button, string iconName)
    { button.Classes.Add($"icon-{iconName}"); return button; }
}
```

### 5.4 TextBox 扩展

```csharp
namespace RS.AvaForms.Extensions;

public static class TextBoxExtensions
{
    public static TextBox Placeholder(this TextBox textBox, string text)
    { textBox.Watermark = text; return textBox; }

    public static TextBox Password(this TextBox textBox)
    { textBox.PasswordChar = '●'; return textBox; }

    public static TextBox ReadOnly(this TextBox textBox, bool value = true)
    { textBox.IsReadOnly = value; return textBox; }

    public static TextBox OnTextChanged(this TextBox textBox, Action<string> action)
    { textBox.TextChanged += (_, _) => action(textBox.Text ?? ""); return textBox; }

    public static TextBox Multiline(this TextBox textBox, bool value = true)
    { textBox.AcceptsReturn = value; return textBox; }
}
```

### 5.5 CheckBox 扩展

```csharp
namespace RS.AvaForms.Extensions;

public static class CheckBoxExtensions
{
    public static CheckBox OnChanged(this CheckBox checkBox, Action<bool> action)
    { checkBox.IsCheckedChanged += (_, _) => action(checkBox.IsChecked == true); return checkBox; }

    public static CheckBox Checked(this CheckBox checkBox, bool value = true)
    { checkBox.IsChecked = value; return checkBox; }
}
```

### 5.6 Form 扩展

```csharp
namespace RS.AvaForms.Extensions;

public static class FormExtensions
{
    public static TForm CenterScreen<TForm>(this TForm form) where TForm : Form
    { form.WindowStartupLocation = WindowStartupLocation.CenterScreen; return form; }

    public static TForm SetSize<TForm>(this TForm form, double width, double height) where TForm : Form
    { form.Width = width; form.Height = height; return form; }

    public static TForm SetTitle<TForm>(this TForm form, string title) where TForm : Form
    { form.Title = title; return form; }

    public static TForm SetContent<TForm>(this TForm form, Control control) where TForm : Form
    { form.Content = control; return form; }

    public static TForm OnClosing<TForm>(this TForm form, EventHandler<CancelEventArgs> handler) where TForm : Form
    { form.FormClosing += handler; return form; }

    public static TForm OnClosed<TForm>(this TForm form, EventHandler handler) where TForm : Form
    { form.FormClosed += handler; return form; }
}
```

### 5.7 两种风格对比

```csharp
// 属性式（适合 WinForms 用户）
var button = new Button();
button.Text = "Save";
button.Width = 120;
button.Height = 36;
button.Click += (_, _) => Save();

// 流式（适合现代 C# 用户）
var button = new Button("Save")
    .Size(120, 36)
    .Margin(8)
    .OnClick(Save);
```

---

## 六、事件模型设计

### 6.1 双模式支持

RS.AvaForms 同时支持传统事件和流式事件：

```csharp
// 传统事件模式（WinForms 风格）
button.Click += (_, _) => Save();

// 流式事件模式
button.OnClick(Save);
button.OnClick(async () => await SaveAsync());
button.OnClick(btn => { btn.IsEnabled = false; Save(); });
```

### 6.2 事件封装三种重载

```csharp
// 同步无参
public static Button OnClick(this Button button, Action action);

// 异步无参
public static Button OnClick(this Button button, Func<Task> action);

// 同步带参数
public static Button OnClick(this Button button, Action<Button> action);
```

这样既支持同步方法，也支持异步方法。

### 6.3 异步事件异常处理

对于异步事件，必须统一捕获异常，并提供全局异常处理器：

```csharp
AvaFormsApp.OnUnhandledException = ex =>
{
    Logger.LogError("未处理的异常", ex);
    MessageBox.Show($"发生错误: {ex.Message}", "错误");
};
```

这对桌面应用很重要，否则用户点击按钮后出现未观察异常，会很难调试。

### 6.4 Form 生命周期事件

```csharp
public class MainForm : Form
{
    public MainForm()
    {
        FormClosing += (_, e) =>
        {
            if (HasUnsavedChanges) e.Cancel = true;
        };
        FormClosed += (_, _) => Cleanup();
    }
}
```

---

## 七、数据绑定与 MVVM

虽然项目是 WinForms-like，但不建议只做事件驱动。Avalonia 的强项之一是数据绑定和 MVVM，RS.AvaForms 应该提供更简单的代码式绑定 API，而不是让用户必须写复杂绑定表达式。

### 7.1 简单绑定

```csharp
// 属性名绑定
new TextBox().BindText(nameof(ViewModel.UserName));

// Lambda 绑定（类型安全）
new TextBox().BindText<LoginViewModel>(x => x.UserName);
```

### 7.2 命令绑定

```csharp
new Button("Login").BindCommand(nameof(ViewModel.LoginCommand));
new Button("Login").BindCommand<LoginViewModel>(x => x.LoginCommand);
```

### 7.3 绑定扩展方法

```csharp
namespace RS.AvaForms.Binding;

public static class BindingExtensions
{
    public static TextBox BindText(this TextBox textBox, string propertyName)
    {
        textBox.Bind(global::Avalonia.Controls.TextBox.TextProperty,
            new global::Avalonia.Data.Binding(propertyName));
        return textBox;
    }

    public static TextBox BindText<TViewModel>(this TextBox textBox,
        Expression<Func<TViewModel, object?>> property)
    {
        return textBox.BindText(GetPropertyName(property));
    }

    public static Button BindCommand(this Button button, string propertyName)
    {
        button.Bind(global::Avalonia.Controls.Button.CommandProperty,
            new global::Avalonia.Data.Binding(propertyName));
        return button;
    }

    public static Button BindCommand<TViewModel>(this Button button,
        Expression<Func<TViewModel, object?>> property)
    {
        return button.BindCommand(GetPropertyName(property));
    }

    public static CheckBox BindChecked(this CheckBox checkBox, string propertyName)
    {
        checkBox.Bind(global::Avalonia.Controls.CheckBox.IsCheckedProperty,
            new global::Avalonia.Data.Binding(propertyName));
        return checkBox;
    }

    public static CheckBox BindChecked<TViewModel>(this CheckBox checkBox,
        Expression<Func<TViewModel, object?>> property)
    {
        return checkBox.BindChecked(GetPropertyName(property));
    }

    private static string GetPropertyName<T>(Expression<Func<T, object?>> expr) => expr.Body switch
    {
        MemberExpression m => m.Member.Name,
        UnaryExpression { Operand: MemberExpression m } => m.Member.Name,
        _ => throw new ArgumentException("不支持的表达式类型")
    };
}
```

### 7.4 泛型 Form\<TViewModel\>

```csharp
public class LoginForm : Form<LoginViewModel>
{
    public LoginForm()
    {
        Text = "登录";
        SetSize(420, 280);
        CenterScreen();

        Content = VStack.Create()
            .Padding(24)
            .Spacing(12)
            .Add(new Label("账号"))
            .Add(new TextBox().Placeholder("请输入账号").BindText(vm => vm.Account))
            .Add(new Label("密码"))
            .Add(new TextBox().Password().Placeholder("请输入密码").BindText(vm => vm.Password))
            .Add(HStack.Create()
                .Spacing(8)
                .AlignRight()
                .Add(new Button("取消").Secondary().OnClick(Close))
                .Add(new Button("登录").Primary().BindCommand(vm => vm.LoginCommand)));
    }
}
```

### 7.5 CommunityToolkit.Mvvm 兼容

用户可以结合 CommunityToolkit.Mvvm 使用：

```csharp
public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string userName = "";

    [RelayCommand]
    private void Login() { }
}
```

然后 UI：

```csharp
public class LoginForm : Form<LoginViewModel>
{
    public LoginForm()
    {
        Content = VStack.Create()
            .Add(new TextBox().BindText(vm => vm.UserName))
            .Add(new Button("Login").BindCommand(vm => vm.LoginCommand));
    }
}
```

这会让 RS.AvaForms 同时服务两类用户：喜欢 WinForms 事件模型的人，以及喜欢 MVVM 的人。

---

## 八、样式与主题

### 8.1 设计原则

不要把样式硬编码在控件里。RS.AvaForms 应该提供默认主题，但底层仍然走 Avalonia 的样式系统。Avalonia 支持主题、样式类、Light/Dark 主题，以及 Fluent 和 Simple 主题。`.Primary()`、`.Danger()`、`.Large()` 这类方法设计为给控件添加样式类，而不是直接改一堆颜色。这样用户以后可以通过主题统一修改外观。

### 8.2 样式变体（通过 Classes）

```csharp
new Button("Save").Primary();      // → button.Classes.Add("primary")
new Button("Cancel").Secondary();  // → button.Classes.Add("secondary")
new Button("Delete").Danger();     // → button.Classes.Add("danger")
new Button("Apply").Success();     // → button.Classes.Add("success")
new Button("Tool").Ghost();        // → button.Classes.Add("ghost")
new Button("Save").Large();        // → button.Classes.Add("large")
new Button("Save").Small();        // → button.Classes.Add("small")
```

### 8.3 样式变体定义

| 变体 | 用途 | 视觉效果 |
|------|------|---------|
| Primary | 主操作按钮（保存、登录、确认） | 强调色背景、白色文字 |
| Secondary | 次要操作（取消、返回） | 边框、默认文字色 |
| Danger | 危险操作（删除、清空） | 红色背景、白色文字 |
| Success | 成功操作（完成、应用） | 绿色背景、白色文字 |
| Ghost | 弱化按钮（工具栏按钮） | 无背景无边框、悬停显示 |
| Large | 大尺寸 | 增大内边距和字号 |
| Small | 小尺寸 | 减小内边距和字号 |

### 8.4 主题切换

```csharp
AvaFormsApp.Create()
    .UseTheme(AvaFormsTheme.Dark)
    .Build()
    .Run<MainForm>(args);
```

### 8.5 主题枚举

```csharp
namespace RS.AvaForms.Themes;

public enum AvaFormsTheme
{
    Light,
    Dark,
    Fluent,
    Modern
}
```

### 8.6 主题包

| 主题包 | 说明 | 状态 |
|--------|------|------|
| `AvaFormsTheme.Light` | 浅色主题（默认） | Phase 2 |
| `AvaFormsTheme.Dark` | 深色主题 | Phase 2 |
| `AvaFormsTheme.Fluent` | Fluent Design 风格 | Phase 2 |
| `AvaFormsTheme.Modern` | 现代简约风格 | 后期 |

### 8.7 默认样式文件

样式通过 Avalonia 的 axaml 定义，嵌入到核心库中：

```xml
<!-- Styles/ButtonStyles.axaml -->
<Styles xmlns="https://github.com/avaloniaui">
    <Style Selector="Button.primary">
        <Setter Property="Background" Value="{DynamicResource SystemAccentColor}"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="CornerRadius" Value="4"/>
    </Style>
    <Style Selector="Button.danger">
        <Setter Property="Background" Value="#DC3545"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="CornerRadius" Value="4"/>
    </Style>
    <Style Selector="Button.secondary">
        <Setter Property="Background" Value="Transparent"/>
        <Setter Property="BorderBrush" Value="{DynamicResource SystemBaseMediumLowColor}"/>
        <Setter Property="BorderThickness" Value="1"/>
        <Setter Property="CornerRadius" Value="4"/>
    </Style>
</Styles>
```

---

## 九、应用启动

### 9.1 最简启动

```csharp
AvaFormsApp.Run<MainForm>(args);
```

### 9.2 Builder 模式

```csharp
var app = AvaFormsApp.Create()
    .UseTheme(AvaFormsTheme.Dark)
    .UseLogger(new FileLogger("app.log"), LogLevel.Debug)
    .Build();

app.Run<MainForm>(args);
```

### 9.3 带 DI 的启动

```csharp
var app = AvaFormsApp.Create()
    .UseTheme(AvaFormsTheme.Fluent)
    .UseLogger(new FileLogger("app.log"), LogLevel.Information)
    .ConfigureServices(services =>
    {
        services.AddSingleton<MainViewModel>();
        services.AddTransient<MainForm>();
    })
    .Build();

app.Run<MainForm>(args);
```

### 9.4 AvaFormsApp 设计

```csharp
namespace RS.AvaForms;

public static class AvaFormsApp
{
    public static Action<Exception>? OnUnhandledException { get; set; }

    public static void Run<TForm>(string[] args) where TForm : Form, new()
    {
        Create().Build().Run<TForm>(args);
    }

    public static AvaFormsAppBuilder Create() => new();
}
```

### 9.5 AvaFormsAppBuilder 设计

```csharp
namespace RS.AvaForms;

public sealed class AvaFormsAppBuilder
{
    private AvaFormsTheme _theme = AvaFormsTheme.Light;
    private IDiagnosticLogger? _logger;
    private LogLevel _minimumLogLevel = LogLevel.Information;
    private Action<IServiceCollection>? _configureServices;

    public AvaFormsAppBuilder UseTheme(AvaFormsTheme theme)
    { _theme = theme; return this; }

    public AvaFormsAppBuilder UseLogger(IDiagnosticLogger logger, LogLevel minimumLevel)
    { _logger = logger; _minimumLogLevel = minimumLevel; return this; }

    public AvaFormsAppBuilder EnableVerboseLogging()
    { _minimumLogLevel = LogLevel.Trace; return this; }

    public AvaFormsAppBuilder ConfigureServices(Action<IServiceCollection> configure)
    { _configureServices = configure; return this; }

    public IAvaFormsApp Build()
    {
        // Build() 内部执行顺序固定，不受 UseXXX() 调用顺序影响：
        // 1. 创建 IDiagnosticLogger
        var logger = _logger ?? new DiagnosticLogger { MinimumLevel = _minimumLogLevel };
        if (_logger is not null)
            _logger.MinimumLevel = _minimumLogLevel;

        // 2. 配置 IServiceCollection 并构建 ServiceProvider
        var services = new ServiceCollection();
        _configureServices?.Invoke(services);
        var serviceProvider = services.BuildServiceProvider();

        // 3. 创建 IAvaFormsApp 实例
        return new AvaFormsApplication(_theme, logger, serviceProvider);
    }
}
```

### 9.6 IAvaFormsApp 接口

```csharp
namespace RS.AvaForms;

public interface IAvaFormsApp
{
    void Run<TForm>(string[] args) where TForm : Form, new();
    void Shutdown(int exitCode = 0);
}
```

### 9.7 AvaFormsApplication 内部实现

```csharp
namespace RS.AvaForms;

internal sealed class AvaFormsApplication : IAvaFormsApp
{
    private readonly AvaFormsTheme _theme;
    private readonly IDiagnosticLogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public AvaFormsApplication(
        AvaFormsTheme theme,
        IDiagnosticLogger logger,
        IServiceProvider serviceProvider)
    {
        _theme = theme;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public void Run<TForm>(string[] args) where TForm : Form, new()
    {
        _logger.LogInformation("RS.AvaForms 应用启动");

        var theme = _theme;
        var logger = _logger;
        var serviceProvider = _serviceProvider;

        BuildAvaloniaApp(theme, logger, serviceProvider, typeof(TForm))
            .StartWithClassicDesktopLifetime(args);
    }

    public void Shutdown(int exitCode = 0)
    {
        _logger.LogInformation($"RS.AvaForms 应用退出，退出码: {exitCode}");
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown(exitCode);
    }

    private static AppBuilder BuildAvaloniaApp(
        AvaFormsTheme theme,
        IDiagnosticLogger logger,
        IServiceProvider serviceProvider,
        Type formType)
    {
        return AppBuilder.Configure(() => new InternalAvaloniaApp(theme, logger, serviceProvider, formType))
            .UsePlatformDetect()
            .WithInterFont()
            .With(new FluentTheme());
    }
}
```

### 9.8 InternalAvaloniaApp 设计

```csharp
namespace RS.AvaForms;

internal sealed class InternalAvaloniaApp : global::Avalonia.Application
{
    private readonly AvaFormsTheme _theme;
    private readonly IDiagnosticLogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Type _formType;

    public InternalAvaloniaApp(
        AvaFormsTheme theme,
        IDiagnosticLogger logger,
        IServiceProvider serviceProvider,
        Type formType)
    {
        _theme = theme;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _formType = formType;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                _logger.LogInformation("开始创建主窗体");

                var form = (Form?)_serviceProvider.GetService(_formType)
                    ?? Activator.CreateInstance(_formType) as Form;

                if (form is null)
                    throw new InvalidOperationException($"无法创建主窗体，类型: {_formType.Name}");

                desktop.MainWindow = form;

                if (_theme == AvaFormsTheme.Dark)
                    RequestedThemeVariant = global::Avalonia.Styling.ThemeVariant.Dark;

                _logger.LogInformation($"主窗体创建完成: Title={form.Title}");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("主窗体创建失败", ex);

                desktop.MainWindow = new Window
                {
                    Title = "RS.AvaForms - 启动错误",
                    Width = 600,
                    Height = 400,
                    Content = new TextBlock
                    {
                        Text = $"启动失败:\n\n{ex.Message}\n\n{ex.GetType().Name}\n{ex.StackTrace}",
                        Margin = new Thickness(16),
                        Foreground = Brushes.Red,
                        TextWrapping = TextWrapping.Wrap
                    }
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
```

### 9.9 Build() 内部顺序保证

`Build()` 的内部执行顺序固定，不受 `UseXXX()` 调用顺序影响：

1. 创建 `IDiagnosticLogger`（未提供则使用默认 `DiagnosticLogger`）
2. 配置 `IServiceCollection` 并构建 `ServiceProvider`
3. 创建 `IAvaFormsApp` 实例

`Run<TForm>()` 内部：

1. 构建 Avalonia `AppBuilder`
2. 在 `OnFrameworkInitializationCompleted` 中创建 `TForm` 实例（优先从 DI 容器获取，否则 `Activator.CreateInstance`）
3. 设置 `desktop.MainWindow`
4. 根据主题设置 `RequestedThemeVariant`

---

## 十、诊断日志

### 10.1 日志级别

```csharp
namespace RS.AvaForms.Diagnostics;

public enum LogLevel
{
    Trace,
    Debug,
    Information,
    Warning,
    Error,
    Critical,
    None
}
```

### 10.2 日志接口

```csharp
namespace RS.AvaForms.Diagnostics;

public interface IDiagnosticLogger
{
    LogLevel MinimumLevel { get; set; }
    bool IsEnabled(LogLevel level);
    void Log(LogLevel level, string message, Exception? exception = null);

    void LogTrace(string message, Exception? ex = null) => Log(LogLevel.Trace, message, ex);
    void LogDebug(string message, Exception? ex = null) => Log(LogLevel.Debug, message, ex);
    void LogInformation(string message, Exception? ex = null) => Log(LogLevel.Information, message, ex);
    void LogWarning(string message, Exception? ex = null) => Log(LogLevel.Warning, message, ex);
    void LogError(string message, Exception? ex = null) => Log(LogLevel.Error, message, ex);
    void LogCritical(string message, Exception? ex = null) => Log(LogLevel.Critical, message, ex);
}
```

### 10.3 内置实现

| 实现 | 说明 |
|------|------|
| `DiagnosticLogger` | 输出到 `Debug.WriteLine` + `Console.Error`，带时间戳和级别标记 |
| `NullLogger` | 静默模式，所有日志丢弃 |
| `FileLogger` | 输出到文件，用户指定文件路径 |

### 10.4 DiagnosticLogger 默认实现

```csharp
namespace RS.AvaForms.Diagnostics;

public class DiagnosticLogger : IDiagnosticLogger
{
    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

    public bool IsEnabled(LogLevel level) => level >= MinimumLevel;

    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        if (!IsEnabled(level)) return;

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var levelStr = level.ToString().ToUpper();
        var logMessage = $"[{timestamp}] [{levelStr}] {message}";

        if (exception is not null)
            logMessage += $"\n  Exception: {exception.GetType().Name}: {exception.Message}\n  {exception.StackTrace}";

        System.Diagnostics.Debug.WriteLine(logMessage);
        Console.Error.WriteLine(logMessage);
    }
}
```

### 10.5 FileLogger 实现

```csharp
namespace RS.AvaForms.Diagnostics;

public class FileLogger : IDiagnosticLogger
{
    private readonly string _filePath;
    private readonly object _lock = new();

    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

    public FileLogger(string filePath) { _filePath = filePath; }

    public bool IsEnabled(LogLevel level) => level >= MinimumLevel;

    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        if (!IsEnabled(level)) return;

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var levelStr = level.ToString().ToUpper();
        var logMessage = $"[{timestamp}] [{levelStr}] {message}";

        if (exception is not null)
            logMessage += $"\n  Exception: {exception.GetType().Name}: {exception.Message}\n  {exception.StackTrace}";

        lock (_lock)
        {
            File.AppendAllText(_filePath, logMessage + Environment.NewLine);
        }
    }
}
```

---

## 十一、Generator 层

### 11.1 JSON 中间描述

Generator 支持 JSON 描述 → C# 代码生成，方便设计器或 AI 工具生成界面：

```json
{
    "form": {
        "type": "Form",
        "name": "MainForm",
        "properties": {
            "text": "我的应用",
            "size": { "width": 800, "height": 600 }
        },
        "controls": [
            {
                "type": "DockLayout",
                "name": "dock",
                "children": [
                    {
                        "type": "Button",
                        "name": "okButton",
                        "properties": { "text": "确定" },
                        "dock": "Bottom",
                        "events": { "click": "OnOkClick" }
                    },
                    {
                        "type": "TextBox",
                        "name": "nameBox",
                        "properties": { "placeholder": "请输入" },
                        "dock": "Fill"
                    }
                ]
            }
        ]
    }
}
```

### 11.2 DTO 定义（record 类型）

```csharp
namespace RS.AvaForms.Generator.Models;

public record FormDefinition(
    string Type,
    string Name,
    Dictionary<string, object?> Properties,
    List<ControlDefinition> Controls);

public record ControlDefinition(
    string Type,
    string Name,
    Dictionary<string, object?>? Properties,
    List<ControlDefinition>? Children,
    string? Dock,
    Dictionary<string, string>? Events);
```

### 11.3 生成的 C# 代码

```csharp
public partial class MainForm : Form
{
    private DockLayout dock = null!;
    private Button okButton = null!;
    private TextBox nameBox = null!;

    private void InitializeComponent()
    {
        this.Text = "我的应用";
        this.Width = 800;
        this.Height = 600;

        dock = new DockLayout();
        okButton = new Button("确定") { Name = "okButton" };
        okButton.Click += OnOkClick;
        DockPanel.SetDock(okButton, global::Avalonia.Controls.Dock.Bottom);

        nameBox = new TextBox { Name = "nameBox", Watermark = "请输入" };

        dock.Add(okButton, global::Avalonia.Controls.Dock.Bottom);
        dock.Add(nameBox);

        this.Content = dock;
    }
}
```

---

## 十二、编码规范

### 12.1 C# 规范

| 规则 | 说明 |
|------|------|
| 可空引用类型 | 启用，警告视为错误 |
| C# 版本 | 12.0+ |
| 文件作用域命名空间 | 所有文件使用 `namespace RS.AvaForms;` |
| DTO/事件/值类型 | 使用 `record` |
| 实体和行为 | 使用 `class` |
| 异步编程 | 仅 `async/await`，禁止 `.Result`/`.Wait()`/`async void`（事件处理器除外） |
| 每个 async I/O 方法 | 接受并传播 `CancellationToken` |
| 命名 | PascalCase（方法/类）、_camelCase（私有字段） |
| switch 表达式 | 优先使用 `switch` 表达式替代 if/else 链 |
| 捕获异常 | 捕获具体异常；使用 `throw;` 重新抛出；允许 `OperationCanceledException` 冒泡 |

### 12.2 架构规范

| 规则 | 说明 |
|------|------|
| 直接继承 Avalonia 控件 | 不包装、不适配，直接继承 |
| 流式 API 用扩展方法 | 不在控件类中堆砌流式方法 |
| 样式走 Avalonia Classes | 不硬编码颜色，用 `.Primary()` 等添加样式类 |
| 日志走 IDiagnosticLogger | 不用 `Console.WriteLine` |
| 用户代码不强制引用 Avalonia | 默认 `using RS.AvaForms` 即可，但需要时可访问底层 |
| DI 通过构造函数注入 | 禁用静态状态和服务定位器 |
| 可复用库的 await 追加 ConfigureAwait(false) | 防止死锁 |

### 12.3 测试规范

| 规则 | 说明 |
|------|------|
| 测试框架 | xUnit + FluentAssertions |
| 覆盖范围 | 覆盖公共接口和边界情况 |
| null 输入 | 测试 null 输入、极限值、异常场景 |

---

## 十三、开发路线

### Phase 1：核心体验

- [ ] Form、Button、Label、TextBox、CheckBox、RadioButton
- [ ] VStack、HStack、DockLayout
- [ ] AvaFormsApp 启动（Builder 模式）
- [ ] 流式扩展方法（OnClick、Primary、Margin、Size）
- [ ] MessageBox 封装
- [ ] IDiagnosticLogger + DiagnosticLogger + NullLogger
- [ ] HelloWorld 示例

**目标**：用户能写一个完整窗口。

### Phase 2：样式与主题

- [ ] Primary / Secondary / Danger / Success / Ghost 样式变体
- [ ] Light / Dark 主题
- [ ] 默认间距、字体、控件尺寸
- [ ] Fluent 主题包
- [ ] FileLogger 实现
- [ ] LoginDemo 示例

**目标**：界面默认就好看，而不是像裸控件。

### Phase 3：MVVM 与绑定

- [ ] BindText / BindChecked / BindItems / BindSelectedItem / BindCommand
- [ ] Form\<TViewModel\> 泛型窗体
- [ ] CommunityToolkit.Mvvm 兼容
- [ ] SettingsDemo 示例

**目标**：中大型项目也能用。

### Phase 4：完整控件库

- [ ] ComboBox、ListBox、TreeView、DataGridView
- [ ] TabControl、GroupBox
- [ ] ProgressBar、TrackBar
- [ ] PictureBox、ToolBar、StatusBar
- [ ] GridForm、AbsoluteLayout
- [ ] Menu

**目标**：覆盖常见桌面应用需求。

### Phase 5：Generator

- [ ] JSON → C# 代码生成
- [ ] 嵌套控件支持
- [ ] 字符串转义
- [ ] Generator 测试

**目标**：支持设计器和 AI 工具生成界面。

### Phase 6：项目模板与文档

- [ ] `dotnet new avaforms` 模板
- [ ] `dotnet new avaforms-mvvm` 模板
- [ ] CRUD 工具示例
- [ ] API 文档

**目标**：降低新用户上手成本。

---

## 十四、API 体验目标

### 14.1 最小应用

```csharp
using RS.AvaForms;
using RS.AvaForms.Controls;
using RS.AvaForms.Layouts;

public class MainForm : Form
{
    public MainForm()
    {
        Text = "Hello AvaForms";
        SetSize(600, 400);
        CenterScreen();

        Content = VStack.Create()
            .Padding(20)
            .Spacing(12)
            .Add(new Label("Hello"))
            .Add(new Button("Click me").OnClick(() =>
            {
                MessageBox.Show("Hello from AvaForms");
            }));
    }
}

// Program.cs
AvaFormsApp.Run<MainForm>(args);
```

### 14.2 登录窗口

```csharp
public class LoginForm : Form
{
    public LoginForm()
    {
        Text = "登录";
        SetSize(420, 280);
        CenterScreen();

        Content = VStack.Create()
            .Padding(24)
            .Spacing(12)
            .Add(new Label("账号"))
            .Add(new TextBox().Placeholder("请输入账号"))
            .Add(new Label("密码"))
            .Add(new TextBox().Password().Placeholder("请输入密码"))
            .Add(HStack.Create()
                .Spacing(8)
                .AlignRight()
                .Add(new Button("取消").Secondary())
                .Add(new Button("登录").Primary()));
    }
}
```

### 14.3 主窗口（Dock 布局）

```csharp
public class MainForm : Form
{
    public MainForm()
    {
        Text = "我的应用";
        SetSize(900, 600);
        CenterScreen();

        Content = DockLayout.Create()
            .LastChildFill(true)
            .Add(new Border { Height = 32, Background = Brushes.DarkGray }, Dock.Top)
            .Add(new Border { Height = 24, Background = Brushes.LightGray }, Dock.Bottom)
            .Add(new Border { Width = 200, Background = Brushes.WhiteSmoke }, Dock.Left)
            .Add(new TextBox { AcceptsReturn = true });
    }
}
```

### 14.4 MVVM 登录窗口

```csharp
public class LoginForm : Form<LoginViewModel>
{
    public LoginForm()
    {
        Text = "登录";
        SetSize(420, 280);
        CenterScreen();

        Content = VStack.Create()
            .Padding(24)
            .Spacing(12)
            .Add(new Label("账号"))
            .Add(new TextBox().Placeholder("请输入账号").BindText(vm => vm.Account))
            .Add(new Label("密码"))
            .Add(new TextBox().Password().Placeholder("请输入密码").BindText(vm => vm.Password))
            .Add(HStack.Create()
                .Spacing(8)
                .AlignRight()
                .Add(new Button("取消").Secondary().OnClick(Close))
                .Add(new Button("登录").Primary().BindCommand(vm => vm.LoginCommand)));
    }
}
```

### 14.5 带 DI 和日志的启动

```csharp
internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var app = AvaFormsApp.Create()
            .UseTheme(AvaFormsTheme.Dark)
            .UseLogger(new FileLogger("app.log"), LogLevel.Debug)
            .ConfigureServices(services =>
            {
                services.AddSingleton<MainViewModel>();
            })
            .Build();

        app.Run<MainForm>(args);
    }
}
```

### 14.6 最终追求的代码风格

```csharp
public class LoginForm : Form<LoginViewModel>
{
    public LoginForm()
    {
        Text = "登录";
        SetSize(420, 280);
        CenterScreen();

        Content = VStack.Create()
            .Padding(24)
            .Spacing(12)
            .Add(new Label("账号"))
            .Add(new TextBox()
                .Placeholder("请输入账号")
                .BindText(vm => vm.Account))
            .Add(new Label("密码"))
            .Add(new TextBox()
                .Password()
                .Placeholder("请输入密码")
                .BindText(vm => vm.Password))
            .Add(HStack.Create()
                .Spacing(8)
                .AlignRight()
                .Add(new Button("取消")
                    .Secondary()
                    .OnClick(Close))
                .Add(new Button("登录")
                    .Primary()
                    .BindCommand(vm => vm.LoginCommand)));
    }
}
```

这段代码的理想效果是：WinForms 用户觉得它亲切，Avalonia 用户觉得它没有破坏框架能力，现代 C# 用户觉得它简洁。

---

## 十五、WinForms 概念映射

| WinForms 概念 | RS.AvaForms API | Avalonia 底层 |
|---------------|-----------------|---------------|
| Form | `Form` | `Window` |
| Control | `Control` / 控件子类 | `Avalonia.Controls.Control` |
| Text | `Text`（统一属性） | `Content` / `Text` / `Title` |
| Click | `OnClick()` / `Click` | Routed event |
| Dock | `DockLayout.Add(control, Dock.X)` | `DockPanel` |
| Anchor | `AbsoluteLayout`（可选） | `Canvas` |
| Padding | `.Padding()` | `Margin`（布局容器） |
| Margin | `.Margin()` | `Margin` |
| Location | `AbsoluteLayout.Add(x, y)` | `Canvas.SetLeft/SetTop` |
| Size | `.Size(w, h)` / `.Width()` / `.Height()` | `Width` / `Height` |
| Show() | `ShowForm()` | `Show()` |
| ShowDialog() | `ShowDialogForm()` | `ShowDialog()` |
| MessageBox | `MessageBox.Show()` | 自定义弹窗 |

**注意**：不要盲目复刻 Anchor、Dock、Location、Size 这些 WinForms 旧布局思维。Avalonia 的布局系统更接近 WPF/XAML 体系，支持面板、约束、样式和数据绑定。RS.AvaForms 提供 WinForms-like 的入口，但底层鼓励更现代的布局方式。

---

## 十六、项目定位文案

**英文**：

> RS.AvaForms is a code-only UI framework built on top of Avalonia, designed to provide a WinForms-like development experience for modern cross-platform desktop applications.

**中文**：

> RS.AvaForms 是一个构建在 Avalonia 之上的 Code-only UI 框架，旨在为现代跨平台桌面应用提供类似 WinForms 的简单开发体验。

**一句话**：

> 用 WinForms 的方式写 Avalonia。
