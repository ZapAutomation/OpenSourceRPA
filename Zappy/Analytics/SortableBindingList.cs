using System.Collections.Generic;
using System.ComponentModel;

namespace Zappy.Analytics
{
    internal class SortableBindingList<T> : BindingList<T>
    {
        public void Sort(IComparer<T> Comparer)
        {
            (this.Items as List<T>).Sort(Comparer);
        }
    }
}