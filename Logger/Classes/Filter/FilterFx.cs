using System.Collections.Generic;

namespace ZappyLogger.Classes.Filter
{
    internal delegate void FilterFx(FilterParams filterParams, List<int> filterResultLines,
        List<int> lastFilterResultLines, List<int> filterHitList);
}