using System.Drawing;

namespace Modern.Forms
{
    public class ContainerControl : ScrollableControl, IContainerControl
    {
        private Control? active_control;

        public ContainerControl ()
        {
        }

        Control? IContainerControl.ActiveControl {
            get => active_control;
            set {
                if (active_control != value) {
                    if (value is not null && !Contains (value))
                        throw new ArgumentException ("Control is not a child of this container.");

                    active_control = value;
                }
            }
        }

        bool IContainerControl.ActivateControl (Control active)
        {
            if (!Contains (active))
                return false;

            ((IContainerControl)this).ActiveControl = active;
            return true;
        }
    }
}
