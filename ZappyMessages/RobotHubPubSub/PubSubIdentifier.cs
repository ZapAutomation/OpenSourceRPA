namespace ZappyMessages.RobotHubPubSub
{
    public class PubSubIdentifier : IPubSubClientIdentifier
    {
        public string IpAddress { get; set; }

        public int HubListnerPort { get; set; }

        public string[] EndpointAddresses { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
    }
}