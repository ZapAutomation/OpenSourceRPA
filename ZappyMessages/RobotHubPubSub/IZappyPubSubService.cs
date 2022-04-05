using System.ServiceModel;

namespace ZappyMessages.RobotHubPubSub
{
    /*
     * ==============================================================================
     */

    [ServiceContract]
    [ServiceKnownType(typeof(PubSubIdentifier))]
    public interface IZappyPubSubService
    {
        [OperationContract]
        bool Subscribe(IPubSubClientIdentifier clientIdentifier, int[] channels, string additionalInfo);

        [OperationContract]
        bool Unsubscribe(IPubSubClientIdentifier clientIdentifier, int channel, string additionalInfo);

        [OperationContract]
        void PublishMessage(IPubSubClientIdentifier clientIdentifier, int channel, string message);

        [OperationContract]
        void PingServer();

        [OperationContract]
        PubSubServerState GetCurrentState();
    }
}
