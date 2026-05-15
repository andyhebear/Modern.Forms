namespace Modern.Forms;

public class TreeViewItemEventArgs : EventArgs
{
    public TreeViewItemEventArgs (TreeViewItem item)
    {
        Item = item;
    }

    public TreeViewItem Item { get; }
}
