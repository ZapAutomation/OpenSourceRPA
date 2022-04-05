using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Picture;
using Zappy.ZappyActions.Picture.Helpers;

namespace Zappy.Decode.Hooks.Keyboard
{


    [Serializable]
    public abstract class ZappyTaskAction : TemplateAction, INotifyPropertyChanged
    {
        protected ZappyTaskAction()
        {
                        additionalInfo = string.Empty;
            needFiltering = true;
            TimeSpan span = new TimeSpan(WallClock.Now.Ticks);
            startTimestamp = endTimestamp = (long)span.TotalMilliseconds;
            actualThinkTime = -1;
            parameterName = string.Empty;
                        NumberOfRetries = 3;
        }

        protected ZappyTaskAction(TaskActivityElement zappyElement) : this()
        {
            ZappyTaskUtilities.CheckForNull(zappyElement, "uiElement");
            ActivityElement = zappyElement;
        }
        [Category("Internals")]
        [Description("Not used in excution of the activity")]
        public string ZappyWindowTitle { get; set; }
        private int actualThinkTime;
        private string additionalInfo;
        private string comment;
        private bool continueOnError;
        private long endTimestamp;
        [NonSerialized]
        private ZappyTaskImageEntry imageEntry;
        private bool isParameterizedBound;
        private bool needFiltering;
        private string parameterName;
        private long startTimestamp;
        private IDictionary<string, object> tags;
        private int thinkTime;
        [NonSerialized]
        private TaskActivityElement uiElement;
        private string zappyObjectName;
        private string zappyWindowTitleName = "";
        private ZappyTaskActionExtension zappyTaskActionExtension;

        public event PropertyChangedEventHandler PropertyChanged;
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public virtual string ActionName
        {
            get =>
                GetType().Name;
            set
            {
                throw new NotSupportedException();
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public virtual string AdditionalInfo
        {
            get =>
                additionalInfo;
            set
            {
                additionalInfo = value;
                NotifyPropertyChanged("AdditionalInfo");
            }
        }

        [XmlAttribute]
        [Browsable(false)]
        public string Comment
        {
            get =>
                comment;
            set
            {
                comment = value;
                NotifyPropertyChanged("Comment");
            }
        }


        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public long EndTimestamp
        {
            get =>
                endTimestamp;
            set
            {
                endTimestamp = value;
                if (StartTimestamp > endTimestamp)
                {
                    StartTimestamp = endTimestamp;
                }
            }
        }


        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        internal ZappyTaskImageEntry ImageEntry
        {
            get =>
                imageEntry;
            set
            {
                if (imageEntry != null)
                {
                    imageEntry.PropertyChanged -= ImageEntry_PropertyChanged;
                }
                imageEntry = value;
                if (imageEntry != null)
                {
                    imageEntry.PropertyChanged += ImageEntry_PropertyChanged;
                    NotifyPropertyChanged("ImageEntry");
                }
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public bool IsParameterBound
        {
            get
            {
                if (!IsParameterizable)
                {
                                    }
                return isParameterizedBound;
            }
            set
            {
                if (!IsParameterizable)
                {
                    throw new NotSupportedException();
                }
                isParameterizedBound = value;
                NotifyPropertyChanged("IsParameterBound");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public virtual bool IsParameterizable =>
            false;

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        internal bool IsParameterized =>
            !string.IsNullOrEmpty(ParameterName);

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public virtual bool NeedFiltering
        {
            get =>
                needFiltering;
            set
            {
                needFiltering = value;
                NotifyPropertyChanged("NeedFiltering");
            }
        }

        [Browsable(false)]
        public string ParameterName
        {
            get =>
                parameterName;
            set
            {
                parameterName = value;
                if (IsParameterizable)
                {
                    IsParameterBound = false;
                }
                NotifyPropertyChanged("ParameterName");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public long StartTimestamp
        {
            get =>
                startTimestamp;
            set
            {
                startTimestamp = value;
                if (EndTimestamp < startTimestamp)
                {
                    EndTimestamp = startTimestamp;
                }
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public IDictionary<string, object> Tags
        {
            get
            {
                if (tags == null)
                {
                    tags = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }
                return tags;
            }
            private set
            {
                tags = value;
                NotifyPropertyChanged("Tags");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public int ThinkTime
        {
            get =>
                thinkTime;
            set
            {
                thinkTime = value;
                NotifyPropertyChanged("ThinkTime");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public TaskActivityElement ActivityElement
        {
            get =>
                uiElement;
            set
            {
                uiElement = value;
                NotifyPropertyChanged("UIElement");
            }
        }
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public TaskActivityElement CopyActivityElementWithoutNotifier
        {
            get =>
                uiElement;
            set
            {
                uiElement = value;
            }
        }

                [Category("Internals")]
        public string TaskActivityIdentifier
        {
            get =>
                zappyObjectName;
            set
            {
                zappyObjectName = value;
                NotifyPropertyChanged("TaskActivityIdentifier");
            }
        }


        [Category("Optional")]
        [Description("Specifie value the TaskActivityIdentifier is dynamic and changes at runtime." +
            " Overrides TaskActivityIdentifier property")]
        public DynamicProperty<string> DynamicTaskActivityIdentifier {get;set;}

        [Category("Optional")]
        [Description("Specifie value the TopLevelWindowTitle is dynamic and changes at runtime.")]
        public DynamicProperty<string> DynamicTopLevelWindowTitle { get; set; }

                
        [XmlElement]
        [Browsable(false)]
        public ZappyTaskActionExtension ZappyTaskActionExtension
        {
            get
            {
                if (zappyTaskActionExtension == null && ImageEntry != null && ImageEntry.ImageInformation != null)
                {
                    zappyTaskActionExtension = new ZappyTaskActionExtension();
                    PropertyCondition[] conditions = { new PropertyCondition(ZappyTaskActionExtension.ImagePathProperty, ImageEntry.ImageInformation.ImagePath), new PropertyCondition(ZappyTaskActionExtension.InteractionPoint, ImageEntry.InteractionPoint.ToString()), new PropertyCondition(ZappyTaskActionExtension.SnapShotTimeStamp, ImageEntry.ImageInformation.SnapShotTimeStamp.ToString(CultureInfo.InvariantCulture)) };
                    zappyTaskActionExtension.ExtendedProperties = new AndCondition(conditions);
                }
                return zappyTaskActionExtension;
            }
            set
            {
                zappyTaskActionExtension = value;
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Category("Internals")]
        [Browsable(false)]
        public virtual string ValueAsString
        {
            get =>
                GetParameterString();
            set
            {
                throw new NotSupportedException();
            }
        }

        [Category("Optional")]
        [Description("Specifie value the WindowIdentifier is dynamic and changes at runtime." +
    " Overrides WindowIdentifier property")]
        public DynamicProperty<ScreenIdentifier> DynamicWindowIdentifier { get; set; }

                [Category("Internals")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ScreenIdentifier WindowIdentifier { get; set; }

        [Category("Internals")]
        [Browsable(false)]
        public string ImageData { get; set; }

        [Editor(typeof(ImageViewer), typeof(UITypeEditor))]
                [Category("Internals")]
        [XmlIgnore, JsonIgnore]
        public ImageObject Screenshot {
            get
            {
                ImageObject img = new ImageObject();
                if (!string.IsNullOrEmpty(ImageData))
                {
                    byte[] bytes = Convert.FromBase64String(ImageData);
                    Image ImageFromData;
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        ImageFromData = Image.FromStream(ms);
                    }
                    img.PatternFile = (Bitmap)ImageFromData;
                }
                return img;
            }
            set
            {
            }
        }

                
        
                                        
        public virtual void BindParameter(ValueMap valueMap, ControlType controlType)
        {
            if (IsParameterizable)
            {
                throw new NotSupportedException();
            }
        }

        public virtual void BindWithCurrentValues()
        {
            if (IsParameterizable && IsParameterized)
            {
                IsParameterBound = true;
            }
        }

        public void CleanupPropertyChangeHandler()
        {
            if (imageEntry != null)
            {
                imageEntry.PropertyChanged -= ImageEntry_PropertyChanged;
            }
        }

        public override bool Equals(object other)
        {
            ZappyTaskAction action = other as ZappyTaskAction;
            bool flag = false;
            if (ReferenceEquals(action, null))
                return flag;
            flag = Id == action.Id;
            if (!flag || string.IsNullOrEmpty(TaskActivityIdentifier) && string.IsNullOrEmpty(action.TaskActivityIdentifier))
            {
                return flag;
            }
            return string.Equals(TaskActivityIdentifier, action.TaskActivityIdentifier, StringComparison.Ordinal);
        }

        
        internal virtual TopLevelElement FixUIObjectName(IDictionary<ITaskActivityElement, string> uniqueNameDictionary)
        {
            ZappyTaskUtilities.CheckForNull(uniqueNameDictionary, "uniqueNameDictionary");
            if (uiElement != null)
            {
                string str;
                if (uniqueNameDictionary.TryGetValue(uiElement, out str))
                {
                    zappyObjectName = str;
                    if (uiElement.TopLevelElement != null)
                    {
                                                if (uiElement.TopLevelElement.Framework != null && uiElement.TopLevelElement.Framework != "WPF")
                        {
                            PauseTimeAfterAction = 100;
                        }
                        TopLevelWindowHandle = uiElement.TopLevelElement.WindowHandle;
                        if (string.IsNullOrEmpty(this.ExeName))
                        {
                            uint _Pid;
                            NativeMethods.GetWindowThreadProcessId(TopLevelWindowHandle, out _Pid);
                            this.ExeName = Path.GetFileName(NativeMethods.GetProcessFileName((int)_Pid));
                        }
                        ZappyWindowTitle = uiElement.TopLevelElement.Name;
                    }
                    ElementWindowHandle = uiElement.WindowHandle;
                                                                                                                                                                                                                                                                }
                else
                {
                                                        }
            }
                        
                                                                                                
            return null;
        }
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public IntPtr TopLevelWindowHandle { get; set; }
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public IntPtr ElementWindowHandle { get; set; }

        public override int GetHashCode() =>
            Id.GetHashCode();

        internal virtual string GetParameterString() =>
            string.Empty;

        internal virtual ICollection<TaskActivityElement> GetUIElementCollection()
        {
            if (uiElement != null)
            {
                return new[] { uiElement };
            }
            return new TaskActivityElement[0];
        }


        private void ImageEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            zappyTaskActionExtension = null;
            NotifyPropertyChanged("ImageEntry");
        }

        internal virtual bool MatchParameter(string specifiedParameter) =>
            !string.IsNullOrEmpty(specifiedParameter) && string.Equals(ValueAsString.Trim(), specifiedParameter, StringComparison.Ordinal);

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static bool operator ==(ZappyTaskAction left, ZappyTaskAction right)
        {
            if (!ReferenceEquals(left, null))
            {

                return left.Equals(right);
            }
            return ReferenceEquals(right, null);
                    }

        public static bool operator !=(ZappyTaskAction left, ZappyTaskAction right)
        =>
        !(left == right);


        internal virtual void ShallowCopy(ZappyTaskAction source, bool isSeparateAction)
        {
            if (!isSeparateAction)
            {
                Id = source.Id;
                StartTimestamp = source.StartTimestamp;
                Timestamp = source.Timestamp;
            }
            ActivityElement = source.ActivityElement;
            Comment = source.Comment;
            ContinueOnError = source.ContinueOnError;
            Tags = source.Tags;
            ImageEntry = source.ImageEntry;
                        ExeName = source.ExeName;
            TaskActivityIdentifier = source.TaskActivityIdentifier;
        }

        public override string ToString()
        {
            if (ActivityElement == null)
            {
                object[] objArray1 = { Id, ActionName, GetParameterString() };
                return string.Format(CultureInfo.InvariantCulture, "Id={0}, Action={1}, Parameters={2}", objArray1);
            }
            if (ActivityElement.IsPassword)
            {
                object[] objArray2 = { Id, ActionName, ActivityElement };
                return string.Format(CultureInfo.InvariantCulture, "Id={0}, Action={1}, UITechnologyElement=[{2}]", objArray2);
            }
            object[] args = { Id, ActionName, GetParameterString(), ActivityElement };
            return string.Format(CultureInfo.InvariantCulture, "Id={0}, Action={1}, Parameters={2}, UITechnologyElement=[{3}]", args);
        }

        internal virtual void ValidateParameter()
        {
            if (IsParameterized && !IsParameterBound)
            {
                object[] args = { ParameterName };
                                                                                                Exception exception = null;
                throw exception;
            }
        }

        internal void updateWindowAndTaskActivityIdentifier()
        {
            if (!string.IsNullOrWhiteSpace(DynamicTaskActivityIdentifier))
            {
                TaskActivityIdentifier = DynamicTaskActivityIdentifier;
            }
            if (DynamicWindowIdentifier != null && DynamicWindowIdentifier.Value != null)
            {
                WindowIdentifier = DynamicWindowIdentifier;
            }
            if (!string.IsNullOrWhiteSpace(DynamicTopLevelWindowTitle) && WindowIdentifier != null)
            {
                WindowIdentifier.TopLevelWindows[0].WindowTitles[0] = DynamicTopLevelWindowTitle;
            }
        }
    }
}
