namespace ZappyMessages.PubSub
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPubSubSubscriber
    {

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IPubSubService/OnPublished")]
        void OnPublished(int channel, string PublishedString);

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IPubSubService/OnPublishedBinary")]
        void OnPublishedBinary(int channel, byte[] PublishedBinaryData);

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IPubSubService/PingClient")]
        void PingClient();
    }
}