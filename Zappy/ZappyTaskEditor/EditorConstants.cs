using System;
using Zappy.ZappyTaskEditor.EditorPage;

namespace Zappy.ZappyTaskEditor
{
    internal static class EditorConstants
    {
                internal const int NodeWidth = 6;
        internal const int NodeHeight = 2;        internal const int StartCord = 40;
        internal static int OffSetForY = Convert.ToInt32(PageRenderOptions.GridSize * EditorConstants.NodeHeight*1.5);

    }
}
