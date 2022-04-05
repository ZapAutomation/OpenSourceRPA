using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml.Serialization;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages;
using ZappyMessages.Chrome;
using ZappyMessages.Helpers;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.Plugins.ChromeBrowser.Chrome
{

    [Serializable]
    public class ChromeAction : ZappyTaskAction
    {
        public enum ChromeTarget
        {
            Id,
            Xpath,
            All
        }
                
        public ChromeAction()
        {
            Id = ActionIDRegister.GetUniqueId();
            SelfGuid = Guid.NewGuid();
            Timestamp = WallClock.UtcNow;
            CommandTarget = new List<string>();
            CommandValue = new DynamicTextProperty();
            NumberOfRetries = 3;
        }

        public ChromeAction(string ParamCommandName) : this()
        {
            this.CommandName = ParamCommandName;
        }
        [Category("Optional")]
        [Description("Command name as open,type,mouse click ")]
        public string CommandName { get; set; }

        [Category("Optional")]
        [Description("Target to send the commands Id,Xpath or both ")]
        public ChromeTarget TargetToSend { get; set; }
                
        [Category("Optional")]
        [Description("Specifie value the command target is dynamic and changes at runtime." +
            " Overrides command target property")]
        public DynamicProperty<string> DynamicCommandTarget { get; set; }

        [Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
                "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(CsvConverter))]
        [Category("Input")]
        [Description(" Target Command code like target URL,css code ")]
        public List<string> CommandTarget { get; set; }

        [Category("Input")]
        [Description("Value to send to the page" +
            "Location for Mouse Actions" +
            "Keyboard value for Keyboard Actions" +
            "URL for Open URL action")]
        public DynamicTextProperty CommandValue { get; set; }

        [Category("Optional")]
        [Description("URL for redirection tab")]
        public string ActionUrlTab { get; set; }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public int WindowID { get; set; }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public int TabID { get; set; }

                
        ManualResetEventSlim _ChromeCallbackEvent;
        string _PlaybackResponse;

        [XmlIgnore, JsonIgnore]
        public BaseChromeAction PlaybackResponseObject;
        
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (_ChromeCallbackEvent == null)
                _ChromeCallbackEvent = new ManualResetEventSlim(false);
            _PlaybackResponse = null;
                                    PubSubClient clnt = context.ContextData[CrapyConstants.ExternalActionPublisher] as PubSubClient;

            if (clnt != null)
            {
                string _CommandValue = this.CommandValue;
                List<string> targetSend = new List<string>();
                if (string.IsNullOrWhiteSpace(DynamicCommandTarget) && CommandTarget.Count>0)
                {
                    if (TargetToSend == ChromeTarget.Id)
                    {
                                                targetSend.Add(CommandTarget[0]);
                    }
                    else if (TargetToSend == ChromeTarget.Xpath)
                    {
                                                targetSend.Add(CommandTarget[CommandTarget.Count - 1]);
                    }
                    else
                    {
                                                targetSend = CommandTarget;
                    }
                }
                else
                {
                    targetSend.Add(DynamicCommandTarget);
                }
                if (this is ChromeActionKeyboard)
                {
                                        if (_CommandValue.Contains(CrapyConstants.pasteChar))
                    {
                        string clipBoardChar = string.Empty;
                        if (!CommonProgram.GetTextFromClipboard(out clipBoardChar))
                            throw new Exception("Unable to read clipboard contents!");
                        _CommandValue = _CommandValue.Replace(CrapyConstants.pasteChar, clipBoardChar);
                    }
                }

                BaseChromeAction _BaseAction = new BaseChromeAction()
                {
                    ActionUrlTab = this.ActionUrlTab,
                    CommandName = this.CommandName,
                    CommandTarget = targetSend,
                    CommandValue = _CommandValue,
                    SelfGuid = this.SelfGuid,
                    TabID = this.TabID,
                    WindowID = this.WindowID,
                    Timestamp = this.Timestamp
                };

                string _JsonString = ZappySerializer.SerializeObject(_BaseAction);
                clnt.DataPublished += Clnt_DataPublished;
                _PlaybackResponse = null;
                _ChromeCallbackEvent.Reset();
                  clnt.Publish(PubSubTopicRegister.ZappyPlaybackHelper2ChromeRequest, _JsonString);
                _ChromeCallbackEvent.Wait(10000);
                clnt.DataPublished -= Clnt_DataPublished;

                if (string.IsNullOrEmpty(_PlaybackResponse))
                    _PlaybackResponse = "TIMEOUT";

                if (_PlaybackResponse != CrapyConstants.ChromePlaybackSuccessful)
                {
                    string _error = string.Format("ChromeAction Failed : {0}:{1}:{2}", this.TabID.ToString(),
                                            this.ActionUrlTab, this.CommandName);
                    throw new Exception(_error);
                }
                else
                {
                                                        }

            }
            else
            {
                string _ErrorMessage = string.Format("Can't Replay ChromeAction : {0}:{1}:{2}", this.TabID.ToString(),
                    this.ActionUrlTab, this.CommandName);
                CrapyLogger.log.Error(_ErrorMessage);
                throw new Exception(_ErrorMessage);
            }
        }

        private void Clnt_DataPublished(PubSubClient Client, int arg1, string arg2)
        {
            if (arg1 == PubSubTopicRegister.Chrome2ZappyPlaybackHelperResponse)
            {
                BaseChromeAction _BaseAction = ZappySerializer.DeserializeObject<BaseChromeAction>(arg2);
                _PlaybackResponse = _BaseAction.CommandValue.ToUpper();
                PlaybackResponseObject = _BaseAction;
                _ChromeCallbackEvent.Set();
            }
        }

        public override string AuditInfo()
        {
            string text = this.CommandValue.Value.Length > 3 ? this.CommandValue.Value.Substring(0, 3) + ".." : this.CommandValue.Value;
                        return base.AuditInfo() + " TargetToSend:" + this.TargetToSend +
                   " CommandValue:" + text + " on Url:" + this.ActionUrlTab;
        }
    }
}
