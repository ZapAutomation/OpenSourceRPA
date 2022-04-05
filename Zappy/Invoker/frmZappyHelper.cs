using System.Collections.Generic;
using Zappy.Helpers;
using Zappy.SharedInterface;

namespace Zappy.Invoker
{
    class frmZappyHelper
    {
        internal static List<IZappyAction> CloneSelectedActivities(List<IZappyAction> uitaskSelectedActivities)
        {
            List<IZappyAction> clonedActions = new List<IZappyAction>();
            for (int i = 0; i < uitaskSelectedActivities.Count; i++)
            {
                IZappyAction curAction = uitaskSelectedActivities[i];
                clonedActions.Add(curAction.DeepCloneAction());
            }
            return clonedActions;
        }
    }
}
