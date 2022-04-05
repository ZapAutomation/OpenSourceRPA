using System;
using System.Threading;
using ZappyMessages;
using ZappyMessages.Helpers;
using ZappyMessages.OutlookMessages;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.Plugins.Outlook
{
                public class OutlookCommunicator : IOutlookZappyTaskCommunication, IPubSubSubscriber
    {
        public static void Init()
        {
            Instance = new OutlookCommunicator(PubSubService.Instance, PubSubTopicRegister.ZappyOutlookRequest, PubSubTopicRegister.ZappyOutlookResponse);
            PubSubService.Instance.Subscribe("Zappy2Outlook", Instance as OutlookCommunicator, new int[] { PubSubTopicRegister.ZappyOutlookResponse });
        }
        public static void Init(PubSubClient Client)
        {
            Instance = new OutlookCommunicator(Client, PubSubTopicRegister.ZappyPlaybackHelper2OutlookRequest, PubSubTopicRegister.Outlook2ZappyPlaybackHelperResponse);
            Client.DataPublished += ((OutlookCommunicator)Instance).OnPublished;

        }

        ManualResetEventSlim _mre;
        string _Response;
        IPubSubService _PubSub;
        readonly int _RequestChannel, _ResponseChannel;

        internal static IOutlookZappyTaskCommunication Instance { get; private set; }

        public OutlookCommunicator(IPubSubService PubSub, int RequestChannel, int ResponseChannel)
        {
            _RequestChannel = RequestChannel;
            _ResponseChannel = ResponseChannel;
            _mre = new ManualResetEventSlim(false);
            _PubSub = PubSub;
        }

        int _RequestCount = 0, _OutlookResponseTimeout = 5000; 
        public string SearchMessage(OutlookMessageInfo messageInfo)
        {
            Tuple<int, OutlookRequest, string> _Request = new Tuple<int, OutlookRequest, string>(_RequestCount++, OutlookRequest.SearchEmail, ZappySerializer.SerializeObject(messageInfo));
            _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
            _mre.Reset();
            if (_mre.Wait(_OutlookResponseTimeout * 30))
            {
                string o = ZappySerializer.DeserializeObject<Tuple<int, object>>(_Response).Item2.ToString();
                return o;
            }
            throw new Exception("Unable To Search Emails");
        }

        public string SearchMessage_OutlookSearch(OutlookMessageInfo messageInfo)
        {
            Tuple<int, OutlookRequest, string> _Request = new Tuple<int, OutlookRequest, string>(_RequestCount++, OutlookRequest.SearchEmail_OutlookSearch, ZappySerializer.SerializeObject(messageInfo));
            _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
            _mre.Reset();
            if (_mre.Wait(_OutlookResponseTimeout * 30))
            {
                string o = ZappySerializer.DeserializeObject<Tuple<int, object>>(_Response).Item2.ToString();
                return o;
            }
            throw new Exception("Unable To Search Emails");
        }

        public string RegisterNewEmailTriggerOutlook(OutlookNewEmailTriggerInfo triggerInfo)
        {
            Tuple<int, OutlookRequest, string> _Request = new Tuple<int, OutlookRequest, string>
                (0, OutlookRequest.NotifyNewMails, ZappySerializer.SerializeObject(triggerInfo));

            _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
            _mre.Reset();
            if (_mre.Wait(_OutlookResponseTimeout * 3))
            {
                string o = ZappySerializer.DeserializeObject<Tuple<int, object>>(_Response).Item2.ToString();
                return o;
            }
            throw new Exception("Unable communicate with outllook for trigger with subject " + triggerInfo.Subject);
        }

        public void OnPublished(PubSubClient clnt, int channel, string PublishedString)
        {
            if (channel == _ResponseChannel)
            {
                _Response = PublishedString;
                _mre.Set();
            }
        }

        public void OnPublishedBinary(int channel, byte[] PublishedBinaryData)
        {

        }

        public void PingClient()
        {

        }

        public void OnPublished(int channel, string PublishedString)
        {
            if (channel == _ResponseChannel)
            {
                _Response = PublishedString;
                _mre.Set();
            }
                    }
    }
}
