using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using ZappyMessages;
using ZappyMessages.Chrome;
using ZappyMessages.Helpers;
using ZappyMessages.Logger;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace ZappyChromeNativeConsole.ChromeHelper
{
    public static class ChromeNativeService
    {
        static PubSubClient _Client;
        private static Dictionary<long, bool> _OpenUrlSent = new Dictionary<long, bool>();

        public static void InitChromeNativeService()
        {
            EndpointAddress _RemoteAddress = new EndpointAddress(ZappyMessagingConstants.EndpointLocationZappyService);
            _Client = new PubSubClient("PlaybackHelper", _RemoteAddress, new int[] {
              PubSubTopicRegister.ZappyPlaybackHelper2ChromeRequest, PubSubTopicRegister.ControlSignals
            });
            _Client.DataPublished += _Client_DataPublished;
            _Client.ConnectionStatusChanged += _Client_ConnectionStatusChanged;
            if (_Client.IsConnected)
                _Client.Publish(PubSubTopicRegister.ControlSignals, PubSubMessages.RequestStateFromZappy);
        }

        private static void _Client_ConnectionStatusChanged(PubSubClient obj)
        {
            if (_Client.IsConnected)
            {
                Thread.Sleep(500);
                _Client.Publish(PubSubTopicRegister.ControlSignals, PubSubMessages.RequestStateFromZappy);
            }
        }

        private static void _Client_DataPublished(PubSubClient client, int arg1, string arg2)
        {
            //arg2 = StringCipher.Decrypt(arg2, ZappyMessagingConstants.MessageKey);

            switch (arg1)
            {
                case PubSubTopicRegister.ControlSignals:
                    if (arg2 == PubSubMessages.CancelZappyExecutionMessage)
                    {

                    }
                    else if (arg2 == PubSubMessages.StopZappyExecutionMessage)
                    {
                        //CancelTask();
                        BaseChromeAction StopAction = new BaseChromeAction();
                        StopAction.CommandName = "StopRecordingForZappy";
                        ExecuteChromeActionInline(StopAction);
                    }
                    else if (arg2 == PubSubMessages.StartZappyExecutionMessage)
                    {
                        BaseChromeAction StartAction = new BaseChromeAction();
                        StartAction.CommandName = "StartRecordingForZappy";
                        ExecuteChromeActionInline(StartAction);
                    }
                    break;
                case PubSubTopicRegister.ZappyPlaybackHelper2ChromeRequest:
                    BaseChromeAction _Action = ZappySerializer.DeserializeObject<BaseChromeAction>(arg2);
                    _Action.CommandValue = ExecuteChromeActionInline(_Action) ? "SUCCESS" : "FAILED";
                    PublishBaseAction(PubSubTopicRegister.Chrome2ZappyPlaybackHelperResponse, _Action);
                    break;
            }
        }

        public static void PublishChromeAction(DateTime Now, int windowId, int tabId,
                                         string url, string command, List<string> targetModified,
                                         string commandvalue, string windowTitle)
        {
            BaseChromeAction _Action = new BaseChromeAction()
            {
                Timestamp = Now,
                ActionUrlTab = url,
                CommandName = command,
                CommandTarget = targetModified,
                CommandValue = commandvalue,
                SelfGuid = Guid.NewGuid(),
                TabID = tabId,
                WindowID = windowId,
                ActionWindowtitle = windowTitle

            };

            PublishBaseAction(PubSubTopicRegister.Chrome2ZappyActionPlayback, _Action);
        }


        public static void PublishBaseAction(int Channel, BaseChromeAction Action)
        {

            if (Channel != PubSubTopicRegister.Chrome2ZappyPlaybackHelperResponse)
            {
                long _WindowTabIdentifier = Action.WindowID;
                _WindowTabIdentifier = (_WindowTabIdentifier << 32);
                _WindowTabIdentifier =
                    _WindowTabIdentifier | (long)(uint)Action.TabID;
                bool _SentOpenUrl = false;
                if (!_OpenUrlSent.TryGetValue(_WindowTabIdentifier, out _SentOpenUrl) && _Client.IsConnected)
                {
                    if (!Action.CommandName.StartsWith("open"))
                    {
                        BaseChromeAction _OpenAction = new BaseChromeAction() { Timestamp = DateTime.Now, ActionUrlTab = Action.ActionUrlTab, CommandName = "open", CommandTarget = new List<string>() { Action.ActionUrlTab }, CommandValue = string.Empty, SelfGuid = Guid.NewGuid(), TabID = Action.TabID, WindowID = Action.WindowID };

                        if (_Client.IsConnected)
                            try
                            {
                                _Client.Publish(Channel, ZappySerializer.SerializeObject(_OpenAction));
                                _OpenUrlSent[_WindowTabIdentifier] = true;
                            }
                            catch { }
                    }
                }
            }

            if (_Client.IsConnected)
                try { _Client.Publish(Channel, ZappySerializer.SerializeObject(Action)); }
                catch { }
        }

        static bool ExecuteChromeActionInline(BaseChromeAction Action)
        {
            ChromeActionRequest _Request =
                new ChromeActionRequest(Action);
#if DEBUG
            ConsoleLogger.Info("Request In " + _Request.DisplayString());
#endif
            _Request.TimeOutEvent = new AutoResetEvent(false);
            ChromeActionManager.EnqueueOutboundActionToChrome(_Request);
            bool _Satisfied = _Request.TimeOutEvent.WaitOne(40000);
#if DEBUG
            if (!_Satisfied)
                ConsoleLogger.Info("Timedout on " + _Request.DisplayString());
#endif
            _Request.TimeOutEvent = null;
#if DEBUG
            ConsoleLogger.Info("Request Out " + _Request.DisplayString());
#endif
            return string.IsNullOrEmpty(_Request.CommandResult)
                ? false
                : _Request.CommandResult.StartsWith(
                    "success");
        }

    }
}
