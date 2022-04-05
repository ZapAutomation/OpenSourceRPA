using System.ServiceModel;

namespace ZappyMessages.RobotHubPubSub
{
    [ServiceKnownType(typeof(PubSubIdentifier))]
    public interface IPubSubClientIdentifier
    {
        string IpAddress { get; set; }
        string[] EndpointAddresses { get; set; }
        string ID { get; set; }
        string Name { get; set; }
    }
}
