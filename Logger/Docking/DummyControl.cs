using System.Windows.Forms;

namespace ZappyLogger.Docking
{
    internal class DummyControl : Control
    {
        #region cTor

        public DummyControl()
        {
            SetStyle(ControlStyles.Selectable, false);
        }

        #endregion
    }
}