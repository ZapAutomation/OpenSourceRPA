using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace ZappyMessages.PubSub
{

    #region WCF Implementaiton
    public class PubSubClient : IPubSubSubscriber, IDisposable, IPubSubService
    {
        public static Timer _ReconnectTimer;
        private static List<PubSubClient> _CLientList;

        static PubSubClient()
        {
            _CLientList = new List<PubSubClient>();
            _ReconnectTimer = new Timer(TimerCallback, null, 5000, 5000);
        }

        static void TimerCallback(object dummy)
        {
            lock (_CLientList)
            {
                for (int i = 0; i < _CLientList.Count; i++)
                {
                    _CLientList[i].CheckAndConnect();
                }
            }
        }


        int[] _SubscriptionChannels;
        public int[] SubscriptionChannels
        {
            get { return _SubscriptionChannels; }
            set { _SubscriptionChannels = value; Subscribe(); }
        }

        PubSubServiceClient PubSubServiceClient { get; set; }

        public object Tag { get; set; }

        string _Name; //_EndpointName,
        System.ServiceModel.Channels.Binding _Binding;
        System.ServiceModel.EndpointAddress _RemoteAddress;

        public event Action<PubSubClient, int, byte[]> BinaryDataPublished;
        public event Action<PubSubClient, int, string> DataPublished;
        public event Action<PubSubClient> ConnectionStatusChanged;


        bool _IsConnected;
        public bool IsConnected
        {
            get { return _IsConnected; }
            private set
            {
                if (_IsConnected == value)
                    return;
                _IsConnected = value;
                ConnectionStatusChanged?.BeginInvoke(this, null, null);
            }
        }

        public PubSubClient(string Name, System.ServiceModel.EndpointAddress RemoteAddress, int[] Channels)
        {
            _Binding = new NetNamedPipeBinding()
            {
                MaxBufferPoolSize = 2147483647,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647
            };
            _RemoteAddress = RemoteAddress;

            //_EndpointName = EndpointName;
            _Name = Name;
            SubscriptionChannels = Channels;
            CheckAndConnect();
            //tries to connect once more
            if (!IsConnected)
                CheckAndConnect();
            if (IsConnected)
                PubSubServiceClient.Ping();

            lock (_CLientList)
                _CLientList.Add(this);
        }

        public PubSubClient(string Name, System.ServiceModel.Channels.Binding Binding, System.ServiceModel.EndpointAddress RemoteAddress, int[] Channels)
        {
            _Binding = Binding;
            _RemoteAddress = RemoteAddress;

            _Name = Name;
            SubscriptionChannels = Channels;
            CheckAndConnect();
            //tries to connect once more
            if (!IsConnected)
                CheckAndConnect();
            if (IsConnected)
                PubSubServiceClient.Ping();

            lock (_CLientList)
                _CLientList.Add(this);
        }



        //public PubSubClient(IPubSubService Service, int[] Channels)
        //{
        //    SubscriptionChannels = Channels;
        //    PubSubServiceClient = Service;
        //}


        public void CheckAndConnect()
        {
            try
            {
                PubSubServiceClient _client = PubSubServiceClient as PubSubServiceClient;

                if (_client == null || _client.InnerDuplexChannel.State != System.ServiceModel.CommunicationState.Opened)
                {
                    //if (_Binding != null)
                        _client = new PubSubServiceClient(new System.ServiceModel.InstanceContext(this), _Binding, _RemoteAddress);
                    //else
                    //    _client = new PubSubServiceClient(new System.ServiceModel.InstanceContext(this), _EndpointName);

                    _client.Open();
                    if (SubscriptionChannels != null)
                        _client.Subscribe(_Name, SubscriptionChannels);
                }


                PubSubServiceClient = _client;


                IsConnected = true;
            }
            catch
            {
                PubSubServiceClient = null;
                IsConnected = false;
            }
        }

        void Subscribe()
        {
            if (IsConnected && PubSubServiceClient != null && SubscriptionChannels != null)
                PubSubServiceClient.Subscribe(_Name, SubscriptionChannels);
        }

        void IPubSubSubscriber.OnPublished(int channel, string publish)
        {
            Task.Factory.StartNew(() => OnPublishedInternal(channel, publish));
        }

        void IPubSubSubscriber.OnPublishedBinary(int channel, byte[] publish)
        {
            Task.Factory.StartNew(() => OnPublishedBinaryInternal(channel, publish));
        }

        void OnPublishedInternal(int channel, string publish)
        {
            //decrypt in channel calls string
            DataPublished?.Invoke(this, channel, publish);
        }

        void OnPublishedBinaryInternal(int channel, byte[] publish)
        {
            BinaryDataPublished?.Invoke(this, channel, publish);
        }


        public void Dispose()
        {
            lock (_CLientList)
            {
                _CLientList.Remove(this);
                try
                {
                    if (PubSubServiceClient != null)
                        PubSubServiceClient.Close();
                }
                catch
                {

                }
            }
        }

        public void PingClient()
        {

        }

        public void Subscribe(string SubscriberName, int[] channels)
        {
            PubSubServiceClient.Subscribe(SubscriberName, channels);
        }

        public void Unsubscribe()
        {
            PubSubServiceClient.Unsubscribe();
        }

        public void Publish(int channel, string PublishedString)
        {
            //encrypt the call string
            //PgpPublicKey CustomPublicKey = _EncryptionKeys.PublicKey;
            //sw = EncryptDecryptUtil.GetEncryptedStreamWriter(ZappyTaskUtilities.GetFileStreamWithCreateReadWriteAccess(Logfilename),
            //    CustomPublicKey);
            PubSubServiceClient.Publish(channel, PublishedString);
        }

        public void PublishBinary(int channel, byte[] PublishedBinaryData)
        {
            PubSubServiceClient.PublishBinary(channel, PublishedBinaryData);
        }

        public void Ping()
        {
            PubSubServiceClient.Ping();
        }

        public void UnsubscribeChannel(int Channel)
        {
            PubSubServiceClient.UnsubscribeChannel(Channel);
        }
    }

    #endregion
}
