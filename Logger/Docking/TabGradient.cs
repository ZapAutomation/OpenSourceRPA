using System.ComponentModel;
using System.Drawing;

namespace ZappyLogger.Docking
{
    /// <summary>
    /// The skin used to display the dock pane tab
    /// </summary>
    [TypeConverter(typeof(DockPaneTabGradientConverter))]
    public class TabGradient : DockPanelGradient
    {
        #region Fields

        #endregion

        #region cTor

        public TabGradient()
        {
            TextColor = SystemColors.ControlText;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The text color.
        /// </summary>
        [DefaultValue(typeof(SystemColors), "ControlText")]
        public Color TextColor { get; set; }

        #endregion
    }
}