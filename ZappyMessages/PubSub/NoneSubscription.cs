namespace ZappyMessages.PubSub
{
    public class NoneSubscription : ZappyMessages.PubSub.IPubSubSubscriber
    {
        public void OnPublished(int channel, string publish)
        {
        }

        public void OnPublishedBinary(int channel, byte[] publish)
        {
        }

        public void PingClient()
        {
        }
    }
}