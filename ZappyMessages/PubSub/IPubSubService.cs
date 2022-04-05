namespace ZappyMessages.PubSub
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "ZappyMessages.PubSub.IPubSubService",
        CallbackContract = typeof(IPubSubSubscriber), SessionMode = System.ServiceModel.SessionMode.Required)]
    public interface IPubSubService
    {

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IPubSubService/Subscribe")]
        void Subscribe(string SubscriberName, int[] channels);



        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IPubSubService/Unsubscribe")]
        void Unsubscribe();

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IPubSubService/UnsubscribeUnsubscribeChannel")]
        void UnsubscribeChannel(int Channel);

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IPubSubService/Publish")]
        void Publish(int channel, string PublishedString);


        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IPubSubService/PublishBinary")]
        void PublishBinary(int channel, byte[] PublishedBinaryData);



        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IPubSubService/Ping")]
        void Ping();


    }
}