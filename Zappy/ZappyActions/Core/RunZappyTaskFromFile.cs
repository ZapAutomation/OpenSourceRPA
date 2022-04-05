using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Core
{
    [Description("Run Zappy Task from file")]
    public class RunZappyTaskFromFile : RunZappyTask
    {
        public RunZappyTaskFromFile()
        {
            NextGuidCopy = Guid.Empty;
        }

        [Category("Input")]
        [Description("Runs from TaskPath if correct path given - leave blank to create custom run zappy task")]
        public new DynamicProperty<string> LoadFromFilePath { get; set; }

        [Category("Optional")]
        [Description("Queue in execution engine")]
        public DynamicProperty<bool> QueueInExecutionEngine { get; set; }

        [Category("Output")]
        [Description("if Runs from TaskPath is correct then true ")]
        public bool Output { get; set; }

        [Browsable(false), XmlIgnore]
        public Guid NextGuidCopy { get; set; }
        private bool LoadTask()
        {
                                    ZappyTask tempTask = ZappyTask.Create(File.OpenRead(LoadFromFilePath));
            NestedExecuteActivities = tempTask.ExecuteActivities;
                        return true;
                    }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (QueueInExecutionEngine)
            {
                CommonProgram.StartPlaybackFromFile(LoadFromFilePath);
            }
            else
            {
                                if (NextGuidCopy == Guid.Empty)
                    NextGuidCopy = this.NextGuid;
                                else
                    this.NextGuid = NextGuidCopy;
                                Output = LoadTask();
                try
                {
                    base.Invoke(context, actionInvoker);
                }
                catch (Exception e)
                {
                    CrapyLogger.log.Error(" Object reference not set to an instance of an object " + e);
                }
                finally
                {
                    Output = true;
                }
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " TaskPath:" + this.LoadFromFilePath;
        }
    }
}