using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.Core.Helper;
using Zappy.ZappyActions.Loops;
using Zappy.ZappyTaskEditor.EditorPage.Forms;
using Zappy.ZappyTaskEditor.ExecutionHelpers;
using Zappy.ZappyTaskEditor.Nodes;

namespace Zappy.ZappyTaskEditor.EditorPage
{
    internal partial class TaskEditorPage : ITaskEditorPage, IDisposable
    {

        public string Name { get; set; }

                public List<Node> Nodes { get; set; }

        public bool showDynamicPropertyCodeBool => PageFormTabbed.showDynamicPropertyCode;

        public List<T> GetNodes<T>() where T : Node
        {
            return this.Nodes.Where(x => x is T).Cast<T>().ToList();
        }

        public ZappyTask _PageZappyTask;

                public RunZappyTaskHelper _runZappyTaskHelper;


        private TaskEditorPage(ZappyTask PageZappyTask, RunZappyTaskHelper runZappyTaskHelper)
        {
            this.Name = string.Empty;
            this.Nodes = new List<Node>();
            _PageZappyTask = PageZappyTask;
            this.Initialize_Events();
            this.Initialize_Runner();
            _runZappyTaskHelper = runZappyTaskHelper;
            if (PageZappyTask == null && runZappyTaskHelper != null)
            {
                                CreateNodesAndConnections(runZappyTaskHelper.runZappyTask.NestedExecuteActivities.Activities, true);
            }
            else
            {
                CreateNodesAndConnections(_PageZappyTask.ExecuteActivities.Activities, true);
            }
            IsDirty = false;

        }

        public static TaskEditorPage Create(string zappytaskFileName, RunZappyTaskHelper runZappyTaskHelper)
        {
            
            ZappyTask _PageTask = null;
            TaskEditorPage _Page;
            if (!string.IsNullOrEmpty(zappytaskFileName))
            {
                                try
                {
                    _PageTask = ZappyTask.Create(zappytaskFileName);
                }
                catch
                {
                    MessageBox.Show("Can not Read/Load ZappyTask!");
                    _PageTask = null;
                    zappytaskFileName = string.Empty;
                }
                                                                                                                                                                                                                            }
            if (runZappyTaskHelper != null)
            {
                _Page = new TaskEditorPage(null, runZappyTaskHelper);
                return _Page;
            }
            if (_PageTask == null)
            {
                _PageTask = new ZappyTask();
                _PageTask.ExecuteActivities.Add(new StartNodeAction() { EditorLocationX = EditorConstants.StartCord*8, EditorLocationY = EditorConstants.StartCord });
                _PageTask.ExecuteActivities.Add(new EndNodeAction() { EditorLocationX = EditorConstants.StartCord*8, EditorLocationY = EditorConstants.StartCord * 10 });

            }
                                                                                                _Page = new TaskEditorPage(_PageTask, null);
            _Page.ZappytaskFullFileName = zappytaskFileName;
            return _Page;
        }

        private Node GetNodeById(Guid id)
        {
            return this.Nodes.FirstOrDefault(x => x.Id == id);
        }

        private Node AddNode(string activityId, int x, int y, bool FromFileOpen = false)
        {
            if (this.State == PageState.Running)
            {
                MessageBox.Show("The robot is currently running.");
                return null;
            }
            IZappyAction activity = ZappyTaskEditorHelper.CreateInstance(activityId);
                        ConnectNearestNode(activity, y);
            return AddNode(activity, x, y, FromFileOpen);
        }

        private Node AddNode(IZappyAction action, int x, int y, bool FromFileOpen)
        {
            Node node;
                                                                                                                                                                                                if (action is StartNodeAction)
            {
                node = Nodes.FirstOrDefault(nd => nd is StartNode);
                if (node == null)
                    node = new StartNode(action);
            }
            else if (action is EndNodeAction)
                node = new EndNode(action);
            else if (action is DecisionNodeAction)
                node = new DecisionNode(action);
            else if (action is IZappyLoopStartAction loopAction)
            {
                node = new LoopStartNode(action);
                if (!FromFileOpen && loopAction.LoopEndGuid == Guid.Empty)
                    AddNode((action as IZappyLoopStartAction).GetEndLoopAction() as IZappyAction, x, y + PageRenderOptions.GridSize * EditorConstants.NodeWidth * 3, FromFileOpen);
            }
            else if (action is IZappyLoopEndAction)
                node = new LoopEndNode(action);
            else if (action is IVariableAction)
            {
                var variableIndex = 1;
                while (this.GetNodes<GlobalVariableNode>().Count(v => v.Name == "Variable " + variableIndex) > 0)
                {
                    variableIndex++;
                }
                node = new GlobalVariableNode(action, variableIndex);
            }
                                                            else if (action is IZappyAction)
            {
                node = new ZappyActionUINode(action);
            }
            else
            {
                throw new NotSupportedException();
            }
            var bounds = node.Bounds;
                        bounds.X = x - bounds.Width / 2;
            bounds.Y = y - bounds.Height / 2;
            node.SetBounds(bounds, this);
            this.Nodes.Add(node);
            IsDirty = true;
            return node;
        }

        private void RemoveNode(Node node)
        {
            if (node is StartNode)
            {
                MessageBox.Show("Start activity cannot be deleted.");
            }
            else if (node is LoopStartNode loopStartNode)
            {
                if (this.GetNodeById(loopStartNode.LoopEndNodeId) is Node relatedNode)
                {
                    this.Nodes.Remove(relatedNode);
                    this.SelectedNodes.Remove(relatedNode);

                }
                this.Nodes.Remove(node);
                this.SelectedNodes.Remove(node);
            }
            else if (node is LoopEndNode loopEndNode)
            {
                if (this.GetNodeById(loopEndNode.LoopStartNodeId) is Node relatedNode)
                {
                    this.Nodes.Remove(relatedNode);
                    this.SelectedNodes.Remove(relatedNode);
                }
                this.Nodes.Remove(node);
                this.SelectedNodes.Remove(node);
            }
            else
            {
                this.Nodes.Remove(node);
                this.SelectedNodes.Remove(node);

            }
            IsDirty = true;
        }

        public override string ToString()
        {
            return string.Empty;        }

        public void Dispose()
        {
                    }

        private void ConnectNearestNode(IZappyAction action, int height)
        {
            Node nearestNode = GetNearestNode(height);

            if (nearestNode != null && !(nearestNode is DecisionNode))
            {
                if (nearestNode.Ports.Count > 0)
                    if (!(action is IVariableAction) && nearestNode.Activity.NextGuid == Guid.Empty)
                    {
                                                nearestNode.Ports[0].NextNodeId = action.SelfGuid;
                        nearestNode.Activity.NextGuid = action.SelfGuid;
                        LinkInputOutputProperties(nearestNode.Activity, action);
                    }
            }

        }

        private Node GetNearestNode(int height)
        {
            Node node = null;
            int Distance = Int32.MaxValue;
            for (int i = 0; i < Nodes.Count; i++)
            {
                int num = height - Nodes[i].Bounds.Bottom;
                if (num > 0 && num < Distance && !(Nodes[i].Activity is EndNodeAction))
                {
                    Distance = num;
                    node = Nodes[i];
                }

            }

            return node;
        }

                        internal void LinkInputOutputProperties(IZappyAction souceAction, IZappyAction destAction)
        {
            try
            {
                                IEnumerable<PropertyInfo> propsSourceOutputs = souceAction.GetType().GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(CategoryAttribute))
                            && prop.GetCustomAttribute<CategoryAttribute>().Category.Equals("Output"));

                                IEnumerable<PropertyInfo> propsSourceInputs = souceAction.GetType().GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(CategoryAttribute))
                            && prop.GetCustomAttribute<CategoryAttribute>().Category.Equals("Input"));

                IEnumerable<PropertyInfo> propsDestInputs = destAction.GetType().GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(CategoryAttribute))
                            && prop.GetCustomAttribute<CategoryAttribute>().Category.Equals("Input"));
                Type type = typeof(IDynamicProperty);

                foreach (PropertyInfo propDestInput in propsDestInputs)
                {
                    try
                    {
                        object property = propDestInput.GetValue(destAction);
                        if (type.IsAssignableFrom(propDestInput.PropertyType))
                        {
                                                        foreach (PropertyInfo propSourceOutput in propsSourceOutputs)
                            {

                                                                if (property == null)
                                {
                                    property = propDestInput.PropertyType.GetConstructor(new System.Type[0] { })?.Invoke(null);
                                    IDynamicProperty dp = property as IDynamicProperty;
                                    
                                                                        if (dp.ElementType == propSourceOutput.PropertyType && propDestInput.Name == propSourceOutput.Name)
                                    {

                                        Guid _SeletcedActionID = souceAction.SelfGuid;
                                        string _PropertyName = propSourceOutput.Name;

                                        dp.DymanicKey = ZappyExecutionContext.GetKey(_SeletcedActionID, _PropertyName);
                                        propDestInput.SetValue(destAction, dp);
                                    }
                                    else
                                    {
                                        property = null;
                                    }
                                }
                            }


                                                    }


                        foreach (var propSourceInput in propsSourceInputs)
                        {
                            if (propSourceInput.PropertyType == propDestInput.PropertyType
                                && property == null && propDestInput.Name == propSourceInput.Name)
                            {
                                var valSource = propSourceInput.GetValue(souceAction, null);
                                propDestInput.SetValue(destAction, valSource.Copy());
                                                                                            }
                        }
                                                                                                                                                                                                                        
                                            }
                    catch
                    {
                                                                    }
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                            }
        }
    }
}