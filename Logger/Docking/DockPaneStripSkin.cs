using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ZappyLogger.Docking
{
    /// <summary>
    /// The skin used to display the document and tool strips and tabs.
    /// </summary>
    [TypeConverter(typeof(DockPaneStripConverter))]
    public class DockPaneStripSkin
    {
        #region Fields

        #endregion

        #region cTor

        public DockPaneStripSkin()
        {
            DocumentGradient = new DockPaneStripGradient();
            DocumentGradient.DockStripGradient.StartColor = SystemColors.Control;
            DocumentGradient.DockStripGradient.EndColor = SystemColors.Control;
            DocumentGradient.ActiveTabGradient.StartColor = SystemColors.ControlLightLight;
            DocumentGradient.ActiveTabGradient.EndColor = SystemColors.ControlLightLight;
            DocumentGradient.InactiveTabGradient.StartColor = SystemColors.ControlLight;
            DocumentGradient.InactiveTabGradient.EndColor = SystemColors.ControlLight;

            ToolWindowGradient = new DockPaneStripToolWindowGradient();
            ToolWindowGradient.DockStripGradient.StartColor = SystemColors.ControlLight;
            ToolWindowGradient.DockStripGradient.EndColor = SystemColors.ControlLight;

            ToolWindowGradient.ActiveTabGradient.StartColor = SystemColors.Control;
            ToolWindowGradient.ActiveTabGradient.EndColor = SystemColors.Control;

            ToolWindowGradient.InactiveTabGradient.StartColor = Color.Transparent;
            ToolWindowGradient.InactiveTabGradient.EndColor = Color.Transparent;
            ToolWindowGradient.InactiveTabGradient.TextColor = SystemColors.ControlDarkDark;

            ToolWindowGradient.ActiveCaptionGradient.StartColor = SystemColors.GradientActiveCaption;
            ToolWindowGradient.ActiveCaptionGradient.EndColor = SystemColors.ActiveCaption;
            ToolWindowGradient.ActiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            ToolWindowGradient.ActiveCaptionGradient.TextColor = SystemColors.ActiveCaptionText;

            ToolWindowGradient.InactiveCaptionGradient.StartColor = SystemColors.GradientInactiveCaption;
            ToolWindowGradient.InactiveCaptionGradient.EndColor = SystemColors.InactiveCaption;
            ToolWindowGradient.InactiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            ToolWindowGradient.InactiveCaptionGradient.TextColor = SystemColors.InactiveCaptionText;

            TextFont = SystemFonts.MenuFont;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The skin used to display the Document style DockPane strip and tab.
        /// </summary>
        public DockPaneStripGradient DocumentGradient { get; set; }

        /// <summary>
        /// The skin used to display the ToolWindow style DockPane strip and tab.
        /// </summary>
        public DockPaneStripToolWindowGradient ToolWindowGradient { get; set; }

        /// <summary>
        /// Font used in DockPaneStrip elements.
        /// </summary>
        public Font TextFont { get; set; }

        #endregion
    }
}