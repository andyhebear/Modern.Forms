using SkiaSharp;

namespace Modern.Forms
{
    public class BorderStyle
    {
        private readonly BorderStyle? _parent;

        private SKColor? color;
        private int? width;

        public event EventHandler? Changed;

        public BorderStyle (BorderStyle? parent)
        {
            _parent = parent;

            Left = new BorderSideStyle (_parent?.Left);
            Top = new BorderSideStyle (_parent?.Top);
            Right = new BorderSideStyle (_parent?.Right);
            Bottom = new BorderSideStyle (_parent?.Bottom);

            Left.Changed += (s, e) => OnChanged ();
            Top.Changed += (s, e) => OnChanged ();
            Right.Changed += (s, e) => OnChanged ();
            Bottom.Changed += (s, e) => OnChanged ();
        }

        public BorderSideStyle Bottom { get; }

        public SKColor? Color {
            get => color;
            set {
                color = value;
                Left.Color = value;
                Right.Color = value;
                Top.Color = value;
                Bottom.Color = value;
            }
        }

        public SKColor GetColor () => Color ?? _parent?.GetColor () ?? SKColor.Empty;
        public int GetRadius () => Radius ?? _parent?.GetRadius () ?? 0;
        public int GetWidth () => Width ?? _parent?.GetWidth () ?? 0;

        public BorderSideStyle Left { get; }

        int? _radius;
        public int? Radius {
            get => _radius;
            set { if (_radius != value) { _radius = value; OnChanged (); } }
        }

        public BorderSideStyle Right { get; }

        public BorderSideStyle Top { get; }

        public int? Width {
            get => width;
            set {
                if (width != value) {
                    width = value;
                    Left.Width = value;
                    Right.Width = value;
                    Top.Width = value;
                    Bottom.Width = value;
                    OnChanged ();
                }
            }
        }

        protected virtual void OnChanged () => Changed?.Invoke (this, EventArgs.Empty);
    }

    public class BorderSideStyle
    {
        private readonly BorderSideStyle? _parent;

        public event EventHandler? Changed;

        public BorderSideStyle (BorderSideStyle? parent) => _parent = parent;

        SKColor? _color;
        public SKColor? Color {
            get => _color;
            set { if (_color != value) { _color = value; OnChanged (); } }
        }

        public SKColor GetColor () => Color ?? _parent?.GetColor () ?? Theme.BorderLowColor;

        int? _width;
        public int? Width {
            get => _width;
            set { if (_width != value) { _width = value; OnChanged (); } }
        }

        public int GetWidth () => Width ?? _parent?.GetWidth () ?? 0;

        protected virtual void OnChanged () => Changed?.Invoke (this, EventArgs.Empty);
    }
}
