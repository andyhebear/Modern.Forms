# Modern.Forms 更新日志

本文档记录 Modern.Forms 项目自初始版本以来的所有重要更新内容。

---

## [Unreleased] - 当前开发版

基于 v0.3 之后的持续开发，目标框架升级至 .NET 8，新增多个控件和重要功能。

### 新增

- **DataGridView 控件** — 从 WinForms 移植完整的数据网格视图控件，支持 Cell/Column/Row 集合 (#121)
- **LinkLabel 控件** — 超链接标签控件 (#120)
- **TrackBar 控件** — 滑块/跟踪条控件，含渲染器和演示面板 (#119)
- **Timer 组件** — 计时器组件 (#113)
- **Modern.Forms-Templates** — dotnet new 项目模板，支持 `dotnet new modernforms` 快速创建项目
- **Dock Layout Test 示例** — Dock 布局功能测试项目
- **Modern 和 Metro 主题色** — 新增 Modern 和 Metro 主题颜色方案
- **标题栏恢复按钮** — 添加还原图标及还原窗口状态支持 (#114)
- **AutoSizeMode** — 多个控件新增 AutoSizeMode 属性
- **AutoEllipsis** — 多个控件新增 AutoEllipsis 支持
- **ImageList** — 新增 ImageList 组件，CheckBox、RadioButton、Label 支持 ImageList (#120 相关)
- **Button 图片支持** — Button 控件新增图片显示功能
- **CheckBox/RadioButton 图片支持** — 复选框和单选按钮新增图片支持
- **Application.OpenForms** — 新增打开窗体集合属性 (#49)
- **WindowBase 键盘事件** — 窗口基类新增键盘处理事件 (#61)
- **Shift+方向键文本选择** — TextBox 支持通过 Shift+方向键选择文本
- **Theme 线程安全** — Theme 类改为线程安全，新增 TextSelectionBackgroundColor (#109)

### 变更

- **目标框架升级至 .NET 8** — 从 .NET 6 升级至 .NET 8 (2dbd162)
- **Theme 线程安全化** — Theme 静态类增加线程安全保证 (#109)
- **IRenderTextAndImage.ImageTextPadding 重命名** — 更名为 ImageTextMargin
- **ComboBox 下拉关闭时机** — 改为鼠标抬起时关闭下拉框（而非鼠标按下）
- **Remove ConfigureAwait(false)** — 从系统对话框类中移除 ConfigureAwait(false) (#118)
- **CI 触发器调整** — 工作流触发从 push 改为 pull_request，新增 workflow_dispatch (#68a6bc3, #6a4f311)
- **最新代码分析器** — 启用最新代码分析器并修复初始问题 (#108)
- **dotnet format CI** — CI 上强制代码格式化检查 (#107)

### 修复

- **PictureBox URL 截断** — 修复从 URL 加载图片时被截断的问题 (#115)
- **Label 多行显示** — 修复 Label Multiline 问题，添加 Multiline 到 IHaveTextAndImageAlign (#105)
- **CheckBox/RadioButton 焦点框** — 修复复选框和单选按钮的焦点矩形绘制
- **ThemeChanged 事件内存泄漏** — 避免 ThemeChanged 事件内存泄漏 (#20)
- **PopupWindow 自管理** — PopupWindow 应自行设置 ActivePopupWindow，而非调用方 (#71)
- **弹出窗口子控件点击** — 不应在点击弹出窗口的子控件时关闭弹出窗口 (#72)
- **ComboBox 弹出关闭** — 窗体移动时确保 ComboBox 弹出框关闭 (#66)
- **TabControl 索引器设置** — 修复通过集合索引器设置 TabPage 的问题 (#60)
- **ScrollableControl DisplayRectangle** — DisplayRectangle 应为未缩放值 (#67)，应包含 Padding (#44)
- **Paint 控件集合修改** — 绘制时复制控件集合，防止集合被修改导致异常 (#40)
- **Control PointToScreen** — 修复 DesktopScaling != Scaling 平台（Mac）的 PointToScreen (#d9c390f)
- **ScrollableControl 缩放舍入** — 修复缩放舍入问题 (#34)
- **Control GetScaledBounds 舍入** — 避免舍入误差 (#33)
- **Mac 窗口绘制时机** — 修复 Mac 在 Form 构造函数完成前请求绘制的问题
- **Linux FileDialog NRE** — 防止在 Linux 上点击取消时出现空引用异常
- **TreeView 递归深度** — Linux 目录可能很深，限制 TreeView 面板递归深度 (#27)
- **TextBox 退格键崩溃** — 修复使用退格键时崩溃的问题

---

## [0.3] - 2022-09-21

从 .NET Core 3.1 升级至 .NET 6，引入 WindowKit 抽象层，新增多个布局面板和控件。

### 新增

- **NavigationPane 控件** — 导航面板控件，类似 Outlook 导航栏
- **FlowLayoutPanel** — 流式布局面板 (#30)
- **TableLayoutPanel** — 表格布局面板 (#30)
- **Dark 主题** — 内置深色主题 (#62)
- **Outlook 克隆示例** — Outlaw 示例应用，展示复杂现代应用 (#36)
- **Form.ShowDialog** — 模态对话框及 DialogResult 支持
- **Form.Closing 事件** — 窗体关闭事件（可取消）
- **Form.OnPaint/OnPaintBackground** — 可重写的绘制方法
- **MinimumSize/MaximumSize** — 窗口最小/最大尺寸
- **Control.Invalidated 事件** — 控件无效化事件
- **ControlBehaviors.Transparent** — 透明行为标志
- **ControlBehaviors.ReceivesMouseEvents** — 鼠标事件接收行为标志
- **Control.IsMnemonic** — 助记键支持
- **TreeView.DrawMode** — 自定义树节点绘制模式
- **TabStrip Office 2021 样式** — 更新为 Office 2021 视觉风格
- **Skia 亚像素抗锯齿** — 使用 SubpixelAntialias 提高字体渲染质量
- **TextMeasurer 粗体支持** — 文本测量器支持粗体字体
- **圆角边框** — 启用圆角矩形边框
- **SourceLink 和确定性构建** — 支持 SourceLink 源码调试和确定性构建
- **IsTrimmable** — 支持 .NET 裁剪优化
- **Label 更多属性** — TextImageRelation 等属性

### 变更

- **目标框架升级至 .NET 6** — 从 .NET Core 3.1 / .NET 5 升级至 .NET 6
- **切换至 WindowKit NuGet** — 从直接引用 Avalonia 后端切换至 Modern.WindowKit NuGet 包
- **合并 Window 和 Form** — 简化窗口层次结构
- **PopupWindow 使用 PopupPositioner** — 弹出窗口使用定位器
- **弹出窗口必须指定父窗体** — Popups 必须拥有父 Form
- **FormTitleBar 隐式控件** — 标题栏改为隐式控件
- **ComboBox 键盘导航** — 添加键盘导航
- **ListBox 键盘导航** — 添加键盘导航和首字母选择
- **TabStrip 键盘导航** — 添加键盘导航
- **TreeView 键盘导航** — 添加键盘导航
- **Control 标签选择** — 移植 MS WinForms 标签选择代码修复 bug
- **PictureBox URL 异步加载** — 现代 framework 中从 URL 设置图片应为异步
- **VS2022** — 升级项目至 VS2022 格式
- **启用 NRT** — ControlGallery 启用 Nullable Reference Types

### 修复

- **Mac PointToScreen 缩放** — 修复 DesktopScaling != Scaling 的平台问题
- **Mac 窗口绘制时机** — 修复 Mac 在 Form 构造完成前请求绘制
- **TextBox 字体缩放** — 确保字体在 4K 下正确缩放
- **TextBox 退格键崩溃** — 修复退格键删除错误字符并最终崩溃
- **TextBox RichTextKit API** — 适配移除的 RichTextKit API
- **文本渲染崩溃** — TextRenderer 修复文本渲染崩溃
- **ScrollableControl 缩放舍入** — 修复缩放舍入问题 (#34)
- **Control GetScaledBounds 舍入** — 避免舍入误差 (#33)
- **Linux FileDialog NRE** — 防止取消时空引用异常
- **Linux TreeView 深度递归** — 限制递归深度 (#27)
- **Control Paint 集合修改** — 绘制时复制控件集合 (#40)
- **ScrollableControl DisplayRectangle** — 应包含 Padding (#44)
- **Window 鼠标移动按钮** — 在鼠标移动事件中填充按钮信息 (#19)
- **SetMinMaxSize** — 修复最小/最大尺寸设置

---

## [0.2] - 2020-05-12

主要增强控件属性、XML 文档和键盘输入支持。

### 新增

- **多字符键盘输入** — 支持多字符输入事件 (#16)
- **TextBox PasswordChar** — 密码字符属性
- **Ribbon MenuSeparatorItem** — Ribbon 支持菜单分隔符
- **XML 文档** — 大量添加 XML 文档注释，包含在 NuGet 包中
- **RenderManager 改进** — 渲染管理器变更

### 变更

- **RibbonItem 移除** — 使用 MenuItem 替代 RibbonItem
- **控件属性完善** — Button、CheckBox、Label、ComboBox、ListBox、FormTitleBar 等控件增加更多属性
- **第三方许可独立文件** — 移至 third-party-licenses.md
- **GitHub Actions** — 使用 GitHub Actions 替代 Azure DevOps

### 修复

- **TextBox 字体颜色和样式渲染** — 修复字体颜色和样式 (#16)
- **TextBox 退格键错误** — 修复退格键删除错误字母并崩溃 (#16)

---

## [0.1] - 2020-03-17

首个正式发布版本，包含基础控件集和跨平台支持。

### 新增

- **核心控件** — Button、CheckBox、RadioButton、Label、TextBox、ComboBox、ListBox、ListView、TreeView
- **容器控件** — Panel、SplitContainer、TabControl、TabStrip
- **高级控件** — Ribbon、ToolBar、StatusBar、ProgressBar、PictureBox、ScrollBar
- **菜单系统** — Menu、MenuItem、ContextMenu
- **对话框** — MessageBox、OpenFileDialog、SaveFileDialog、FolderBrowserDialog
- **布局系统** — Dock/Anchor 布局引擎（从 WinForms/Mono Winforms 移植）
- **主题系统** — 内置 Light 主题
- **渲染器架构** — 基于 SkiaSharp 的渲染器分离模式
- **跨平台支持** — Windows、macOS、Linux
- **HiDPI 支持** — 4K 和高 DPI 缩放
- **键盘导航** — 基础键盘导航支持
- **ControlGallery 示例** — 控件展示画廊
- **Explorer 示例** — Windows 资源管理器克隆
- **NuGet 包** — Modern.Forms NuGet 包发布

### 技术基础

- 目标框架：.NET Core 3.1
- 渲染引擎：SkiaSharp + HarfBuzzSharp + RichTextKit
- 窗口抽象：Avalonia 后端（Win32 / Cocoa / X11）
- 布局引擎：从 Microsoft WinForms 和 Mono Winforms 移植
- 许可证：MIT

---

## 版本对比总览

| 版本 | 发布日期 | 目标框架 | 主要里程碑 |
|------|----------|----------|------------|
| v0.1 | 2020-03-17 | .NET Core 3.1 | 首个发布版，基础控件集，跨平台支持 |
| v0.2 | 2020-05-12 | .NET Core 3.1 | 控件属性完善，XML 文档，键盘输入增强 |
| v0.3 | 2022-09-21 | .NET 6 | WindowKit 抽象层，布局面板，Dark 主题，模态对话框 |
| Unreleased | - | .NET 8 | DataGridView，TrackBar，LinkLabel，Timer，.NET 8 升级，线程安全主题 |
