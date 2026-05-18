using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Modern.Forms
{
    public class ListBoxItemCollection : ObservableCollection<object>
    {
        private readonly ListBox owner;
        private int focused_index;
        private int hovered_index = -1;

        internal ListBoxItemCollection (ListBox owner)
        {
            this.owner = owner;
        }

        public void AddRange (params object[] items)
        {
            owner.SuspendLayout ();

            foreach (var item in items)
                Add (item);

            owner.ResumeLayout (true);
        }

        internal void AddSelectedIndex (int index, bool single)
        {
            if (single)
                SelectedIndexes.Clear ();

            focused_index = Math.Max (index, 0);

            if (index != -1)
                SelectedIndexes.Add (index);

            owner.Invalidate ();
        }

        internal int FocusedIndex {
            get => focused_index;
            set {
                if (focused_index != value) {
                    focused_index = value;
                    owner.Invalidate ();
                }
            }
        }

        internal (int start, int end) GetSingleContiguousSelection ()
        {
            if (SelectedIndexes.Count == 0)
                return (-1, -1);

            if (SelectedIndexes.Count == 1)
                return (SelectedIndex, SelectedIndex);

            var min = SelectedIndexes[0];
            var max = SelectedIndexes[0];
            for (var i = 1; i < SelectedIndexes.Count; i++) {
                if (SelectedIndexes[i] < min) min = SelectedIndexes[i];
                if (SelectedIndexes[i] > max) max = SelectedIndexes[i];
            }

            if (max - min + 1 == SelectedIndexes.Count)
                return (min, max);

            return (-1, -1);
        }

        internal int HoveredIndex {
            get => hovered_index;
            set {
                if (hovered_index != value) {
                    hovered_index = value;
                    owner.Invalidate ();
                }
            }
        }

        protected override void InsertItem (int index, object item)
        {
            base.InsertItem (index, item);

            for (var i = 0; i < SelectedIndexes.Count; i++) {
                if (SelectedIndexes[i] >= index)
                    SelectedIndexes[i]++;
            }

            if (focused_index >= index)
                focused_index++;
        }

        protected override void RemoveItem (int index)
        {
            SelectedIndexes.Remove (index);

            for (var i = 0; i < SelectedIndexes.Count; i++) {
                if (SelectedIndexes[i] > index)
                    SelectedIndexes[i]--;
            }

            if (focused_index > index || focused_index >= Count - 1)
                focused_index = Math.Max (0, focused_index - 1);

            base.RemoveItem (index);
        }

        protected override void ClearItems ()
        {
            SelectedIndexes.Clear ();
            focused_index = 0;

            base.ClearItems ();
        }

        protected override void OnCollectionChanged (NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged (e);

            owner.Invalidate ();
        }

        internal void RemoveSelectedIndex (int index)
        {
            focused_index = Math.Max (index, 0);

            SelectedIndexes.Remove (index);

            owner.Invalidate ();
        }

        internal int SelectedIndex {
            get => SelectedIndexes.Count > 0 ? SelectedIndexes[0] : -1;
            set {
                if (value < -1 || value >= Count)
                    throw new ArgumentOutOfRangeException (nameof (value), "Index out of range");

                AddSelectedIndex (value, true);
            }
        }

        internal List<int> SelectedIndexes { get; } = [];

        internal object? SelectedItem {
            get => SelectedIndexes.Count > 0 ? this[SelectedIndexes[0]] : null;
            set {
                if (value is null) {
                    SelectedIndex = -1;
                    return;
                }

                var index = IndexOf (value);

                if (index == -1)
                    throw new ArgumentException ("Item is not part of this list");

                SelectedIndex = index;
            }
        }

        internal IEnumerable<object> SelectedItems {
            get {
                for (var i = 0; i < SelectedIndexes.Count; i++)
                    yield return this[SelectedIndexes[i]];
            }
        }

        internal void ToggleSelectedIndex (int index)
        {
            if (SelectedIndexes.Contains (index))
                RemoveSelectedIndex (index);
            else
                AddSelectedIndex (index, false);
        }
    }
}
