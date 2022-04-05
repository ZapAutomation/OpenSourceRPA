using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Automation;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;

namespace Zappy.Decode.Helper
{
    internal sealed class ZappyTaskPluginManager : IUITechnologyPluginManager
    {
        private UITechnologyManager defaultPlugin;
        private IDictionary<string, UITechnologyManager> nameToTechnologyManagerMap = new Dictionary<string, UITechnologyManager>(StringComparer.OrdinalIgnoreCase);
        private ReadOnlyCollection<UITechnologyManager> technologyManagerList;
        private IDictionary<string, UITechnologyManager> typeToAllTechnologyManagersMap = new Dictionary<string, UITechnologyManager>(StringComparer.OrdinalIgnoreCase);

        public ZappyTaskPluginManager(IList<UITechnologyManager> technologyManagerList, UITechnologyManager defaultManager)
        {
            DefaultTechnologyManager = defaultManager;
            if (technologyManagerList != null)
            {
                foreach (UITechnologyManager manager in technologyManagerList)
                {
                    if (manager != null)
                    {
                        if (string.IsNullOrEmpty(manager.TechnologyName))
                        {
                            object[] args = { manager.TechnologyName };
                                                    }
                        AddTechnologyManager(manager);
                    }
                }
            }
        }

        private void AddTechnologyManager(UITechnologyManager technologyManager)
        {
            ZappyTaskUtilities.CheckForNull(technologyManager, "technologyManager");
            string technologyName = technologyManager.TechnologyName;
            ZappyTaskUtilities.CheckForNull(technologyName, "TechnologyName");
            technologyManagerList = null;
            string fullName = technologyManager.GetType().FullName;
            if (!typeToAllTechnologyManagersMap.ContainsKey(fullName))
            {
                typeToAllTechnologyManagersMap[fullName] = technologyManager;
            }
            if (!nameToTechnologyManagerMap.ContainsKey(technologyName))
            {
                nameToTechnologyManagerMap[technologyName] = technologyManager;
                object[] args = { technologyManager.GetType(), technologyManager.TechnologyName };
                
            }
            else
            {
                object[] objArray2 = { technologyName };
                
            }
        }

        public UITechnologyManager GetTechnologyManagerByAutomationElementOrWindowHandle(object element)
        {
            IntPtr? nullable;
            UITechnologyManager defaultPlugin = this.defaultPlugin;
            int num = 0;
            if (!(element is AutomationElement) || element == null)
            {
                if (element == null)
                {
                    return defaultPlugin;
                }
                nullable = element as IntPtr?;
                if (nullable.Value == IntPtr.Zero)
                {
                    return defaultPlugin;
                }
            }
            foreach (UITechnologyManager manager2 in TechnologyManagers)
            {
                if (manager2 != this.defaultPlugin)
                {
                    try
                    {
                        int controlSupportLevel;
                        if (element is AutomationElement)
                        {
                            controlSupportLevel = manager2.GetControlSupportLevel(element as AutomationElement);
                        }
                        else
                        {
                            nullable = element as IntPtr?;
                            controlSupportLevel = manager2.GetControlSupportLevel(nullable.Value);
                        }
                        if (controlSupportLevel > num)
                        {
                            defaultPlugin = manager2;
                            num = controlSupportLevel;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            try
            {
                int num3;
                if (element is AutomationElement)
                {
                    num3 = this.defaultPlugin.GetControlSupportLevel(element as AutomationElement);
                }
                else
                {
                    nullable = element as IntPtr?;
                    num3 = this.defaultPlugin.GetControlSupportLevel(nullable.Value);
                }
                if (num3 > num)
                {
                    num = num3;
                    defaultPlugin = this.defaultPlugin;
                }
            }
            catch (Exception)
            {
                if (num < 1)
                {
                    throw;
                }
            }
                                                            return defaultPlugin;
        }

        public UITechnologyManager GetTechnologyManagerByName(string technologyName)
        {
            if (nameToTechnologyManagerMap.ContainsKey(technologyName))
            {
                return nameToTechnologyManagerMap[technologyName];
            }
            object[] args = { technologyName };
            
            return null;
        }

        public UITechnologyManager GetTechnologyManagerByTypeInAll(string typeName)
        {
            if (typeToAllTechnologyManagersMap.ContainsKey(typeName))
            {
                return typeToAllTechnologyManagersMap[typeName];
            }
            object[] args = { typeName };
            
            return null;
        }

        private void RemoveTechnologyManager(UITechnologyManager technologyManager)
        {
            UITechnologyManager manager;
            ZappyTaskUtilities.CheckForNull(technologyManager, "technologyManager");
            string technologyName = technologyManager.TechnologyName;
            ZappyTaskUtilities.CheckForNull(technologyName, "TechnologyName");
            technologyManagerList = null;
            string fullName = technologyManager.GetType().FullName;
            object[] args = { fullName, technologyName };
            
            if (nameToTechnologyManagerMap.TryGetValue(technologyName, out manager))
            {
                nameToTechnologyManagerMap.Remove(technologyName);
            }
            else
            {
                object[] objArray2 = { technologyName };
                
            }
        }

        public void StartSession(bool recordingSession)
        {
            IList<UITechnologyManager> source = new List<UITechnologyManager>(TechnologyManagers);
            IList<UITechnologyManager> initializedManagers = new List<UITechnologyManager>();
            OrderablePartitioner<UITechnologyManager> partitioner = Partitioner.Create(source);
            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 4
            };
            try
            {
                Parallel.ForEach(partitioner, parallelOptions, delegate (UITechnologyManager manager)
                {
                    try
                    {
                        manager.StartSession(recordingSession);
                        initializedManagers.Add(manager);
                    }
                    catch (NotSupportedException)
                    {
                        RemoveTechnologyManager(manager);
                    }
                    catch (NotImplementedException)
                    {
                        RemoveTechnologyManager(manager);
                    }
                    catch
                    {
                        foreach (UITechnologyManager manager2 in initializedManagers)
                        {
                            manager2.StopSession();
                        }
                        throw;
                    }
                });
            }
            catch (AggregateException exception)
            {
                if (exception.InnerExceptions[0] is ZappyTaskException)
                {
                    exception.InnerExceptions[0].Data["OriginalStackTrace"] = exception.InnerExceptions[0].StackTrace;
                    throw exception.InnerExceptions[0];
                }
                throw;
            }
        }

        public bool StartSessionAndAdd(UITechnologyManager technologyManager, bool recordingSession)
        {
            if (technologyManager != null && !TechnologyManagers.Contains(technologyManager))
            {
                try
                {
                    technologyManager.StartSession(recordingSession);
                    AddTechnologyManager(technologyManager);
                    return true;
                }
                catch (NotSupportedException)
                {
                }
                catch (NotImplementedException)
                {
                }
            }
            return false;
        }

        public void StopSession()
        {
            OrderablePartitioner<UITechnologyManager> partitioner = Partitioner.Create(TechnologyManagers);
            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 4
            };
            try
            {
                Parallel.ForEach(partitioner, parallelOptions, plugin => plugin.StopSession());
            }
            catch (AggregateException exception)
            {
                if (exception.InnerExceptions[0] is ZappyTaskException)
                {
                    exception.InnerExceptions[0].Data["OriginalStackTrace"] = exception.InnerExceptions[0].StackTrace;
                    throw exception.InnerExceptions[0];
                }
                throw;
            }
        }

        public bool StopSessionAndRemove(UITechnologyManager technologyManager)
        {
            if (technologyManager == null || !TechnologyManagers.Contains(technologyManager))
            {
                return false;
            }
            try
            {
                technologyManager.StopSession();
            }
            catch (NotSupportedException)
            {
            }
            catch (NotImplementedException)
            {
            }
            RemoveTechnologyManager(technologyManager);
            return true;
        }

        public UITechnologyManager DefaultTechnologyManager
        {
            get =>
                defaultPlugin;
            private set
            {
                defaultPlugin = value;
                if (defaultPlugin != null)
                {
                    AddTechnologyManager(defaultPlugin);
                }
            }
        }

        public ICollection<string> TechnologyManagerNames =>
            nameToTechnologyManagerMap.Keys;

        public IList<UITechnologyManager> TechnologyManagers
        {
            get
            {
                if (technologyManagerList == null)
                {
                    IList<UITechnologyManager> list = new List<UITechnologyManager>(nameToTechnologyManagerMap.Values);
                    technologyManagerList = new ReadOnlyCollection<UITechnologyManager>(list);
                }
                return technologyManagerList;
            }
        }


    }
}