using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Zappy.Decode.LogManager;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.ZappyActions.Core
{
    [Description("Global Update for Zappy Robot Management")]
    public class GlobalUpdate : TemplateAction
    {
        public GlobalUpdate()
        {
            UpdateText = string.Empty;
            UpdateListenerAddress = string.Empty;
            UpdateText = string.Empty;
            UpdateChannel = PubSubTopicRegister.ZappyGlobalUpdate;
        }

        [Category("Input")]
        [Description("Update Text")]
        public DynamicProperty<string> UpdateText { get; set; }

        [Category("Input")]
        [Description("Listener Address Update")]
        public DynamicProperty<string> UpdateListenerAddress { get; set; }

        [Category("Input")]
        [Description("Update hannel")]
        public DynamicProperty<int> UpdateChannel { get; set; }

        [Category("Input")]
        [Description("PublisherID")]
        public DynamicProperty<string> PublisherID { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
                        Binding _Binding = null;
            if (UpdateListenerAddress.Value.StartsWith("net.tcp"))
                _Binding = new NetTcpBinding()
                {
                    MaxBufferPoolSize = 2147483647,
                    MaxBufferSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647
                };
            else if (UpdateListenerAddress.Value.StartsWith("net.pipe"))
                _Binding = new NetNamedPipeBinding()
                {
                    MaxBufferPoolSize = 2147483647,
                    MaxBufferSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647
                };
            EndpointAddress _RemoteAddress = new EndpointAddress(UpdateListenerAddress.Value);
            using (PubSubClient _Client = new PubSubClient(PublisherID.Value, _Binding, _RemoteAddress, null))
            {
                try
                {
                    if (_Client.IsConnected)
                        _Client.Publish(UpdateChannel.Value, UpdateText.Value);
                    else
                        throw new Exception("Unable to connect to Global Update Listener!");
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.Error(ex);
                    throw;
                }
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " UpdateText:" + this.UpdateText + " UpdateListenerAddress:" + this.UpdateListenerAddress + " UpdateChannel:" + this.UpdateChannel + " PublisherID:" + this.PublisherID;
        }
    }
}
