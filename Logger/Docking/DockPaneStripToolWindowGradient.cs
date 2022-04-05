using System.ComponentModel;

namespace ZappyLogger.Docking
{
    /// <summary>
    /// The skin used to display the DockPane ToolWindow strip and tab.
    /// </summary>
    [TypeConverter(typeof(DockPaneStripGradientConverter))]
    public class DockPaneStripToolWindowGradient : DockPaneStripGradient
    {
        #region Fields

        #endregion

        #region cTor

        public DockPaneStripToolWindowGradient()
        {
            ActiveCaptionGradient = new TabGradient();
            InactiveCaptionGradient = new TabGradient();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The skin used to display the active ToolWindow caption.
        /// </summary>
        public TabGradient ActiveCaptionGradient { get; set; }

        /// <summary>
        /// The skin used to display the inactive ToolWindow caption.
        /// </summary>
        public TabGradient InactiveCaptionGradient { get; set; }

        #endregion
    }
}