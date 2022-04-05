using System.Activities;
using System.ComponentModel;
using System.Linq;

namespace Zappy.ZappyActions.Record
{
    public class StopRecording : NativeActivity
    {
                        public OutArgument<string> Filename { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
                                                                                                        }
        public new string DisplayName
        {
            get
            {
                var displayName = base.DisplayName;
                if (displayName == this.GetType().Name)
                {
                    var displayNameAttribute = this.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
                    if (displayNameAttribute != null) displayName = displayNameAttribute.DisplayName;
                }
                return displayName;
            }
            set
            {
                base.DisplayName = value;
            }
        }
    }
}