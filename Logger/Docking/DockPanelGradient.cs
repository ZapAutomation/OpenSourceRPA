using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ZappyLogger.Docking
{
    /// <summary>
    /// The gradient color skin.
    /// </summary>
    [TypeConverter(typeof(DockPanelGradientConverter))]
    public class DockPanelGradient
    {
        #region Fields

        #endregion

        #region cTor

        public DockPanelGradient()
        {
            StartColor = SystemColors.Control;
            EndColor = SystemColors.Control;
            LinearGradientMode = LinearGradientMode.Horizontal;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The beginning gradient color.
        /// </summary>
        [DefaultValue(typeof(SystemColors), "Control")]
        public Color StartColor { get; set; }

        /// <summary>
        /// The ending gradient color.
        /// </summary>
        [DefaultValue(typeof(SystemColors), "Control")]
        public Color EndColor { get; set; }

        /// <summary>
        /// The gradient mode to display the colors.
        /// </summary>
        [DefaultValue(LinearGradientMode.Horizontal)]
        public LinearGradientMode LinearGradientMode { get; set; }

        #endregion
    }
}