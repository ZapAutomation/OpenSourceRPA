using System;
using System.Drawing;
using Zappy.ActionMap.ElementManager;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Hooks.LowLevelHookEvent
{
    internal abstract class EventCaptureBase : IEventCapture
    {
        protected IEventCapture accessoryEventCapture;
        protected IZappyActionStack actions;        protected IElementManager elementManager;
        private Exception eventCaptureException;
        protected readonly object lockObject = new object();
        protected volatile bool paused;
        protected RecorderOptions recorderOptions;

                public event EventHandler<EventArgs> Started;

        public EventCaptureBase(IEventCapture accessoryEventCapture)
        {
            this.accessoryEventCapture = accessoryEventCapture;
        }

                                                                                                                                                                                        
                        
                                
                        
                                
        bool IEventCapture.SetTrackingElement(TaskActivityElement element, Point point, bool alwaysTakeSnapshot) =>
            false;

                        
                        
        protected virtual void OnStarted(object sender, EventArgs args)
        {
            Started?.Invoke(this, args);
        }

        protected Exception EventCaptureException
        {
            get =>
                eventCaptureException;
            set
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    eventCaptureException = value;
                }
            }
        }

        IElementManager IEventCapture.ElementManager
        {
            get =>
                elementManager;
            set
            {
                elementManager = value;
            }
        }

        IZappyActionStack IEventCapture.RawActionList
        {
            get =>
                actions;
            set
            {
                actions = value;
            }
        }

        RecorderOptions IEventCapture.RecorderOptions
        {
            get =>
                recorderOptions;
            set
            {
                recorderOptions = value;
            }
        }

        protected bool Paused = false;

    }
}
