using System.Drawing;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a TreeView.
    /// </summary>
    public class TreeViewRenderer : Renderer<TreeView>
    {
        protected const int INDENT_SIZE = 18;
        protected const int IMAGE_SIZE = 16;
        protected const int GLYPH_SIZE = 10;
        protected const int CHECKBOX_SIZE = 13;

        /// <inheritdoc/>
        protected override void Render (TreeView control, PaintEventArgs e)
        {
            e.Canvas.Save ();
            e.Canvas.Clip (control.ClientRectangle);

            var visible_item_count = control.ScaledHeight / control.ScaledItemHeight;

            foreach (var item in control.GetVisibleItems (true).Take (visible_item_count + 1))
                RenderItem (control, item, e);

            e.Canvas.Restore ();
        }

        /// <summary>
        /// Renders a TreeViewItem.
        /// </summary>
        protected virtual void RenderItem (TreeView control, TreeViewItem item, PaintEventArgs e)
        {
            var is_selected = item == control.SelectedItem;
            var foreground_color = control.Enabled ? Theme.ForegroundColor : Theme.ForegroundDisabledColor;

            if (is_selected)
                e.Canvas.FillRectangle (item.Bounds, control.Style.GetSelectedItemBackgroundColor ());

            if (is_selected && control.Focused && control.ShowFocusCues)
                e.Canvas.DrawFocusRectangle (item.Bounds, e.LogicalToDeviceUnits (1));

            if (control.DrawMode == TreeViewDrawMode.OwnerDrawContent) {
                var dea = new TreeViewDrawEventArgs (control, item, e);
                control.OnDrawNode (dea);

                if (!dea.DrawDefault)
                    return;
            }

            if (control.CheckBoxes) {
                var checkbox_bounds = GetCheckBoxBounds (control, item);
                ControlPaint.DrawCheckBox (e, checkbox_bounds, item.CheckState, !control.Enabled);
            }

            if (control.ShowDropdownGlyph == true) {
                var glyph_bounds = GetGlyphBounds (control, item);

                if (GetShouldDrawDropdownGlyph (control, item))
                    ControlPaint.DrawArrowGlyph (e, glyph_bounds, foreground_color, item.Expanded ? ArrowDirection.Down : ArrowDirection.Right);
            }

            if (control.ShowItemImages == true && item.Image != null) {
                var image_bounds = GetImageBounds (control, item, e);

                e.Canvas.DrawBitmap (item.Image!, image_bounds, !control.Enabled);
            }

            if (string.IsNullOrWhiteSpace (item.Text))
                return;

            var text_bounds = GetTextBounds (control, item, e);

            e.Canvas.DrawText (item.Text.Trim (), Theme.UIFont, e.LogicalToDeviceUnits (Theme.FontSize), text_bounds, foreground_color, ContentAlignment.MiddleLeft, maxLines: 1);
        }

        /// <summary>
        /// Gets the bounds of the dropdown glyph.
        /// </summary>
        public virtual Rectangle GetGlyphBounds (TreeView control, TreeViewItem item)
        {
            if (!control.ShowDropdownGlyph)
                return Rectangle.Empty;

            var indent_start = GetContentStart (control, item);
            var glyph_area = new Rectangle (indent_start, item.Bounds.Top, control.LogicalToDeviceUnits (GLYPH_SIZE), item.Bounds.Height);
            var glyph_bounds = DrawingExtensions.CenterSquare (glyph_area, control.LogicalToDeviceUnits (GLYPH_SIZE));

            glyph_bounds.Width = control.LogicalToDeviceUnits (GLYPH_SIZE);

            return glyph_bounds;
        }

        /// <summary>
        /// Gets the bounds of the checkbox.
        /// </summary>
        public virtual Rectangle GetCheckBoxBounds (TreeView control, TreeViewItem item)
        {
            if (!control.CheckBoxes)
                return Rectangle.Empty;

            var indent_start = GetIndentStart (control, item);
            var checkbox_size = control.LogicalToDeviceUnits (CHECKBOX_SIZE);
            var checkbox_area = new Rectangle (indent_start, item.Bounds.Top, checkbox_size, item.Bounds.Height);

            return DrawingExtensions.CenterSquare (checkbox_area, checkbox_size);
        }

        /// <summary>
        /// Gets the bounds of the item image.
        /// </summary>
        protected virtual Rectangle GetImageBounds (TreeView control, TreeViewItem item, PaintEventArgs e)
        {
            if (!control.ShowItemImages || item.Image is null)
                return Rectangle.Empty;

            var left_index = control.ShowDropdownGlyph ? GetGlyphBounds (control, item).Right : GetContentStart (control, item);
            var image_area = new Rectangle (left_index, item.Bounds.Top, item.Bounds.Height, item.Bounds.Height);

            return DrawingExtensions.CenterSquare (image_area, e.LogicalToDeviceUnits (IMAGE_SIZE));
        }

        protected virtual Rectangle GetTextBounds (TreeView control, TreeViewItem item, PaintEventArgs e)
        {
            var show_glyph = control.ShowDropdownGlyph;
            var show_image = control.ShowItemImages;

            if (!show_glyph && !show_image)
                return new Rectangle (GetContentStart (control, item), item.Bounds.Top, item.Bounds.Width - GetContentStart (control, item), item.Bounds.Height);

            var padding = e.LogicalToDeviceUnits (6);
            var used_bounds = show_image && item.Image is not null ? GetImageBounds (control, item, e) : GetGlyphBounds (control, item);
            return new Rectangle (used_bounds.Right + padding, item.Bounds.Top, item.Bounds.Right - used_bounds.Right - padding, item.Bounds.Height);
        }

        protected virtual int GetIndentStart (TreeView control, TreeViewItem item) => item.Bounds.Left + item.IndentLevel * control.LogicalToDeviceUnits (INDENT_SIZE) + 2;

        protected virtual int GetContentStart (TreeView control, TreeViewItem item)
        {
            var start = GetIndentStart (control, item);

            if (control.CheckBoxes)
                start += control.LogicalToDeviceUnits (CHECKBOX_SIZE) + control.LogicalToDeviceUnits (4);

            return start;
        }

        /// <summary>
        /// Gets if the item should draw a dropdown glyph.
        /// </summary>
        protected virtual bool GetShouldDrawDropdownGlyph (TreeView control, TreeViewItem item) => control.ShowDropdownGlyph && (item.HasChildren || (control.VirtualMode && item.items == null));
    }
}
