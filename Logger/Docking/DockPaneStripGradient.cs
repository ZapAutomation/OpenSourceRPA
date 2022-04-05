using System.ComponentModel;

namespace ZappyLogger.Docking
{
    /// <summary>
    /// The skin used to display the DockPane strip and tab.
    /// </summary>
    [TypeConverter(typeof(DockPaneStripGradientConverter))]
    public class DockPaneStripGradient
    {
        #region Fields

        #endregion

        #region cTor

        public DockPaneStripGradient()
        {
            DockStripGradient = new DockPanelGradient();
            ActiveTabGradient = new TabGradient();
            InactiveTabGradient = new TabGradient();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The gradient color skin for the DockStrip.
        /// </summary>
        public DockPanelGradient DockStripGradient { get; set; }

        /// <summary>
        /// The skin used to display the active DockPane tabs.
        /// </summary>
        public TabGradient ActiveTabGradient { get; set; }

        /// <summary>
        /// The skin used to display the inactive DockPane tabs.
        /// </summary>
        public TabGradient InactiveTabGradient { get; set; }

        #endregion
    }
}