using System;
using System.Threading;
using Zappy.ZappyActions.Triggers;
using ZappyMessages;
using ZappyMessages.Helpers;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.ExecuteTask
{
    public class ZappyPlaybackCommunicator : IPubSubSubscriber
    {
        public static void Init()
        {
            Instance = new ZappyPlaybackCommunicator(PubSubService.Instance);
        }
        public static void Init(PubSubClient Client)
        {
            Instance = new ZappyPlaybackCommunicator(Client);
            Client.DataPublished += ((ZappyPlaybackCommunicator)Instance).OnPublished;
        }

                
        ManualResetEventSlim _mre;
        string _Response;
        public IPubSubService _PubSub;
        int _ResponseChannel = 0;
                                internal static ZappyPlaybackCommunicator Instance { get; private set; }

                                public ZappyPlaybackCommunicator(IPubSubService PubSub)
        {           
            _mre = new ManualResetEventSlim(false);
            _PubSub = PubSub;
        }

        int _ResponseTimeout = 5000;

        internal string GetLastActivityTime(int _RequestChannel, int ResponseChannel)
        {
            lock (Instance)
            {
                _ResponseChannel = ResponseChannel;
                Tuple<PlayBackHelperRequestEnum, string> _Request = new Tuple<PlayBackHelperRequestEnum, string>
                    (PlayBackHelperRequestEnum.GetLastActivityTime, string.Empty);

                _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
                _mre.Reset();
                if (!_mre.Wait(_ResponseTimeout))
                    throw new Exception("Unable to register trigger");
                return _Response;
            }
        }

        internal string RegisterWindowLaunchTrigger(int _RequestChannel, int ResponseChannel, WindowLaunchTriggerHelper trigger)
        {
            lock (Instance)
            {
                _ResponseChannel = ResponseChannel;
                Tuple<PlayBackHelperRequestEnum, string> _Request = new Tuple<PlayBackHelperRequestEnum, string>
                    (PlayBackHelperRequestEnum.RegisterWindowLaunchTriiger, ZappySerializer.SerializeObject(trigger));
                _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
                _mre.Reset();
                if (!_mre.Wait(_ResponseTimeout))
                    throw new Exception("Unable to register trigger");
                return _Response;
            }
        }

        internal string UnRegisterWindowLaunchTrigger(int _RequestChannel, int ResponseChannel, WindowLaunchTriggerHelper trigger)
        {
            lock (Instance)
            {
                _ResponseChannel = ResponseChannel;
                Tuple<PlayBackHelperRequestEnum, string> _Request = new Tuple<PlayBackHelperRequestEnum, string>
                    (PlayBackHelperRequestEnum.UnRegisterWindowLaunchTriiger, ZappySerializer.SerializeObject(trigger));
                _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
                _mre.Reset();
                if (!_mre.Wait(_ResponseTimeout))
                    throw new Exception("Unable to register trigger");
                return _Response;
            }
        }

        internal void PublishResponseToZappyPlayback(string message, int channelToReply)
        {
            lock (Instance)
            {
                _PubSub.Publish(channelToReply, message);               
            }
        }

        internal void PublishWindowLaunchRequestToZappyPlayback(WindowLaunchTriggerHelper trigger)
        {
            lock (Instance)
            {
                Tuple<PlayBackHelperRequestEnum, string> _Request = new Tuple<PlayBackHelperRequestEnum, string>
                    (PlayBackHelperRequestEnum.WindowLaunchTriggerFire, ZappySerializer.SerializeObject(trigger));
                _PubSub.Publish(PubSubTopicRegister.Zappy2ZappyPlaybackRequest, ZappySerializer.SerializeObject(_Request));
            }
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
