using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using ZappyMessages.Helpers;
using ZappyMessages.Logger;
using ZappyMessages.PubSubHelper;

namespace ZappyMessages.PubSub
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class PubSubService : IPubSubService
    {
        public static readonly PubSubService Instance = new PubSubService();
        private PubSubService()
        {

        }
        private PubSubProxyClientIdentifier[] _callbackChannels = new PubSubProxyClientIdentifier[8];
        private IPubSubSubscriber _mySubscriberCallback;
        private ServiceHost _serviceHost;

        public Timer _ReconnectTimer;

        public void Start()
        {
            try
            {
                _serviceHost = new ServiceHost(Instance);

                ServiceMetadataBehavior serviceMetadataBehavior =
                       _serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();

                if (serviceMetadataBehavior == null)
                {
                    serviceMetadataBehavior = new ServiceMetadataBehavior();
                    _serviceHost.Description.Behaviors.Add(serviceMetadataBehavior);
                }

                NetNamedPipeBinding _Binding = new NetNamedPipeBinding()
                {
                    MaxBufferPoolSize = 2147483647,
                    MaxBufferSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647
                };

                string _RemoteAddress = ZappyMessagingConstants.EndpointLocationZappyService;


                _serviceHost.AddServiceEndpoint(typeof(ZappyMessages.PubSub.IPubSubService), _Binding, _RemoteAddress);
                
                _serviceHost.AddServiceEndpoint(
                    typeof(IMetadataExchange),
                    MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                    ZappyMessagingConstants.EndpointLocationZappyService + "/mex"
                );

                //USE THIS FOR HUB CONNECTIONS
                //_serviceHost.AddServiceEndpoint(
                //                    typeof(IMetadataExchange),
                //                    MetadataExchangeBindings.CreateMexTcpBinding(),
                //                    "net.tcp://localhost/ZappyPubSubServiceTcp/mex"
                //                );


                _serviceHost.Open();
                _ReconnectTimer = new Timer(TimerCallback, null, 30000, 30000);


                //Console.WriteLine("Service started");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void StartTcpService()
        //{
        //    try
        //    {
        //        _serviceHost = new ServiceHost(Instance);

        //        ServiceMetadataBehavior serviceMetadataBehavior =
        //               _serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();

        //        if (serviceMetadataBehavior == null)
        //        {
        //            serviceMetadataBehavior = new ServiceMetadataBehavior();
        //            _serviceHost.Description.Behaviors.Add(serviceMetadataBehavior);
        //        }
             
        //        //_serviceHost.AddServiceEndpoint(
        //        //    typeof(IMetadataExchange),
        //        //    MetadataExchangeBindings.CreateMexNamedPipeBinding(),
        //        //    "net.pipe://localhost/ZappyPubSubServicePipe/mex"
        //        //);

        //        //net.pipe://localhost/ZappyPubSubServicePipe/mex
        //        //USE THIS FOR HUB CONNECTIONS
        //        _serviceHost.AddServiceEndpoint(
        //                            typeof(IMetadataExchange),
        //                            MetadataExchangeBindings.CreateMexTcpBinding(),
        //                            "net.tcp://localhost/ZappyPubSubServiceTcp/mex"
        //                        );


        //        _serviceHost.Open();
        //        _ReconnectTimer = new Timer(TimerCallback, null, 30000, 30000);


        //        //Console.WriteLine("Service started");
        //    }
        //    catch (Exception e)
        //    {
        //        ConsoleLogger.Error(e);
        //    }
        //}

        //Check on how to get this serviced endpoints
        public List<Uri> GetServicedEndpoints()
        {
            List<Uri> _Uris = new List<Uri>(_serviceHost.ChannelDispatchers.Count);
            foreach (var channelDispatcher in _serviceHost.ChannelDispatchers)
            {
                _Uris.Add(channelDispatcher.Listener.Uri);
            }

            return _Uris;
        }

        public void Start(ServiceEndpoint ServiceEndpoint)
        {
            try
            {
                _serviceHost = new ServiceHost(Instance);
                _serviceHost.AddServiceEndpoint(ServiceEndpoint);
                _serviceHost.Open();
                _ReconnectTimer = new Timer(TimerCallback, null, 30000, 30000);
                //Console.WriteLine("Service started");
            }
            catch (Exception e)
            {
                ConsoleLogger.Error(e);
            }
        }

        public bool Start(string TcpUrl)
        {
            try
            {
                _serviceHost = new ServiceHost(Instance);
                _serviceHost.AddServiceEndpoint(typeof(IPubSubService), new NetTcpBinding()
                {
                    MaxBufferPoolSize = 2147483647,
                    MaxBufferSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647
                }, TcpUrl);
                _serviceHost.Open();
                _ReconnectTimer = new Timer(TimerCallback, null, 30000, 30000);
                ConsoleLogger.Info("Service started");
                return true;
            }
            catch (AddressAlreadyInUseException)
            {
                return false;
            }
        }

        void TimerCallback(object dummy)
        {
            for (int i = 0; i < _callbackChannels.Length; i++)
            {
                PubSubProxyClientIdentifier _Tuple = _callbackChannels[i];
                if (_Tuple == null)
                    continue;
                try
                {
                    _Tuple.Subscription.PingClient();
                }
                catch //(Exception e)
                {
                    ConsoleLogger.Info(string.Format("Subscription Client Ping Error:{0}, subscribed:{1}", _Tuple.ClientRegistrationID, string.Join("|", _Tuple.Channels)));
                }
            }

        }

        public void Publish(int channel, string publish)
        {
            //publish = StringCipher.Encrypt(publish, ZappyMessagingConstants.MessageKey);
            for (int i = 0; i < _callbackChannels.Length; i++)
            {
                PubSubProxyClientIdentifier _Tuple = _callbackChannels[i];

                try
                {
                    if (IsChannelSubscribed(channel, _Tuple))
                        _Tuple.Subscription.OnPublished(channel, publish);
                }
                catch (Exception)
                {
                    _callbackChannels[i] = null;
                    ConsoleLogger.Info(string.Format("Subscription Dropped:{0}", _Tuple.ToString()));
                }
            }

        }
        public void Publish(string ClientRegistrationID, int channel, string publish)
        {
            for (int i = 0; i < _callbackChannels.Length; i++)
            {
                PubSubProxyClientIdentifier _Tuple = _callbackChannels[i];
                if (_Tuple != null && _Tuple.ClientRegistrationID == ClientRegistrationID)
                {
                    try
                    {
                        _Tuple.Subscription.OnPublished(channel, publish);
                    }
                    catch //(Exception e)
                    {
                        _callbackChannels[i] = null;
                        ConsoleLogger.Info(string.Format("Subscription Dropped:{0}", _Tuple.ToString()));
                    }
                    break;
                }
            }
        }

        public void PublishOptimized(int channel, object publish)
        {
            string _PublishData = null;
            for (int i = 0; i < _callbackChannels.Length; i++)
            {
                PubSubProxyClientIdentifier _Tuple = _callbackChannels[i];
                try
                {
                    if (IsChannelSubscribed(channel, _Tuple))
                    {
                        if (_PublishData == null)
                            _PublishData = ZappySerializer.SerializeObject(publish);

                        _Tuple.Subscription.OnPublished(channel, _PublishData);
                    }
                }
                catch //(Exception e)
                {
                    _callbackChannels[i] = null;
                    ConsoleLogger.Info(string.Format("Subscription Dropped:{0}, subscribed:{1}", _Tuple.ClientRegistrationID, string.Join("|", _Tuple.Channels)));
                }
            }

        }

        bool IsChannelSubscribed(int Channel, PubSubProxyClientIdentifier client)
        {
            int _RetryCount = 0, MaxRetryCount = 3;
        __RETRY:
            try
            {
                if (client == null || client.Channels == null)
                    return false;
                return (client.Channels.Count == 1 && client.Channels[0] == PubSubTopicRegister.ZappyTraceChannel) ||
                           client.Channels.IndexOf(Channel) >= 0;
            }
            catch
            {
                if (++_RetryCount <= MaxRetryCount)
                {
                    Thread.Sleep(10);
                    goto __RETRY;
                }
                else
                {
                    return false;
                }
            }
        }

        public void PublishBinary(int channel, byte[] publish)
        {
            for (int i = 0; i < _callbackChannels.Length; i++)
            {
                PubSubProxyClientIdentifier _Tuple = _callbackChannels[i];

                try
                {
                    if (IsChannelSubscribed(channel, _Tuple))
                        _Tuple.Subscription.OnPublishedBinary(channel, publish);
                }
                catch //(Exception e)
                {
                    //ConsoleExtension.WriteError(e);
                    _callbackChannels[i] = null;
                    ConsoleLogger.Info(string.Format("Subscription Dropped:{0}", _Tuple.ToString()));
                }
            }
        }

        public void Subscribe(string SubscriberName, int[] channels)
        {

            IPubSubSubscriber callback = _mySubscriberCallback = OperationContext.Current.GetCallbackChannel<IPubSubSubscriber>();
            Subscribe(SubscriberName, callback, channels);
        }

        public void Subscribe(string SubscriberName, IPubSubSubscriber callback, int[] channels)
        {
            if (channels == null || channels.Length == 0)
                channels = new int[] { PubSubTopicRegister.ZappyTraceChannel };

            PubSubProxyClientIdentifier _SubscriptionRef = null;
            lock (_callbackChannels)
            {
                bool _HasSpace = false;

                for (int i = 0; i < _callbackChannels.Length; i++)
                {
                    if (_callbackChannels[i] == null)
                    {
                        _HasSpace = true;
                        continue;
                    }

                    if (_callbackChannels[i].Subscription == callback)
                    {
                        _SubscriptionRef = _callbackChannels[i];
                        break;
                    }
                }

                if (_SubscriptionRef == null)
                {
                    if (!_HasSpace)
                    {
                        PubSubProxyClientIdentifier[] tuples = new PubSubProxyClientIdentifier[_callbackChannels.Length * 2];
                        Array.Copy(_callbackChannels, tuples, _callbackChannels.Length);
                        _callbackChannels = tuples;
                    }

                    for (int i = 0; i < _callbackChannels.Length; i++)
                    {
                        if (_callbackChannels[i] == null)
                        {
                            _SubscriptionRef = _callbackChannels[i] = new PubSubProxyClientIdentifier(callback, channels, SubscriberName);
                            ConsoleLogger.Info(string.Format("Subscription Added:{0}", _SubscriptionRef.ToString()));
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < channels.Length; i++)
                        if (_SubscriptionRef.Channels.IndexOf(channels[i]) < 0)
                            _SubscriptionRef.Channels.Add(channels[i]);

                    ConsoleLogger.Info(string.Format("Subscription Modified:{0}", _SubscriptionRef.ToString()));
                }
            }
        }

        public void Unsubscribe()
        {
            IPubSubSubscriber callback = OperationContext.Current.GetCallbackChannel<IPubSubSubscriber>();
            UnsubscribeChannelInternal(callback, int.MinValue);
        }

        public void UnsubscribeChannel(int Channel)
        {
            IPubSubSubscriber callback = OperationContext.Current.GetCallbackChannel<IPubSubSubscriber>();
            UnsubscribeChannelInternal(callback, Channel);
        }

        void UnsubscribeChannelInternal(IPubSubSubscriber callback, int Channel)
        {
            lock (_callbackChannels)
            {
                for (int i = 0; i < _callbackChannels.Length; i++)
                {
                    if (_callbackChannels[i].Subscription == null)
                        continue;

                    if (_callbackChannels[i].Subscription == callback)
                    {
                        PubSubProxyClientIdentifier _SubscriptionRef = _callbackChannels[i];

                        if (Channel == int.MinValue)
                        {

                            _callbackChannels[i] = null;
                            ConsoleLogger.Info(string.Format("Subscription Dropped:{0}", _SubscriptionRef));
                        }
                        else
                        {
                            if (_SubscriptionRef.Channels.IndexOf(Channel) >= 0)
                            {
                                _SubscriptionRef.Channels.Remove(Channel);
                                ConsoleLogger.Info(string.Format("Subscription Modified:{0}", _SubscriptionRef));
                            }
                        }
                        break;
                    }
                }
            }
        }

        // public void Stop() => _serviceHost.Abort();
        public void Stop()
        {
            _serviceHost?.Abort();
        }

        public void Ping() { }


    }
}
