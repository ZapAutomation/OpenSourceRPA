using System.ComponentModel;

namespace ZappyLogger.Docking
{
    #region DockPanelSkin classes

    /// <summary>
    /// The skin to use when displaying the DockPanel.
    /// The skin allows custom gradient color schemes to be used when drawing the
    /// DockStrips and Tabs.
    /// </summary>
    [TypeConverter(typeof(DockPanelSkinConverter))]
    public class DockPanelSkin
    {
        #region Fields

        #endregion

        #region cTor

        public DockPanelSkin()
        {
            AutoHideStripSkin = new AutoHideStripSkin();
            DockPaneStripSkin = new DockPaneStripSkin();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The skin used to display the auto hide strips and tabs.
        /// </summary>
        public AutoHideStripSkin AutoHideStripSkin { get; set; }

        /// <summary>
        /// The skin used to display the Document and ToolWindow style DockStrips and Tabs.
        /// </summary>
        public DockPaneStripSkin DockPaneStripSkin { get; set; }

        #endregion
    }

    #endregion

    #region Converters

    #endregion
}