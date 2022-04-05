using System;
using System.Collections;
using System.Runtime.InteropServices;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Mssa
{
    internal sealed class AccChildrenEnumerator
    {
        private IEnumerator childEnumerator;
        private int childrenCount;
        private int currentChildIndex;
        private AccWrapper element;

        public AccChildrenEnumerator(AccWrapper element)
        {
            this.element = element;
            if (this.element != null && this.element.ChildId == 0)
            {
                try
                {
                    childrenCount = this.element.ChildCount;
                    childEnumerator = this.element.AccessibleObject as IEnumerator;
                    if (childEnumerator != null)
                    {
                        childEnumerator.Reset();
                    }
                }
                catch (SystemException)
                {
                    childrenCount = 0;
                    childEnumerator = null;
                }
            }
        }

        public AccWrapper GetNextChild(bool ignoreInvisibleChild)
        {
            AccWrapper wrapper = null;
            while (currentChildIndex < childrenCount)
            {
                try
                {
                    object current;
                    if (childEnumerator != null && childEnumerator.MoveNext())
                    {
                        current = childEnumerator.Current;
                    }
                    else
                    {
                        current = currentChildIndex + 1;
                    }
                    wrapper = AccWrapper.GetElementFromChildObject(element.AccessibleObject, current, ignoreInvisibleChild);
                }
                catch (COMException)
                {
                }
                catch (ZappyTaskControlNotAvailableException)
                {
                }
                currentChildIndex++;
                if (wrapper != null)
                {
                    wrapper.ContainerElement = element.ContainerElement;
                    return wrapper;
                }
            }
            return wrapper;
        }
    }
}