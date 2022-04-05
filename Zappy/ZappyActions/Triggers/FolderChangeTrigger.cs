using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Triggers.Helpers;
using Zappy.ZappyActions.Triggers.Trigger;

namespace Zappy.ZappyActions.Triggers
{
    [Description("Folder change Trigger")]
    public class FolderChangeTrigger : TemplateAction, IZappyTrigger
    {
        public FolderChangeTrigger()
        {
            NotificationDelay = 1000;
            WatcherChangeTypes = WatcherChangeTypes.All;
            NotifyFilters = NotifyFilters.Size;
            IsDisabled = false;
        }

        [Editor(typeof(FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Category("Optional")]
        public WatcherChangeTypes WatcherChangeTypes { get; set; }

        [Editor(typeof(FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Category("Optional")]
        public NotifyFilters NotifyFilters { get; set; }

                        [Browsable(false)]
        public string Filter { get; set; }

        [Description("Folder path")]
        [Category("Input")]
        public DynamicProperty<string> FolderPath { get; set; }

                [Category("Optional")]
        public DynamicProperty<bool> IncludeSubdirectories { get; set; }

        [Category("Optional")]
        [Description("Delay for notification")]
        public DynamicProperty<int> NotificationDelay { get; set; }

        [XmlIgnore]
        [Category("Optional")]
        public ZappyTriggerType ZappyTriggerType { get { return ZappyTriggerType.FileSystem; } }

        [Category("Optional")]
        public DynamicProperty<bool> IsDisabled { get; set; }

        [Category("Output")]
        [Description("Path of the Add, Delete, Modified")]
        public string AffectedFilePath { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
        }

        public override string AuditInfo()
        {
            return HelperFunctions.HumanizeNameForIZappyAction(this) + " Path:" + this.FolderPath + " Filter:" + this.Filter + " AffectedFilePath:" + this.AffectedFilePath;
        }

        internal IDisposable ConfigureFileSystemTrigger(IZappyAction TriggerAction)
        {
            ZappyFileSystemWatcher _Watcher = new ZappyFileSystemWatcher(TriggerAction);
            _Watcher.Trigger += _Watcher_Trigger;
            _Watcher.StartMonitor();
            return _Watcher;
        }

        internal void _Watcher_Trigger(System.IO.FileSystemEventArgs arg2, ZappyFileSystemWatcher arg3)
        {
            ZappyTriggerManager.CheckAndFireTrigger(arg3.FileSystemWatcherAction, arg2);
        }

        private IDisposable disposableTimerTrigger;

        public virtual IDisposable RegisterTrigger(IZappyExecutionContext context)
        {
            disposableTimerTrigger = ConfigureFileSystemTrigger(this);
            return disposableTimerTrigger;
        }

        public void UnRegisterTrigger()
        {
            disposableTimerTrigger.Dispose();
        }
    }

}

