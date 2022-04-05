using System.Collections.Generic;

namespace ZappyMessages.PubSub
{
    public class PubSubProxyClientIdentifier
    {
        public PubSubProxyClientIdentifier(IPubSubSubscriber callback, int[] channels, string subscriberName)
        {
            this.Subscription = callback;
            Channels = new List<int>(channels);
            ClientRegistrationID = subscriberName;
        }

        public string ClientRegistrationID { get; set; }
        public List<int> Channels { get; set; }
        public IPubSubSubscriber Subscription { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Subscribed : {1}", ClientRegistrationID, string.Join("|", Channels));
        }
    }
}
