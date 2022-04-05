using System.Collections.Generic;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Helpers
{
    public abstract class ApplicationBase : ZappyTaskControl
    {
        private bool m_closeOnCleanupFlag;
        private static List<ApplicationBase> s_ApplicationControlObjects = new List<ApplicationBase>();

        internal ApplicationBase()
        {
        }

        internal ApplicationBase(TaskActivityObject uiObject) : base(uiObject)
        {
        }

        protected internal static void AddToApplicationCache(ApplicationBase applicationWindow)
        {
            foreach (ApplicationBase base2 in s_ApplicationControlObjects)
            {
                if (base2 == applicationWindow)
                {
                    return;
                }
            }
            s_ApplicationControlObjects.Add(applicationWindow);
        }

        public abstract void Close();
        internal static void CloseProcessesOnPlaybackCleanup()
        {
            List<ApplicationBase> list = new List<ApplicationBase>(s_ApplicationControlObjects);
            foreach (ApplicationBase base2 in list)
            {
                try
                {
                    if (base2.CloseOnPlaybackCleanup)
                    {
                        base2.Close();
                    }
                }
                catch
                {
                }
            }
            s_ApplicationControlObjects.Clear();
        }

        protected internal static void RemoveFromApplicationCache(ApplicationBase applicationWindow)
        {
            int index = -1;
            bool flag = false;
            foreach (ApplicationBase base2 in s_ApplicationControlObjects)
            {
                index++;
                if (base2 == applicationWindow)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                s_ApplicationControlObjects.RemoveAt(index);
            }
        }

        public bool CloseOnPlaybackCleanup
        {
            get =>
                m_closeOnCleanupFlag;
            set
            {
                m_closeOnCleanupFlag = value;
                if (value)
                {
                    AddToApplicationCache(this);
                }
                else
                {
                    RemoveFromApplicationCache(this);
                }
            }
        }
    }
}