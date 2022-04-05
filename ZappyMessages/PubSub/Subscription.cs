using System;

namespace ZappyMessages.PubSub
{
    public class Subscription : IPubSubSubscriber
    {
        public event Action<int, string> DataPublished;
        public event Action<int, byte[]> BinaryDataPublished;

        public void OnPublished(int channel, string publish)
        {
            DataPublished?.Invoke(channel, publish);
        }

        public void OnPublishedBinary(int channel, byte[] publish)
        {
            BinaryDataPublished?.Invoke(channel, publish);
        }
        public void PingClient()
        {
        }
    }
}