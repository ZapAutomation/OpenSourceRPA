﻿using System.Drawing;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyTaskEditor.Port
{
    public sealed class TruePort : Port
    {
        public override Point GetOffset(Rect r) => r.CenterBottom;

        public override Brush GetBackBrush() => new SolidBrush(Color.Green);
    }
}