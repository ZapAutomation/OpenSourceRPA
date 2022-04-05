using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.Core
{
                        [Description("Nested activities/Run Zappy Task")]
    public class RunZappyTask : TemplateAction
    {
        private string _TaskPath;

        [Description("The File to load actions from")]
        [Category("Input")]
        [XmlIgnore, JsonIgnore]
        public string LoadFromFilePath
        {
            get => _TaskPath;
            set
            {
                _TaskPath = value;
                LoadTask();
                _TaskPath = "";
            }
        }
        private void LoadTask()
        {
            if (!string.IsNullOrEmpty(LoadFromFilePath) && File.Exists(LoadFromFilePath))
            {
                ZappyTask tempTask = ZappyTask.Create(File.OpenRead(LoadFromFilePath));
                NestedExecuteActivities = tempTask.ExecuteActivities;
                            }
        }

        [Category("AutoGenerate")]
        [Description("Auto generates from the task path given")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ActionList NestedExecuteActivities { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ZappyExecutionContext _context = context as ZappyExecutionContext;
            ZappyTask runningZappyTask = context.ContextData[CrapyConstants.ZappyTaskKey] as ZappyTask;

            List<IZappyAction> _Activities = new List<IZappyAction>();
            StartNodeAction importedStartNodeAction = null;
            List<EndNodeAction> importedEndNodeActions = new List<EndNodeAction>();
            for (int i = 0; i < NestedExecuteActivities.Activities.Count; i++)
            {
                if (NestedExecuteActivities.Activities[i] is StartNodeAction sNodeAction)
                {
                    importedStartNodeAction = sNodeAction;
                }

                else if (NestedExecuteActivities.Activities[i] is EndNodeAction eNodeAction)
                {
                    importedEndNodeActions.Add(eNodeAction);
                }
                else
                {

                    if (NestedExecuteActivities.Activities[i] is IVariableAction ivariableAction)
                    {
                        VariableNodeHelper.UpdateContextWithVariables(ivariableAction, _context, true);
                                                                                                                    }
                    _Activities.Add(NestedExecuteActivities.Activities[i]);
                }

                                if(NestedExecuteActivities.Activities[i].ErrorHandlerGuid == Guid.Empty)
                {
                    NestedExecuteActivities.Activities[i].ErrorHandlerGuid = this.ErrorHandlerGuid;
                }
            }

            if (_Activities.Count > 0 && importedStartNodeAction != null && importedEndNodeActions.Count > 0)
            {
                                                foreach (var action in _Activities)
                {
                                        foreach (var importedEndNodeAction in importedEndNodeActions)
                    {
                                                if (action is DecisionNodeAction daction)
                        {
                            if (daction.TrueGuid == importedEndNodeAction.SelfGuid)
                                daction.TrueGuid = this.NextGuid;
                            else if (daction.FalseGuid == importedEndNodeAction.SelfGuid)
                                daction.FalseGuid = this.NextGuid;
                        }
                        else if (action.NextGuid == importedEndNodeAction.SelfGuid)
                            action.NextGuid = this.NextGuid;
                                                if (action.ErrorHandlerGuid == importedEndNodeAction.SelfGuid)
                            action.ErrorHandlerGuid = this.NextGuid;
                    }
                    runningZappyTask.ActionDictionary[action.SelfGuid] = action;
                }
                this.NextGuid = importedStartNodeAction.NextGuid;
                runningZappyTask.Append(_Activities);
            }
            else
            {
                CrapyLogger.log.Error("No zappy actions found");
                throw new Exception("No zappy actions found");
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo();
        }
    }
}
