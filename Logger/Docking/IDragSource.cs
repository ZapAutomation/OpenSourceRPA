using System.Windows.Forms;

namespace ZappyLogger.Docking
{
    internal interface IDragSource
    {
        #region Properties

        Control DragControl { get; }

        #endregion
    }
}