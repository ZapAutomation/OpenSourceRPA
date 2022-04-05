using System;
using System.Collections.Generic;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Core;
using Zappy.ZappyTaskEditor.Nodes;

namespace Zappy.ZappyTaskEditor.EditorPage
{
    internal partial class TaskEditorPage
    {
        public void ImportTask(ZappyTask _ImportedTask, ref int x, ref int y)
        {
            List<IZappyAction> _Activities = new List<IZappyAction>(_ImportedTask.ExecuteActivities.Activities.Count);
            for (int i = 0; i < _ImportedTask.ExecuteActivities.Activities.Count; i++)
            {
                if (x != -1 && (_ImportedTask.ExecuteActivities.Activities[i] is StartNodeAction ||
                                _ImportedTask.ExecuteActivities.Activities[i] is EndNodeAction))
                    continue;
                _Activities.Add(_ImportedTask.ExecuteActivities.Activities[i]);
            }


            List<IZappyAction> clonedActionsWithUpdatedLink = CloneActionsWithUpdatedLink(_Activities);


                                                                        {
                _PageZappyTask.Append(clonedActionsWithUpdatedLink);
            }

            CreateNodesAndConnections(clonedActionsWithUpdatedLink, false,x, y);
        }

        public void CreateNodesAndConnections(List<IZappyAction> Activities, bool FromFileOpen = false, int x = 0, int y= 0, bool fromPaste = false)
        {
            Dictionary<Guid, Node> _NodesDict = new Dictionary<Guid, Node>();

            int refStartX = 0, refStartY = int.MaxValue;

            if (fromPaste)
            {
                                for (int i = 0; i < Activities.Count; i++)
                {
                    if (refStartY > Activities[i].EditorLocationY)
                    {
                        refStartY = Activities[i].EditorLocationY;
                        refStartX = Activities[i].EditorLocationX;
                    }
                }
            }
            else
            {
                refStartY = 0;
            }
            
            for (int i = 0; i < Activities.Count; i++)
            {
                IZappyAction item = Activities[i];

                Node _Node;
                                if (item.EditorLocationX <= 0 && item.EditorLocationY <= 0)
                {                 
                    if (y == 0)
                        y = EditorConstants.OffSetForY / 2;
                    else
                        y += EditorConstants.OffSetForY;
                    _Node = AddNode(item, x, y, FromFileOpen);                 
                }
                                else
                {
                    _Node = AddNode
                        (item, x + item.EditorLocationX - refStartX,
                        y + item.EditorLocationY - refStartY, FromFileOpen);
                }
                                if (fromPaste)
                    ApplyFlag(_Node, NodeState.Selected, true);
                _NodesDict[_Node.Activity.SelfGuid] = _Node;
            }

            for (int i = 0; i < Activities.Count; i++)
            {
                IZappyAction item = Activities[i];
                Node _Node, _NextNode;
                if (_NodesDict.TryGetValue(item.SelfGuid, out _Node))
                {
                    if (_Node is DecisionNode)
                    {
                        if (_NodesDict.TryGetValue((item as DecisionNodeAction).TrueGuid, out _NextNode))
                            (_Node as DecisionNode).Ports[0].NextNodeId = _NextNode.Id;
                        if (_NodesDict.TryGetValue((item as DecisionNodeAction).FalseGuid, out _NextNode))
                            (_Node as DecisionNode).Ports[1].NextNodeId = _NextNode.Id;
                    }
                    else if (_NodesDict.TryGetValue(item.NextGuid, out _NextNode))
                    {
                        if (_Node.Ports.Count > 0)
                            _Node.Ports[0].NextNodeId = _NextNode.Id;
                    }

                    if (item.ErrorHandlerGuid != Guid.Empty && _NodesDict.TryGetValue(item.ErrorHandlerGuid, out _NextNode))
                    {
                        if (_Node.Ports.Count > 0)
                            _Node.Ports[_Node.Ports.Count - 1].NextNodeId = _NextNode.Id;
                    }
                }
            }

            this.Canvas.Invalidate();
        }


    }
}