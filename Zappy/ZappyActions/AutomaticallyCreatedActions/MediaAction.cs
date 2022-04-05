using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Aggregator;
using Zappy.Decode.Helper;
using Zappy.Decode.Helper.Enums;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Extension.HtmlControls;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Serializable]
    [Description(" Media Related Activities Like Set,Mute,Volume,etc")]
    public class MediaAction : AggregatedAction
    {
        private bool mute;
        private bool muteSet;
        private TimeSpan time;
        private bool timeSet;
        private float volume;
        private bool volumeSet;

        public MediaAction()
        {
        }

        public MediaAction(TaskActivityElement element, ZappyTaskMediaEventInfo eventArgs) : base(element)
        {
            if (eventArgs != null && eventArgs.Value != null)
            {
                ActionType = eventArgs.ActionType;
                SetValue(ActionType, eventArgs.Value);
            }
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ZappyTaskControl uITaskControl = UIActionInterpreter.GetZappyTaskControl(this.TaskActivityIdentifier, WindowIdentifier);
            HtmlMedia specializedControl = ZappyTaskControl.GetSpecializedControl(uITaskControl) as HtmlMedia;
            if (specializedControl != null)
            {
                ExecutionHandler.PlaybackContext = new InterpreterPlaybackContext(WindowIdentifier, this, uITaskControl);
                switch (this.ActionType)
                {
                    case MediaActionType.Play:
                        specializedControl.Play(this.Time);
                        return;

                    case MediaActionType.Pause:
                        specializedControl.Pause(this.Time);
                        return;

                    case MediaActionType.Seek:
                        specializedControl.Seek(this.Time);
                        return;

                    case MediaActionType.VolumeChange:
                        specializedControl.Unmute();
                        specializedControl.SetVolume(this.Volume);
                        return;

                    case MediaActionType.Mute:
                        if (!this.Mute)
                        {
                            specializedControl.Unmute();
                            return;
                        }
                        specializedControl.Mute();
                        return;
                }
                object[] objArray2 = { this.ActionType.ToString() };
                throw new ZappyTaskException(string.Format(CultureInfo.CurrentCulture, Resources.ActionNotSupported, objArray2));
            }

        }

        internal void SetValue(MediaActionType actionType, object value)
        {
            switch (actionType)
            {
                case MediaActionType.Play:
                case MediaActionType.Pause:
                case MediaActionType.Seek:
                    Time = value == null ? new TimeSpan(0, 0, 0) : (TimeSpan)value;
                    return;

                case MediaActionType.VolumeChange:
                    Volume = Convert.ToSingle(value, CultureInfo.InvariantCulture);
                    return;

                case MediaActionType.Mute:
                    Mute = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                    return;
            }
        }

        public MediaActionType ActionType { get; set; }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public bool Mute
        {
            get =>
                mute;
            set
            {
                mute = value;
                muteSet = true;
            }
        }

        [XmlElement("Mute")]
        public string MuteWrapper
        {
            get
            {
                if (muteSet)
                {
                    return Mute.ToString();
                }
                return null;
            }
            set
            {
                Mute = bool.Parse(value);
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public TimeSpan Time
        {
            get =>
                time;
            set
            {
                time = value;
                timeSet = true;
            }
        }

        [XmlElement("Time")]
        public string TimeWrapper
        {
            get
            {
                if (timeSet)
                {
                    return Time.ToString();
                }
                return null;
            }
            set
            {
                Time = TimeSpan.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public float Volume
        {
            get =>
                volume;
            set
            {
                volume = value;
                volumeSet = true;
            }
        }

        [XmlElement("Volume")]
        public string VolumeWrapper
        {
            get
            {
                if (volumeSet)
                {
                    return Volume.ToString(CultureInfo.InvariantCulture);
                }
                return null;
            }
            set
            {
                Volume = float.Parse(value, CultureInfo.InvariantCulture);
            }
        }
    }
}