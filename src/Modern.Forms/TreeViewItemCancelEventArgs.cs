namespace Modern.Forms;

public class TreeViewItemCancelEventArgs : System.ComponentModel.CancelEventArgs
{
    public TreeViewItemCancelEventArgs (TreeViewItem item, CheckState newCheckState)
    {
        Item = item;
        NewCheckState = newCheckState;
    }

    public TreeViewItem Item { get; }

    public CheckState NewCheckState { get; }
}
