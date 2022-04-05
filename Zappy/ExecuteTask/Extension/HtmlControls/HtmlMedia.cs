using System;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlMedia : HtmlControl
    {
        public HtmlMedia() : this(null)
        {
        }

        public HtmlMedia(ZappyTaskControl parent) : base(parent)
        {
        }

        public virtual void Mute()
        {
            SetProperty(PropertyNames.Muted, true);
        }

        public virtual void Pause()
        {
            SetProperty(PropertyNames.Pause, null);
        }

        public virtual void Pause(TimeSpan pauseTime)
        {
            SetProperty(PropertyNames.Pause, pauseTime);
        }

        public virtual void Play()
        {
            SetProperty(PropertyNames.Play, null);
        }

        public virtual void Play(TimeSpan startTime)
        {
            SetProperty(PropertyNames.Play, startTime);
        }

        public virtual void Seek(TimeSpan seekToTime)
        {
            SetProperty(PropertyNames.CurrentTime, seekToTime);
        }

        public virtual void SetVolume(float volume)
        {
            SetProperty(PropertyNames.Volume, volume);
        }

        public virtual void Unmute()
        {
            SetProperty(PropertyNames.Muted, false);
        }

        public virtual bool AutoPlay =>
            (bool)GetProperty(PropertyNames.AutoPlay);

        public virtual bool Controls =>
            (bool)GetProperty(PropertyNames.Controls);

        public virtual string CurrentSrc =>
            (string)GetProperty(PropertyNames.CurrentSrc);

        public virtual TimeSpan CurrentTime =>
            (TimeSpan)GetProperty(PropertyNames.CurrentTime);

        public virtual string CurrentTimeAsString =>
            (string)GetProperty(PropertyNames.CurrentTimeAsString);

        public virtual TimeSpan Duration =>
            (TimeSpan)GetProperty(PropertyNames.Duration);

        public virtual string DurationAsString =>
            (string)GetProperty(PropertyNames.DurationAsString);

        public virtual bool Ended =>
            (bool)GetProperty(PropertyNames.Ended);

        public virtual bool Loop =>
            (bool)GetProperty(PropertyNames.Loop);

        public virtual bool Muted =>
            (bool)GetProperty(PropertyNames.Muted);

        public virtual bool Paused =>
            (bool)GetProperty(PropertyNames.Paused);

        public virtual float PlaybackRate
        {
            get =>
                (float)GetProperty(PropertyNames.PlaybackRate);
            set
            {
                SetProperty(PropertyNames.PlaybackRate, value);
            }
        }

        public virtual int ReadyState =>
            (int)GetProperty(PropertyNames.ReadyState);

        public virtual bool Seeking =>
            (bool)GetProperty(PropertyNames.Seeking);

        public virtual string Src =>
            (string)GetProperty(PropertyNames.Src);

        public virtual float Volume =>
            (float)GetProperty(PropertyNames.Volume);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string AutoPlay = "AutoPlay";
            public static readonly string Controls = "Controls";

            public static readonly string CurrentSrc = "CurrentSrc";
            public static readonly string CurrentTime = "CurrentTime";
            internal static readonly string CurrentTimeAsFloat = "CurrentTimeAsFloat";
            public static readonly string CurrentTimeAsString = "CurrentTimeAsString";
            public static readonly string Duration = "Duration";
            public static readonly string DurationAsString = "DurationAsString";
            public static readonly string Ended = "Ended";
            public static readonly string Loop = "Loop";
            public static readonly string Muted = "Muted";
            internal static readonly string Pause = "Pause";
            public static readonly string Paused = "Paused";
            internal static readonly string Play = "Play";
            public static readonly string PlaybackRate = "PlaybackRate";
            public static readonly string ReadyState = "ReadyState";
            public static readonly string Seeking = "Seeking";

            public static readonly string Src = "Src";
            public static readonly string Volume = "Volume";
        }
    }
}

