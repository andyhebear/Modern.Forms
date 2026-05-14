using System.Drawing;
using System.Text;
using Modern.Forms;

namespace DockLayoutTest
{
    public class DockTestForm : Form
    {
        private Menu? menu1;
        private ToolBar? toolBar1;
        private StatusBar? statusBar1;
        private ProgressBar? progressBar1;
        private Panel? panel1;

        protected override void InitializeComponent()
        {
            SuspendLayout();

            Size = new Size(800, 600);
            Text = "Dock Layout Test";
            UseForwardDockOrder = true;

            menu1 = new Menu();
            menu1.Name = "menu1";
            menu1.Dock = DockStyle.Top;
            menu1.TabIndex = 1;
            menu1.Style.Border.Color = SkiaSharp.SKColor.Parse("#1E90FF");
            menu1.Items.Add("File");
            menu1.Items.Add("Edit");
            menu1.Items.Add("Help");

            toolBar1 = new ToolBar();
            toolBar1.Name = "toolBar1";
            toolBar1.Dock = DockStyle.Top;
            toolBar1.TabIndex = 2;
            toolBar1.Style.BackgroundColor = SkiaSharp.SKColor.Parse("#FF1493");
            toolBar1.Style.Border.Width = 4;

            statusBar1 = new StatusBar();
            statusBar1.Name = "statusBar1";
            statusBar1.Text = "Ready";
            statusBar1.Dock = DockStyle.Bottom;
            statusBar1.TabIndex = 3;
            statusBar1.Style.ForegroundColor = SkiaSharp.SKColor.Parse("#4169E1");

            progressBar1 = new ProgressBar();
            progressBar1.Name = "progressBar1";
            progressBar1.Dock = DockStyle.Bottom;
            progressBar1.TabIndex = 4;
            progressBar1.Style.ForegroundColor = SkiaSharp.SKColor.Parse("#CD5C5C");
            progressBar1.Style.Border.Width = 2;
            progressBar1.Style.Border.Color = SkiaSharp.SKColor.Parse("#40E0D0");

            panel1 = new Panel();
            panel1.Name = "panel1";
            panel1.Dock = DockStyle.Fill;
            panel1.TabIndex = 5;
            panel1.TabStop = false;
            panel1.Style.BackgroundColor = SkiaSharp.SKColor.Parse("#FFD700");

            Controls.Add(menu1);
            Controls.Add(toolBar1);
            Controls.Add(statusBar1);
            Controls.Add(progressBar1);
            Controls.Add(panel1);

            ResumeLayout(false);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            PrintLayoutDiagnostics();

            var timer = new Modern.Forms.Timer();
            timer.Interval = 500;
            timer.Tick += (s, ea) =>
            {
                PrintLayoutDiagnostics();
                timer.Stop();
            };
            timer.Start();
        }

        private void PrintLayoutDiagnostics()
        {
            var sb = new StringBuilder();
            sb.AppendLine("========== Dock Layout Diagnostics ==========");
            sb.AppendLine($"Form Size: {Size.Width} x {Size.Height}");
            sb.AppendLine($"Form DisplayRectangle: X={DisplayRectangle.X}, Y={DisplayRectangle.Y}, W={DisplayRectangle.Width}, H={DisplayRectangle.Height}");
            sb.AppendLine($"UseForwardDockOrder: {UseForwardDockOrder}");
            sb.AppendLine();

            if (TitleBar is { Visible: true })
            {
                sb.AppendLine($"[TitleBar] Bounds: X={TitleBar.Bounds.X}, Y={TitleBar.Bounds.Y}, W={TitleBar.Bounds.Width}, H={TitleBar.Bounds.Height}");
                sb.AppendLine();
            }

            sb.AppendLine("--- Controls in Controls.Add order ---");
            for (var i = 0; i < Controls.Count; i++)
            {
                var ctrl = Controls[i];
                sb.AppendLine($"  [{i}] {ctrl.GetType().Name} (Name={ctrl.Name})");
                sb.AppendLine($"      Dock={ctrl.Dock}, Bounds: X={ctrl.Bounds.X}, Y={ctrl.Bounds.Y}, W={ctrl.Bounds.Width}, H={ctrl.Bounds.Height}");
                sb.AppendLine($"      ClientRectangle: X={ctrl.ClientRectangle.X}, Y={ctrl.ClientRectangle.Y}, W={ctrl.ClientRectangle.Width}, H={ctrl.ClientRectangle.Height}");
                sb.AppendLine($"      AutoSize={ctrl.AutoSize}, Visible={ctrl.Visible}");
            }

            sb.AppendLine();
            sb.AppendLine("--- Expected layout (AI-friendly) ---");
            sb.AppendLine("  TitleBar:    Y=0,  H=34  (always at top)");
            sb.AppendLine("  menu1:       Y=34, H=28  (Dock.Top, 1st added)");
            sb.AppendLine("  toolBar1:    Y=62, H=34  (Dock.Top, 2nd added)");
            sb.AppendLine("  statusBar1:  Y=Bottom-25, H=25  (Dock.Bottom, 1st added, closest to edge)");
            sb.AppendLine("  progressBar1: Y=statusBar1.Top-23, H=23  (Dock.Bottom, 2nd added, above statusBar)");
            sb.AppendLine("  panel1:      Fill remaining area");

            sb.AppendLine();
            sb.AppendLine("--- Actual layout verification ---");
            var errors = new List<string>();

            if (TitleBar is { Visible: true })
            {
                if (TitleBar.Bounds.Y != 0)
                    errors.Add($"TitleBar.Y={TitleBar.Bounds.Y}, expected 0");
                if (TitleBar.Bounds.Height != 34)
                    errors.Add($"TitleBar.Height={TitleBar.Bounds.Height}, expected 34");
            }

            var titleBarH = TitleBar is { Visible: true } ? TitleBar.Bounds.Height : 0;

            if (menu1 != null)
            {
                var expectedY = titleBarH;
                if (menu1.Bounds.Y != expectedY)
                    errors.Add($"menu1.Y={menu1.Bounds.Y}, expected {expectedY}");
                if (menu1.Bounds.Height != 28)
                    errors.Add($"menu1.Height={menu1.Bounds.Height}, expected 28");
            }

            if (toolBar1 != null)
            {
                var expectedY = titleBarH + 28;
                if (toolBar1.Bounds.Y != expectedY)
                    errors.Add($"toolBar1.Y={toolBar1.Bounds.Y}, expected {expectedY}");
                if (toolBar1.Bounds.Height != 34)
                    errors.Add($"toolBar1.Height={toolBar1.Bounds.Height}, expected 34");
            }

            if (statusBar1 != null && progressBar1 != null)
            {
                var totalUsedH = titleBarH + 28 + 34;
                var availableH = 598 - totalUsedH;
                var expectedStatusY = totalUsedH + availableH - 25;
                var expectedProgressY = expectedStatusY - 23;

                if (statusBar1.Bounds.Y != expectedStatusY)
                    errors.Add($"statusBar1.Y={statusBar1.Bounds.Y}, expected {expectedStatusY}");
                if (progressBar1.Bounds.Y != expectedProgressY)
                    errors.Add($"progressBar1.Y={progressBar1.Bounds.Y}, expected {expectedProgressY}");

                if (statusBar1.Bounds.Height != 25)
                    errors.Add($"statusBar1.Height={statusBar1.Bounds.Height}, expected 25");
                if (progressBar1.Bounds.Height != 23)
                    errors.Add($"progressBar1.Height={progressBar1.Bounds.Height}, expected 23");
            }

            if (panel1 != null)
            {
                var expectedPanelY = titleBarH + 28 + 34;
                var expectedPanelH = DisplayRectangle.Height - titleBarH - 28 - 34 - 25 - 23;
                if (panel1.Bounds.Y != expectedPanelY)
                    errors.Add($"panel1.Y={panel1.Bounds.Y}, expected {expectedPanelY}");
                if (panel1.Bounds.Height != expectedPanelH)
                    errors.Add($"panel1.Height={panel1.Bounds.Height}, expected {expectedPanelH}");
            }

            if (errors.Count == 0)
            {
                sb.AppendLine("  ALL CHECKS PASSED! Layout matches AI-friendly expectations.");
            }
            else
            {
                sb.AppendLine($"  FOUND {errors.Count} ISSUES:");
                foreach (var err in errors)
                    sb.AppendLine($"    - {err}");
            }

            sb.AppendLine();
            sb.AppendLine("==============================================");

            var output = sb.ToString();
            Console.WriteLine(output);

            var logPath = Path.Combine(Path.GetTempPath(), "DockLayoutTest.log");
            File.WriteAllText(logPath, output);
            Console.WriteLine($"Log written to: {logPath}");

            if (statusBar1 != null)
                statusBar1.Text = errors.Count == 0 ? "Layout OK" : $"{errors.Count} issues found";
        }
    }
}
