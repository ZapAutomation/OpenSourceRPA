using System;
using System.Collections;
using System.Windows.Automation;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Plugins.Uia.Uia
{
    [Serializable]
    internal class ChildrenEnumerator : IEnumerator, IDisposable
    {
        private UiaElement current;
        [NonSerialized]
        private AutomationElement currentChild;
        private bool disposed;
        private UiaElement parent;

        public ChildrenEnumerator(UiaElement element)
        {
            parent = element;
            Reset();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public bool MoveNext()
        {
            VirtualizationContext disable = VirtualizationContext.Disable;
            if (currentChild == null)
            {
                currentChild = TreeWalkerHelper.GetFirstChild(parent.InnerElement, disable);
                currentChild = UiaUtility.CheckForHtmlControls(currentChild);
            }
            else
            {
                currentChild = TreeWalkerHelper.GetNextSibling(currentChild, ref disable);
            }
            current = UiaElementFactory.GetUiaElement(currentChild, false);
            if (current != null && !ControlType.MenuItem.NameEquals(current.ControlTypeName))
            {
                current.Parent = parent;
            }
            return current != null;
        }

        public void Reset()
        {
            if (UiaTechnologyManager.Instance != null && ControlType.ComboBox.NameEquals(parent.ControlTypeName))
            {
                UiaTechnologyManager.Instance.LastAccessedComboBox = parent;
            }
            currentChild = null;
            current = null;
        }

        public object Current
        {
            get
            {
                UiaUtility.CopyCeilingElement(current, parent);
                return current;
            }
        }
    }
}

