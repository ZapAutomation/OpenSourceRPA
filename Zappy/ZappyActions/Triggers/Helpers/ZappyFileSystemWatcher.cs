using System;
using System.Collections.Generic;
using System.IO;
using Zappy.Decode.LogManager;
using Zappy.SharedInterface;

namespace Zappy.ZappyActions.Triggers.Helpers
{
    public class ZappyFileSystemWatcher : IDisposable
    {
        private FileSystemWatcher FileSystemWatcher;
        public FolderChangeTrigger FileSystemWatcherAction;
        public event Action<FileSystemEventArgs, ZappyFileSystemWatcher> Trigger;
        List<KeyValuePair<FileSystemEventArgs, System.Threading.Timer>> _EventTimers;

        WatcherChangeTypes ChangeType { get { return FileSystemWatcherAction.WatcherChangeTypes; } }

        string Filter { get { return FileSystemWatcherAction.Filter; } }

        bool IncludeSubdirectories { get { return FileSystemWatcherAction.IncludeSubdirectories; } }

        System.IO.NotifyFilters NotifyFilters { get { return FileSystemWatcherAction.NotifyFilters; } }

        string Path { get { return FileSystemWatcherAction.FolderPath; } }

        public ZappyFileSystemWatcher(IZappyAction fileSystemWatcherAction)
        {
            _EventTimers = new List<KeyValuePair<FileSystemEventArgs, System.Threading.Timer>>();
            FileSystemWatcherAction = fileSystemWatcherAction as FolderChangeTrigger;
            if (FileSystemWatcherAction == null)
                throw new Exception("Invalid Argument : fileSystemWatcherAction, value must be of type : FileSystemWatcherAction");
        }

        protected virtual void Event_Trigger(object sender, FileSystemEventArgs args)
        {
            bool _Found = false;
            lock (_EventTimers)
            {
                for (int i = 0; i < _EventTimers.Count; i++)
                {
                    KeyValuePair<FileSystemEventArgs, System.Threading.Timer> _Kvp = _EventTimers[i];
                    if (_Kvp.Key.ChangeType == args.ChangeType && _Kvp.Key.FullPath == args.FullPath && _Kvp.Key.Name == args.Name)
                    {
                        _Found = true;
                        try { _Kvp.Value.Change(FileSystemWatcherAction.NotificationDelay, System.Threading.Timeout.Infinite); }
                        catch { }
                        break;
                    }
                }
                if (!_Found)
                {
                    _EventTimers.Add(new KeyValuePair<FileSystemEventArgs, System.Threading.Timer>(args,
                        new System.Threading.Timer(FireTriggerEvent_OnTimer, args, FileSystemWatcherAction.NotificationDelay, System.Threading.Timeout.Infinite)));
                }
            }
        }

        void FireTriggerEvent_OnTimer(object data)
        {
            FileSystemEventArgs args = data as FileSystemEventArgs;
            FileSystemWatcherAction.AffectedFilePath = args.FullPath;
            bool _Found = false;
            KeyValuePair<FileSystemEventArgs, System.Threading.Timer> _Kvp = default(KeyValuePair<FileSystemEventArgs, System.Threading.Timer>);
            lock (_EventTimers)
            {
                int _Count = _EventTimers.Count;
                for (int i = 0; i < _Count; i++)
                {
                    _Kvp = _EventTimers[i];
                    if (_Kvp.Key == args)
                    {
                        _Found = true;
                        _EventTimers.RemoveAt(i);
                        _Kvp.Value.Dispose();
                        break;
                    }
                }
            }
            if (_Found)
            {
                Trigger?.Invoke(args, this);
            }
        }

        public void StartMonitor()
        {

            this.FileSystemWatcher = new System.IO.FileSystemWatcher(Path);
            this.FileSystemWatcher.Filter = Filter;            this.FileSystemWatcher.IncludeSubdirectories = this.IncludeSubdirectories;
            this.FileSystemWatcher.NotifyFilter = this.NotifyFilters;

            if (this.ChangeType.HasFlag(WatcherChangeTypes.All) || this.ChangeType.HasFlag(WatcherChangeTypes.Changed))
            {
                this.FileSystemWatcher.Changed += new FileSystemEventHandler(this.Event_Trigger);
            }
            if (this.ChangeType.HasFlag(WatcherChangeTypes.All) || this.ChangeType.HasFlag(WatcherChangeTypes.Created))
            {
                this.FileSystemWatcher.Created += new FileSystemEventHandler(this.Event_Trigger);
            }
            if (this.ChangeType.HasFlag(WatcherChangeTypes.All) || this.ChangeType.HasFlag(WatcherChangeTypes.Renamed))
            {
                this.FileSystemWatcher.Renamed += new RenamedEventHandler(this.Event_Trigger);
            }
            if (this.ChangeType.HasFlag(WatcherChangeTypes.All) || this.ChangeType.HasFlag(WatcherChangeTypes.Deleted))
            {
                this.FileSystemWatcher.Deleted += new FileSystemEventHandler(this.Event_Trigger);
            }
            this.FileSystemWatcher.EnableRaisingEvents = true;
        }

        public void StopMonitor()
        {
            try
            {
                if (this.FileSystemWatcher != null)
                {
                    this.FileSystemWatcher.EnableRaisingEvents = false;
                    this.FileSystemWatcher.Dispose();
                    this.FileSystemWatcher = null;
                    Trigger = null;
                }
            }
            catch (Exception exception1)
            {
                CrapyLogger.log.Error(exception1);
            }
        }


        #region IDisposable Support

        public void Dispose()
        {
            StopMonitor();
        }
        #endregion

    }

}

