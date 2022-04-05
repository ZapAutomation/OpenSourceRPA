using System;

namespace ZappyMessages.RobotHubPubSub
{
    public interface IZappyPubSubManager
    {
        event Action<IPubSubClientIdentifier, int, string> Published;
        event Action<IPubSubClientIdentifier> Subscribed;
        event Action<IPubSubClientIdentifier> Unsubscribed;

        bool Subscribe(int[] channels);

        bool Unsubscribe(int channel);

        void Publish(int channel, string message);
    }
}
