using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ZappyMessages.RobotHubPubSub
{
    public class ZappyPubSubClient : ClientBase<IZappyPubSubService>, IZappyPubSubService
    {
        private Binding endpointBinding;
        private EndpointAddress endpointAddress;

        public int[] SubscriptionChannels { get; set; }

        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
            private set
            {
                if (_isConnected == value)
                    return;
                _isConnected = value;
            }
        }

        public ZappyPubSubClient(Binding binding, EndpointAddress address) : base(binding, address)
        {
            endpointBinding = binding;
            endpointAddress = address;
            Connect();
        }

        public void Connect()
        {
            try
            {


                Open();
                IsConnected = true;
            }
            catch//(Exception excp)
            {
                IsConnected = false;
            }
        }

        public void PingServer()
        {
            Channel.PingServer();
        }

        public void PublishMessage(IPubSubClientIdentifier clientIdentifier, int channel, string message)
        {
            //Console.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} In Client.PublishMessage()");
            Channel.PublishMessage(clientIdentifier, channel, message);
        }

        public bool Subscribe(IPubSubClientIdentifier clientIdentifier, int[] channels, string additionalInfo)
        {
            if (InnerChannel.State != CommunicationState.Faulted)
            {
                return Channel.Subscribe(clientIdentifier, channels, additionalInfo);
            }

            return false;
        }

        public bool Unsubscribe(IPubSubClientIdentifier clientIdentifier, int channel, string additionalInfo)
        {
            return Channel.Unsubscribe(clientIdentifier, channel, additionalInfo);
        }

        public PubSubServerState GetCurrentState()
        {
            return Channel.GetCurrentState();
        }
    }
}