using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using Zappy.ActionMap.ScreenMaps;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;

namespace Zappy.ActionMap.ZappyTaskUtil
{


    public class ZappyTask
    {
        internal static readonly string AssemblyFileVersion = ZappyTaskUtilities.GetAssemblyFileVersion(ZappyTaskUtilities.GetExecutingAssembly());
        private string assemblyVersion;
        private ZappyTaskEnvironment configuration;
        private bool containsRecording;
        private ActionList executeActivities;
        private Guid id;
                private string name;
        
        
        public static event EventHandler<ZappyTaskEventArgs> Saving;

        [Category("Internals")]

        public bool ReuseID { get; set; }

        
        static ZappyTask()
        {

        }
        public ZappyTask()
        {
            name = string.Empty;
            id = Guid.NewGuid();
            assemblyVersion = AssemblyFileVersion;
                        executeActivities = new ActionList();
                                    ActionDictionary = new Dictionary<Guid, IZappyAction>();
                                            }

        public ZappyTask(IEnumerable<IZappyAction> executeActivities) : this(executeActivities, null)
        {
        }

        public ZappyTask(IEnumerable<IZappyAction> executeActivities, ScreenIdentifier map) : this()
        {
                                                            Append(executeActivities);
        }

        public ZappyTask(ActionList nestedExecuteActivities, Collection<ScreenIdentifier> nestedScreenIdentifiers) : this()
        {
            this.ExecuteActivities = nestedExecuteActivities;
                    }

        [XmlElement]
        public string FilePath { get; set; }

        [Category("Internals")]
        public int ZappyExecutionTimeMillisecs { get; set; }

        [Category("Internals")]
        public int ManualExecutionTimeMillisecs { get; set; }


        public int Append(IEnumerable<IZappyAction> actions, ScreenIdentifier map)
        {
            int num = ExecuteActivities.AddRange(actions);
                                                                                    foreach (IZappyAction action in actions)
            {
                if (action is ZappyTaskAction zaction)
                {

                    if (!string.IsNullOrEmpty(zaction.TaskActivityIdentifier))
                    {
                                                                                                                                                                                                                                                                        CrapyLogger.log.Error("Null - zaction.TaskActivityIdentifier");
                        
                    }
                }
            }
            
            return num;
        }


        public int
            Append(IEnumerable<IZappyAction> actions)         {
            ZappyTaskUtilities.CheckForNull(actions, "actions");
            bool flag2 = false;

            foreach (IZappyAction action in actions)
            {
                if (action is ZappyTaskAction zaction)
                {
                                                                                                    
                                                                                                                        if (zaction.WindowIdentifier == null)
                    {
                        zaction.WindowIdentifier = new ScreenIdentifier
                        {
                            Id = "Desktop"
                        };
                        zaction.WindowIdentifier.AddUIObjects(zaction.GetUIElementCollection());
                        zaction.FixUIObjectName(zaction.WindowIdentifier.UniqueNameDictionary);
                    }

                                                                                                                                        }
                flag2 = true;
            }
                                                                                                                                                                                                                                                                                                                                                                                    if (flag2)
            {
                containsRecording = true;
            }
            return ExecuteActivities.AddRange(actions);
        }



        public static Collection<SettingMismatch> CompareEnvironments(ZappyTaskEnvironment source, ZappyTaskEnvironment target)
        {
            
            Collection<SettingMismatch> mismatchedSettings = null;
            if (source != null)
            {
                mismatchedSettings = source.Compare(target);
                LogMismatchedSettings(mismatchedSettings);
            }
            return mismatchedSettings;
        }


        public static ZappyTask Create(Stream reader)
        {
            try
            {
                ZappyTaskUtilities.CheckForNull(reader, "uiTaskReader");
                if (reader.CanSeek)
                {
                                        reader.Seek(0L, SeekOrigin.Begin);
                }
                else
                {
                    CrapyLogger.log.Error("ZappyTask.Create() - Could not validate input uitask xml.");
                }

                ZappyTask _ZappyTask = ActionListSerializer.Deserialize(reader);
                
                return _ZappyTask;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read the Zappy Task");
                CrapyLogger.log.Error(ex);

                throw ex;
            }
        }


        public static ZappyTask Create(string uitaskFile, PgpPrivateKey CustomEncryptionKey = null)
        {
            ZappyTaskUtilities.CheckForNull(uitaskFile, "uitaskFile");

            {
                using (StreamReader reader = ZappyTaskUtilities.GetStreamReader(uitaskFile))
                {
                    return Create(reader.BaseStream);
                }
            }
        }





        public bool Equals(ZappyTask uiTask)
        {
            if (uiTask == null)
            {
                return false;
            }
            if (this != uiTask)
            {
                if (containsRecording)
                {
                    return false;
                }

                if (!ExecuteActivities.Equals(uiTask.ExecuteActivities))
                {
                    return false;
                }

            }
            return true;
        }

        public override bool Equals(object other)
        {
            ZappyTask uiTask = other as ZappyTask;
            return Equals(uiTask);
        }

                                        
                
                
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public IZappyAction LastPlaybackAction { get; set; }

                                                
                
                
                                                
                                                                                                
        public override int GetHashCode()
        {
            int num = 0;
                                    num ^= ExecuteActivities.GetHashCode();
            return num;        }

        private static void LogMismatchedSettings(Collection<SettingMismatch> mismatchedSettings)
        {
            if (mismatchedSettings != null)
            {
                foreach (SettingMismatch mismatch in mismatchedSettings)
                {
                    object[] args = { mismatch.Message };
                    
                }
            }
        }

        public void Save(Stream stream, string uitaskFile, bool removeTextValues = true, bool changeFileName = true)
        {
                        if (changeFileName)
                this.FilePath = uitaskFile;

            if (ManualExecutionTimeMillisecs == 0)
            {
                List<IZappyAction> _Activities = executeActivities.Activities;
                DateTime _MinTimeStamp = DateTime.MaxValue, _MaxTimeStamp = DateTime.MinValue;
                for (int i = 0; i < _Activities.Count; i++)
                {
                    DateTime _TimeStamp = _Activities[i].Timestamp;
                    if (_TimeStamp > _MaxTimeStamp)
                        _MaxTimeStamp = _TimeStamp;

                    if (_TimeStamp < _MinTimeStamp)
                        _MinTimeStamp = _TimeStamp;
                }

                ManualExecutionTimeMillisecs = (int)((_MaxTimeStamp - _MinTimeStamp).Ticks / TimeSpan.TicksPerMillisecond);
            }

            ZappyTaskUtilities.CheckForNull(stream, "writer");
            

            Saving?.Invoke(this, new ZappyTaskEventArgs(this));

            BeforeSerialize(removeTextValues);

            ActionListSerializer.Serialize(stream, this);
        }

        public void BeforeSerialize(bool removeTextValues)
        {
            List<IZappyAction> _Activities = executeActivities.Activities;
            bool _StartFound = false;

            for (int i = 0; i < _Activities.Count; i++)
            {
                if (_Activities[i] is StartNodeAction)
                {
                    _StartFound = true;
                    break;
                }
            }

            if (!_StartFound)
            {
                _Activities.Insert(0, new StartNodeAction());
                _Activities.Add(new EndNodeAction());
            }

            for (int i = 0; i < _Activities.Count; i++)
            {
                if (_Activities[i].SelfGuid == Guid.Empty)
                    _Activities[i].SelfGuid = Guid.NewGuid();
            }


            for (int i = 0; i < _Activities.Count; i++)
            {
                                if (_Activities[i].NextGuid == Guid.Empty && (i + 1) != _Activities.Count
                                                          && !(_Activities[i] is EndNodeAction))
                    _Activities[i].NextGuid = _Activities[i + 1].SelfGuid;

                                
                if (_Activities[i] is IUserTextAction)
                    (_Activities[i] as IUserTextAction).Cleanup();
            }

                                                                                }

                
                                                        


        public void Save(string uitaskFile, PgpPublicKey CustomPublicKey = null, bool changeFileName = true)
        {
            using (Stream fs = File.Open(uitaskFile, FileMode.Create))
            {
                Save(fs, uitaskFile, false, changeFileName);
            }

        }

        public void SaveUnencrypted(string uitaskFile)
        {
            using (Stream fs = File.Open(uitaskFile, FileMode.Create))
            {
                Save(fs, uitaskFile, false);
            }
        }
                                                                                                                                                                                                
                                                                                
                        
                                
                                                                                                                                                                        
        [XmlAttribute]
        [Category("Internals")]

        public string AssemblyVersion
        {
            get =>
                assemblyVersion;
            set
            {
                assemblyVersion = value;
            }
        }

                                                                        
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Category("Internals")]
        [Browsable(false)]
        public ZappyTaskEnvironment Configuration
        {
            get
            {
                if (configuration == null)
                {
                    configuration = ZappyTaskEnvironment.SystemEnvironment;
                }
                return configuration;
            }
            set
            {
                configuration = value;
            }
        }

                                                
                                                                                                                                        
        
                                                                                                        
                


        [Category("Internals")]
        [Browsable(false)]
        public ActionList ExecuteActivities
        {
            get =>
                executeActivities;
            set
            {
                executeActivities = value;
            }
        }

        [XmlAttribute]
        [Category("Internals")]
        [Browsable(false)]
        public Guid Id
        {
            get =>
                id;
            set
            {
                id = value;
            }
        }



        [XmlAttribute]
        public string Name
        {
            get =>
                name;
            set
            {
                name = value;
            }
        }

                                                                                        
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public Dictionary<Guid, IZappyAction> ActionDictionary { get; set; }

    }
}
