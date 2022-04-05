using System;
using System.Windows.Forms;

namespace Zappy.Analytics
{
    public class StatusStripTextbox : ToolStripControlHost
    {
        public event EventHandler ValueChanged
        {
            add
            {
                TextBox.TextChanged += value;
            }
            remove
            {
                TextBox.TextChanged -= value;

            }
        }

        public TextBox TextBox { get; private set; }
        public StatusStripTextbox() : base(new TextBox() { })
        {
            TextBox = base.Control as TextBox;
        }

    }
}