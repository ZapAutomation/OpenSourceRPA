namespace ZappyMessages.PubSub
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PubSubServiceClient : System.ServiceModel.DuplexClientBase<IPubSubService>, IPubSubService
    {

        public PubSubServiceClient(System.ServiceModel.InstanceContext callbackInstance) :
            base(callbackInstance)
        {
        }

        public PubSubServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) :
            base(callbackInstance, endpointConfigurationName)
        {
        }

        public PubSubServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) :
            base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public PubSubServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public PubSubServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(callbackInstance, binding, remoteAddress)
        {
        }

        public void Subscribe(string SubscriberName, int[] channels)
        {
            base.Channel.Subscribe(SubscriberName, channels);
        }



        public void Unsubscribe()
        {
            base.Channel.Unsubscribe();
        }


        public void Publish(int channel, string PublishedString)
        {
            base.Channel.Publish(channel, PublishedString);
        }



        public void PublishBinary(int channel, byte[] PublishedBinaryData)
        {
            base.Channel.PublishBinary(channel, PublishedBinaryData);
        }



        public void Ping()
        {
            base.Channel.Ping();
        }

        public void UnsubscribeChannel(int Channel)
        {
            base.Channel.UnsubscribeChannel(Channel);
        }
    }
}