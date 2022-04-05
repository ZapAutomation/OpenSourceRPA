using ZappyMessages.Logger;

namespace ZappyMessages.PubSub
{
    public class LoggerSubscription : ZappyMessages.PubSub.IPubSubSubscriber
    {
        public void OnPublished(int channel, string publish)
        {
            ConsoleLogger.Info(string.Format(" publication:{0}, {1}", channel.ToString(), publish));
        }

        public void OnPublishedBinary(int channel, byte[] publish)
        {
            ConsoleLogger.Info(string.Format(" publication:{0}, {1}", channel.ToString(), string.Join("|", publish)));
        }
        public void PingClient()
        {
        }
    }
}