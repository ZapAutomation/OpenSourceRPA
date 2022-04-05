using System;
using System.Collections;
using System.Collections.Generic;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.Decode.Mssa
{
    [Serializable]
    internal sealed class ChildrenEnumerator : IEnumerator, IEnumerator<MsaaElement>, IDisposable
    {
        [NonSerialized]
        private AccWrapper current;
        private MsaaElement parent;
        private IEnumerator children;
        private ITaskActivityElement element;
        private ITaskActivityElement switchingElement;

        public ChildrenEnumerator(MsaaElement element)
        {
            parent = element;
            Reset();
        }

        public ChildrenEnumerator(IEnumerator children, ITaskActivityElement element, ITaskActivityElement switchingElement)
        {
            this.children = children;
            this.element = element;
            this.switchingElement = switchingElement;
        }

        public void Dispose()
        {
            current = null;
            parent = null;
        }

        public bool MoveNext()
        {
            current = null;
            if (parent != null)
            {
                current = parent.AccessibleWrapper.GetNextChild(false);
            }
            return current != null;
        }

        public void Reset()
        {
            current = null;
            if (parent != null)
            {
                parent.AccessibleWrapper.UpdateChildren();
            }
        }

        public MsaaElement Current =>
            new MsaaElement(current) { Parent = parent };

        object IEnumerator.Current =>
            Current;
    }
}