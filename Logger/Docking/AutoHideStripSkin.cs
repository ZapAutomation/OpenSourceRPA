using System.ComponentModel;
using System.Drawing;

namespace ZappyLogger.Docking
{
    /// <summary>
    /// The skin used to display the auto hide strip and tabs.
    /// </summary>
    [TypeConverter(typeof(AutoHideStripConverter))]
    public class AutoHideStripSkin
    {
        #region Fields

        #endregion

        #region cTor

        public AutoHideStripSkin()
        {
            DockStripGradient = new DockPanelGradient();
            DockStripGradient.StartColor = SystemColors.ControlLight;
            DockStripGradient.EndColor = SystemColors.ControlLight;

            TabGradient = new TabGradient();
            TabGradient.TextColor = SystemColors.ControlDarkDark;

            TextFont = SystemFonts.MenuFont;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The gradient color skin for the DockStrips.
        /// </summary>
        public DockPanelGradient DockStripGradient { get; set; }

        /// <summary>
        /// The gradient color skin for the Tabs.
        /// </summary>
        public TabGradient TabGradient { get; set; }

        /// <summary>
        /// Font used in AutoHideStrip elements.
        /// </summary>
        public Font TextFont { get; set; }

        #endregion
    }
}