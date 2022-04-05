using System.Collections.Generic;
using Zappy.ActionMap.TaskTechnology;

namespace Zappy.Decode.Helper
{
    internal interface IUITechnologyPluginManager
    {
        UITechnologyManager GetTechnologyManagerByAutomationElementOrWindowHandle(object element);
        UITechnologyManager GetTechnologyManagerByName(string technologyName);
        UITechnologyManager GetTechnologyManagerByTypeInAll(string typeName);
        void StartSession(bool recordingSession);
        bool StartSessionAndAdd(UITechnologyManager technologyManager, bool recordingSession);
        void StopSession();
        bool StopSessionAndRemove(UITechnologyManager technologyManager);

        UITechnologyManager DefaultTechnologyManager { get; }

        ICollection<string> TechnologyManagerNames { get; }

        IList<UITechnologyManager> TechnologyManagers { get; }
    }
}