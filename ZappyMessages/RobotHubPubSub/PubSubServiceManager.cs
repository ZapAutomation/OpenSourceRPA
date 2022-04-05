using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using ZappyMessages.Logger;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace ZappyMessages.RobotHubPubSub
{
    public class PubSubServiceManager<BindingType>
        where BindingType : Binding
    {
        private static PubSubServiceManager<BindingType> _singletonInstance;

        private ZappyPubSubService _service;
        private Dictionary<string, ZappyPubSubClient> _clients;
        private Dictionary<string, IPubSubClientIdentifier> _clientIdentifiers;

        private int _activeOperationCount = 0;
        private PubSubProxyClientIdentifier[] _callbackChannels = new PubSubProxyClientIdentifier[8];

        public delegate void RegisteringWithClientHandler(IPubSubClientIdentifier idntf, ref string addnData);
        public delegate void GettingServerStateHandler(ref PubSubServerState state);

        public event RegisteringWithClientHandler RegisteringWithClient;
        public event GettingServerStateHandler GettingServerState;
        public event Action<IPubSubClientIdentifier, string> NewClientAdded;
        public event Action<IPubSubClientIdentifier, string> Reconnected;
        public event Action<IPubSubClientIdentifier> ClientRemoved;
        public event Action<IPubSubClientIdentifier, int, string> ReceivedPublish;
        public event Action ReceivedPing;

        public bool IsWorking
        {
            get
            {
                return (_activeOperationCount > 0) ? true : false;
            }
        }

        public PubSubServerState ServerState { get; set; }

        public IList<IPubSubClientIdentifier> Clients
        {
            get
            {
                return _clientIdentifiers.Values.ToList().AsReadOnly();
            }
        }

        public IPubSubClientIdentifier Identifier { get; private set; }

        public PubSubServiceManager(IPubSubClientIdentifier identifier)
        {
            Identifier = identifier;

            _clients = new Dictionary<string, ZappyPubSubClient>();
            _clientIdentifiers = new Dictionary<string, IPubSubClientIdentifier>();

            _service = new ZappyPubSubService();
            _service.Subscribing += Service_Subscribing;
            _service.Unsubscribing += Service_Unsubscribing;
            _service.Publishing += Service_Publishing;
            _service.Pinging += Service_Pinging;
            _service.ServerStateRequested += Service_ServerStateRequested;

            _service.Init(identifier.EndpointAddresses.Select(r => new Uri(r)).ToArray(),
                (BindingType)Activator.CreateInstance(typeof(BindingType), null));

            ServerState = PubSubServerState.Working;
        }

        public static PubSubServiceManager<BindingType> GetInstance(IPubSubClientIdentifier idntf)
        {
            if (_singletonInstance == null)
            {
                _singletonInstance = new PubSubServiceManager<BindingType>(idntf);
            }

            return _singletonInstance;
        }

        public void RequestClientConnection(string endpointAddr, string addnData)
        {
            //Ping the client to request for connection
            ZappyPubSubClient tmpClientConn =
                new ZappyPubSubClient(
                    (BindingType)Activator.CreateInstance(typeof(BindingType), null),
                    new EndpointAddress(endpointAddr));

            tmpClientConn.Subscribe(Identifier, null, addnData);
        }

        public void RequestClientConnection(IPubSubClientIdentifier idntf, string addnData)
        {
            Service_SubscribeToHub(idntf, null, addnData);
        }

        public void AddClient(IPubSubClientIdentifier idntf)
        {
            //Remove old ones as this may be a HUB
            if (_clients.ContainsKey(idntf.ID))
            {
                _clients.Remove(idntf.ID);
                _clientIdentifiers.Remove(idntf.ID);
            }

            //Configure with new parameters
            _clients.Add(
                    idntf.ID,
                    new ZappyPubSubClient((BindingType)Activator.CreateInstance(typeof(BindingType), null),
                    new EndpointAddress(idntf.EndpointAddresses.First())));
        }

        public void AddClient(IPubSubClientIdentifier idntf, int[] channels)
        {
            //If client exists then just reset its properties
            if (_clients.ContainsKey(idntf.ID))
                return;

            //Else register the new client
            _clients.Add(idntf.ID,
                    new ZappyPubSubClient((BindingType)Activator.CreateInstance(typeof(BindingType), null),
                    new EndpointAddress(idntf.EndpointAddresses.First())));

            //Save the identifier info
            _clientIdentifiers.Add(idntf.ID, idntf);
        }

        public void RemoveClient(IPubSubClientIdentifier idntf, int channel)
        {
            if (!_clients.ContainsKey(idntf.ID))
                return;

            //Remove this client
            _clients.Remove(idntf.ID);
            _clientIdentifiers.Remove(idntf.ID);

            //Notify watchers that a client was removed
            ClientRemoved?.Invoke(idntf);
        }

        public IPubSubClientIdentifier GetIdentifier(string uid)
        {
            if (_clientIdentifiers.ContainsKey(uid))
                return _clientIdentifiers[uid];

            return null;
        }

        public bool PingClient(IPubSubClientIdentifier idntf)
        {
            //Publish to all the connected clients
            if (_clients.ContainsKey(idntf.ID))
            {
                var client = _clients[idntf.ID];

                try
                {
                    //If not connected then connect
                    if (!client.IsConnected)
                        client.Connect();

                    //Publish message to the client
                    client.PingServer();
                }
                catch (Exception)
                {
                    ConsoleLogger.Info(string.Format("Ping failed:{0}", client.Endpoint.Address.ToString()));
                    return false;
                }
            }

            return true;
        }

        public void Publish(int channel, string message)
        {
            Interlocked.Increment(ref _activeOperationCount);

            //Publish to all the connected clients
            foreach (var client in _clients.Values)
            {
                try
                {
                    //If not connected then connect
                    if (!client.IsConnected)
                        client.Connect();

                    //Publish message to the client
                    client.PublishMessage(Identifier, channel, message);
                }
                catch (Exception)
                {
                    ConsoleLogger.Info(string.Format("Publish failed:{0}", client.Endpoint.Address.ToString()));
                }
            }

            Interlocked.Decrement(ref _activeOperationCount);
        }

        public void Publish(IPubSubClientIdentifier idntf, int channel, string message)
        {
            Interlocked.Increment(ref _activeOperationCount);

            //Publish to all the connected clients
            if (_clients.ContainsKey(idntf.ID))
            {
                var client = _clients[idntf.ID];

                try
                {
                    //If not connected then connect
                    if (!client.IsConnected)
                        client.Connect();

                    //Publish message to the client
                    client.PublishMessage(Identifier, channel, message);
                }
                catch (Exception)
                {
                    ConsoleLogger.Info(string.Format("Publish failed:{0}", client.Endpoint.Address.ToString()));
                }
            }

            Interlocked.Decrement(ref _activeOperationCount);
        }

        public void Subscribe(IPubSubClientIdentifier idntf, int[] channels, string addnData)
        {
            _clients[idntf.ID]?.Subscribe(Identifier, channels, addnData);
        }

        public PubSubServerState GetCurrentState(IPubSubClientIdentifier idntf)
        {
            return _clients[idntf.ID]?.GetCurrentState() ?? PubSubServerState.Busy;
        }

        public void AddSubscriberClass(IPubSubSubscriber callback, string subscriberName, int[] channels)
        {
            if (callback == null)
                callback = OperationContext.Current.GetCallbackChannel<IPubSubSubscriber>();

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
                            _SubscriptionRef = _callbackChannels[i] = new PubSubProxyClientIdentifier(callback, channels, subscriberName);
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

        public void RemoveSubscriberClass(IPubSubSubscriber callback, string subscriberName, int channel = int.MinValue)
        {
            if (callback == null)
                callback = OperationContext.Current.GetCallbackChannel<IPubSubSubscriber>();

            lock (_callbackChannels)
            {
                for (int i = 0; i < _callbackChannels.Length; i++)
                {
                    if (_callbackChannels[i].Subscription == null)
                        continue;

                    if (_callbackChannels[i].Subscription == callback)
                    {
                        PubSubProxyClientIdentifier _SubscriptionRef = _callbackChannels[i];

                        if (channel == int.MinValue)
                        {
                            _callbackChannels[i] = null;
                            ConsoleLogger.Info(string.Format("Subscription Dropped:{0}", _SubscriptionRef));
                        }
                        else
                        {
                            if (_SubscriptionRef.Channels.IndexOf(channel) >= 0)
                            {
                                _SubscriptionRef.Channels.Remove(channel);
                                ConsoleLogger.Info(string.Format("Subscription Modified:{0}", _SubscriptionRef));
                            }
                        }

                        break;
                    }
                }
            }
        }

        private bool IsChannelSubscribed(int Channel, PubSubProxyClientIdentifier client)
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

        private void Service_Subscribing(IPubSubClientIdentifier idntf, int[] channels, string addnData)
        {
            Interlocked.Increment(ref _activeOperationCount);

            if (!_clients.ContainsKey(idntf.ID))
            {
                //First add client to our client list
                AddClient(idntf, channels);

                //See if users of this manager want to provide some additional data
                string additionalData = "";
                RegisteringWithClient?.Invoke(idntf, ref additionalData);

                //Now subscribe to the client so that we can receive thier messages
                _clients[idntf.ID].Subscribe(Identifier, null, additionalData);

                //Inform watchers that a new client has been added
                NewClientAdded?.Invoke(idntf, addnData);
            }
            else
            {
                //Reconnect to client service
                if (!_clients[idntf.ID].IsConnected)
                {
                    _clients[idntf.ID].Connect();
                }

                //Inform the watchers that we have reconnected to this client
                Reconnected?.Invoke(idntf, addnData);
            }

            Interlocked.Decrement(ref _activeOperationCount);
        }


        private void Service_SubscribeToHub(IPubSubClientIdentifier idntf, int[] channels, string addnData)
        {
            lock (_clients)
            {
                AddClient(idntf, channels);

                //Now subscribe to the client so that we can receive thier messages
                _clients[idntf.ID].Subscribe(Identifier, null, addnData);

                //Inform watchers that a new client has been added
                NewClientAdded?.Invoke(idntf, addnData);
            }
        }

        private void Service_Unsubscribing(IPubSubClientIdentifier idntf, int channel, string addnData)
        {
            Interlocked.Increment(ref _activeOperationCount);

            RemoveClient(idntf, channel);

            Interlocked.Decrement(ref _activeOperationCount);
        }

        private void Service_Publishing(IPubSubClientIdentifier idntf, int channel, string message)
        {
            Interlocked.Increment(ref _activeOperationCount);

            ReceivedPublish?.Invoke(idntf, channel, message);

            if (_callbackChannels.Any())
            {
                for (int i = 0; i < _callbackChannels.Length; i++)
                {
                    PubSubProxyClientIdentifier _Tuple = _callbackChannels[i];

                    try
                    {
                        if (IsChannelSubscribed(channel, _Tuple))
                            _Tuple.Subscription.OnPublished(channel, message);
                    }
                    catch
                    {
                        _callbackChannels[i] = null;
                        ConsoleLogger.Info(string.Format("Subscription Dropped:{0}", _Tuple.ToString()));
                    }
                }
            }

            Interlocked.Decrement(ref _activeOperationCount);
        }

        private void Service_ServerStateRequested(ref PubSubServerState state)
        {
            //Check if we are serving some request and set state accordingly
            PubSubServerState tempState = ServerState = (IsWorking) ? PubSubServerState.Working : PubSubServerState.Idle;

            //If clients of this class wish to change state then check
            GettingServerState?.Invoke(ref tempState);

            state = tempState;
        }

        private void Service_Pinging()
        {
            ReceivedPing?.Invoke();
        }
    }
}
