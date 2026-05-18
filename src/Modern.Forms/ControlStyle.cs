using SkiaSharp;

namespace Modern.Forms
{
    public class ControlStyle
    {
        internal readonly ControlStyle? _parent;

        public event EventHandler? Changed;

        public ControlStyle (ControlStyle? parent, Action<ControlStyle> setDefaults)
        {
            _parent = parent;

            Border = new BorderStyle (parent?.Border);
            Border.Changed += (s, e) => OnChanged ();

            setDefaults (this);

            Theme.ThemeChanged += (o, e) => { setDefaults (this); OnChanged (); };
        }

        public ControlStyle (ControlStyle parent)
        {
            _parent = parent;

            Border = new BorderStyle (parent?.Border);
            Border.Changed += (s, e) => OnChanged ();
        }

        SKColor? _backgroundColor;
        public SKColor? BackgroundColor {
            get => _backgroundColor;
            set { if (_backgroundColor != value) { _backgroundColor = value; OnChanged (); } }
        }

        public BorderStyle Border { get; }

        SKTypeface? _font;
        public SKTypeface? Font {
            get => _font;
            set { if (_font != value) { _font = value; OnChanged (); } }
        }

        int? _fontSize;
        public int? FontSize {
            get => _fontSize;
            set { if (_fontSize != value) { _fontSize = value; OnChanged (); } }
        }

        SKColor? _foregroundColor;
        public SKColor? ForegroundColor {
            get => _foregroundColor;
            set { if (_foregroundColor != value) { _foregroundColor = value; OnChanged (); } }
        }

        public SKColor GetBackgroundColor () => BackgroundColor ?? _parent?.GetBackgroundColor () ?? Theme.ControlMidColor;
        public SKTypeface GetFont () => Font ?? _parent?.GetFont () ?? Theme.UIFont;
        public int GetFontSize () => FontSize ?? _parent?.GetFontSize () ?? Theme.FontSize;
        public SKColor GetForegroundColor () => ForegroundColor ?? _parent?.GetForegroundColor () ?? Theme.ForegroundColor;

        protected virtual void OnChanged () => Changed?.Invoke (this, EventArgs.Empty);
    }
}
