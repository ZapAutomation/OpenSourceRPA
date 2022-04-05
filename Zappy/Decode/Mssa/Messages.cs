using System.ComponentModel;
using System.Globalization;

namespace Zappy.Decode.Mssa
{
    internal class Messages
    {
        private static CultureInfo resourceCulture;


        internal Messages()
        {
        }

        internal static string ActionException =>
            Properties.Resources.ResourceManager.GetString("ActionException", resourceCulture);

        internal static string Common_CrapyLoggerReachedMaxEventLogEntries =>
            Properties.Resources.ResourceManager.GetString("Common_CrapyLoggerReachedMaxEventLogEntries", resourceCulture);

        internal static string ControlBlockedByCurrentProcess =>
            Properties.Resources.ResourceManager.GetString("ControlBlockedByCurrentProcess", resourceCulture);

        internal static string ControlNotFoundException =>
            Properties.Resources.ResourceManager.GetString("ControlNotFoundException", resourceCulture);

        internal static string ControlNotFoundWithQidOrCondition =>
            Properties.Resources.ResourceManager.GetString("ControlNotFoundWithQidOrCondition", resourceCulture);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get =>
                resourceCulture;
            set
            {
                resourceCulture = value;
            }
        }

        internal static string FailedToSendMessage =>
            Properties.Resources.ResourceManager.GetString("FailedToSendMessage", resourceCulture);

        internal static string InvalidDateRange =>
            Properties.Resources.ResourceManager.GetString("InvalidDateRange", resourceCulture);

        internal static string InvalidIUnknownObject =>
            Properties.Resources.ResourceManager.GetString("InvalidIUnknownObject", resourceCulture);

        internal static string InvalidKeySpecified =>
            Properties.Resources.ResourceManager.GetString("InvalidKeySpecified", resourceCulture);

        internal static string MultipleControlsFoundException =>
            Properties.Resources.ResourceManager.GetString("MultipleControlsFoundException", resourceCulture);

                                                                                                        
        internal static string Utility_IgnoredThrownException =>
            Properties.Resources.ResourceManager.GetString("Utility_IgnoredThrownException", resourceCulture);

        internal static string Utility_ProcessNameWhenCannotGetIt =>
            Properties.Resources.ResourceManager.GetString("Utility_ProcessNameWhenCannotGetIt", resourceCulture);

        internal static string WindowNotFoundException =>
            Properties.Resources.ResourceManager.GetString("WindowNotFoundException", resourceCulture);
    }
}