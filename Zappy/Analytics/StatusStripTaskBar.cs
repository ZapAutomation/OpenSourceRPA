using System;
using System.Windows.Forms;

namespace Zappy.Analytics
{
    public class StatusStripTaskBar : ToolStripControlHost
    {
        public event EventHandler ValueChanged
        {
            add
            {
                Trackbar.ValueChanged += value;
            }
            remove
            {
                Trackbar.ValueChanged -= value;

            }
        }

        public TrackBar Trackbar { get; private set; }
        public StatusStripTaskBar() : base(new TrackBar() { AutoSize = false, Dock = DockStyle.Fill })
        {

            Trackbar = base.Control as TrackBar;
            if (Trackbar != null)
                Trackbar.TickFrequency = 15;
        }

    }
}
