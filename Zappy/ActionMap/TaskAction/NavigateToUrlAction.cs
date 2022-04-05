using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml.Serialization;
using Crapy.ActionMap.HelperClasses;
using Crapy.ActionMap.ScreenMaps;
using Crapy.ActionMap.ZappyTaskUtil;
using Crapy.Decode.Aggregator;
using Crapy.Decode.Helper;
using Crapy.ExecuteTask.Execute;
using Crapy.ExecuteTask.Helpers;
using Crapy.Properties;
using Crapy.ZappyTaskEditor;

namespace Crapy.ActionMap.TaskAction
{
    [Serializable]
    public class NavigateToUrlAction : AggregatedAction
    {
        private const string bingSearchString = "http://www.bing.com/search?q={0}";
        private bool newInstance;
        private Uri url;
        private const string webUrlString = "http://{0}";

        public NavigateToUrlAction()
        {
        }

        public NavigateToUrlAction(Uri url)
        {
            ZappyTaskUtilities.CheckForNull(url, "url");
            Url = url;
        }

        public override void BindParameter(ValueMap valueMap, ControlType controlType)
        {
            if (IsParameterized)
            {
                object parameterValue = null;
                if (valueMap.TryGetParameterValue(ParameterName, out parameterValue) && parameterValue != null)
                {
                    UrlString = parameterValue.ToString();
                }
            }
            BindWithCurrentValues();
        }

        internal override string GetParameterString() =>
            UrlString;


        public static Uri GetUri(string probableUri)
        {
            Uri uri;
            if (!string.IsNullOrEmpty(probableUri) && probableUri.StartsWith("//", StringComparison.Ordinal))
            {
                probableUri = probableUri.Substring(2);
            }
            if (!string.IsNullOrEmpty(probableUri) && !probableUri.StartsWith(@"\\", StringComparison.Ordinal))
            {
                probableUri = probableUri.Replace(@"\", "/");
            }
            try
            {
                uri = new Uri(probableUri);
            }
            catch (UriFormatException)
            {
                try
                {
                    object[] args = { probableUri };
                    uri = new Uri(string.Format(CultureInfo.InvariantCulture, "http://{0}", args));
                }
                catch (UriFormatException exception)
                {
                    object[] objArray2 = { probableUri };
                    throw new ZappyTaskException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidUriFormatError, objArray2), exception);
                }
            }
            return uri;
        }

        public override void Invoke(ZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker, ScreenIdentifier map)
        {
            //actionInvoker.Invoke(this, map);
            //object[] args = { ZappyTaskInterpreterCore.ActionDetailsMessage(action) };
            //CrapyLogger.log.InfoFormat("UIActionInterpreter.Invoke(): {0}", args);

            BrowserWindow uiControl = null;
            if (!this.NewInstance)
            {
                uiControl = UIActionInterpreter.GetZappyTaskControl(this.TaskActivityIdentifier, map) as BrowserWindow;
                if (uiControl != null)
                {
                    try
                    {
                        uiControl.NavigateToUrl(this.Url);
                    }
                    catch (Exception exception)
                    {
                        object[] objArray2 = { this.TaskActivityIdentifier };
                        //CrapyLogger.log.WarnFormat("Could not find window {0}. Starting new instance.", objArray2);

                        object[] objArray3 = { exception.Message };
                        //CrapyLogger.log.WarnFormat("Detail error: {0}", objArray3);

                    }
                }
            }
            bool flag = false;
            if (uiControl == null || !uiControl.IsBound)
            {
                uiControl = BrowserWindow.Launch(this.Url);
                flag = true;
            }
            uiControl.CloseOnPlaybackCleanup = false;
            if (flag)
            {
                if (!string.IsNullOrEmpty(this.TaskActivityIdentifier))
                {
                    TaskActivityObject uIObjectFromUIObjectId = map.GetUIObjectFromUIObjectId(this.TaskActivityIdentifier);
                    uiControl.SessionId = uIObjectFromUIObjectId.SessionId;
                    UIActionInterpreter.UpdateControlsCache(this.TaskActivityIdentifier, uiControl);
                    SearchHelper.Instance.UpdateTopLevelElementCache(new ZappyTaskControlSearchArgument(uiControl), uiControl);
                }
                else
                {
                    uiControl.Dispose();
                }
            }
        }

        internal override bool MatchParameter(string specifiedParameter)
        {
            if (string.IsNullOrEmpty(specifiedParameter))
            {
                return false;
            }
            Uri objB = null;
            Uri objA = null;
            try
            {
                objB = GetUri(specifiedParameter);
                objA = GetUri(ValueAsString.Trim());
            }
            catch (UriFormatException)
            {
                return false;
            }
            catch (ZappyTaskException exception)
            {
                if (!(exception.InnerException is UriFormatException))
                {
                    throw;
                }
                return false;
            }
            return Equals(objA, objB);
        }

        [XmlIgnore]
        public override bool IsParameterizable =>
            true;

        public bool NewInstance
        {
            get =>
                newInstance;
            set
            {
                newInstance = value;
                NotifyPropertyChanged("NewInstance");
            }
        }

        [XmlIgnore]
        public Uri Url
        {
            get =>
                url;
            set
            {
                url = value;
                NotifyPropertyChanged("Url");
            }
        }

        [XmlElement("Url"), ]
        public string UrlString
        {
            get
            {
                if (Url == null)
                {
                    return string.Empty;
                }
                if (IsParameterized && !IsParameterBound)
                {
                    return string.Empty;
                }
                return Url.ToString();
            }
            set
            {
                Url = GetUri(value);
                NotifyPropertyChanged("UrlString");
            }
        }
    }
}