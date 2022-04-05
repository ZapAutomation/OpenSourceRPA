using System;
using System.ComponentModel;
using Zappy.ActionMap.ScreenMaps;

namespace Zappy.ActionMap.HelperClasses
{
    [Serializable, EditorBrowsable(EditorBrowsableState.Never)]
    public class TopLevelElement : TaskActivityObject
    {
        [Browsable(false)]
        public TimeSpan TimeSpent { get; set; }
        public override object Clone() =>
            FillDetails(new TopLevelElement());

        public override string GetQueryString() =>
            GetQueryString(true);
    }
}