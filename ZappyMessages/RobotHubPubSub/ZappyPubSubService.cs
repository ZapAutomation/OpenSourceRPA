using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using ZappyMessages.Logger;

namespace ZappyMessages.RobotHubPubSub
{
    //Listner //Server
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ZappyPubSubService : IZappyPubSubService, IErrorHandler
    {
        private ServiceHost _serviceHost;

        public delegate void GetServerStateActionHandler(ref PubSubServerState state);

        public event Action<IPubSubClientIdentifier, int[], string> Subscribing;
        public event Action<IPubSubClientIdentifier, int, string> Unsubscribing;
        public event Action<IPubSubClientIdentifier, int, string> Publishing;
        public event GetServerStateActionHandler ServerStateRequested;

        public event Action Pinging;

        public ZappyPubSubService()
        {
        }

        public void Init(Uri[] hostingAddress, Binding binding)
        {
            try
            {
                ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior();
                _serviceHost = new ServiceHost(this);

                //FIXME: Add appropriate behaviour
                serviceMetadataBehavior.HttpGetEnabled = true;
                serviceMetadataBehavior.HttpGetUrl = hostingAddress[0];
                //_serviceHost.AddServiceEndpoint(typeof(IZappyPubSubService), new BasicHttpBinding(),
                //"http://localhost:8010/ZappyHub");

                foreach (var addr in hostingAddress)
                {
                    _serviceHost.AddServiceEndpoint(typeof(IZappyPubSubService), binding, addr);
                    _serviceHost.Description.Behaviors.Add(serviceMetadataBehavior);
                }

                _serviceHost.Open();
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                ConsoleLogger.Error(ex);
                throw ex;
            }
        }

        #region Service functions

        public void PingServer()
        {
            Pinging?.Invoke();
        }

        public void PublishMessage(IPubSubClientIdentifier clientIdentifier, int channel, string message)
        {
            Publishing?.Invoke(clientIdentifier, channel, message);
        }

        public bool Subscribe(IPubSubClientIdentifier clientIdentifier, int[] channels, string additionalInfo)
        {
            Subscribing?.Invoke(clientIdentifier, channels, additionalInfo);
            return true;
        }

        public bool Unsubscribe(IPubSubClientIdentifier clientIdentifier, int channel, string additionalInfo)
        {
            Unsubscribing?.Invoke(clientIdentifier, channel, additionalInfo);
            return true;
        }

        public PubSubServerState GetCurrentState()
        {
            PubSubServerState state = PubSubServerState.Working;
            ServerStateRequested?.Invoke(ref state);
            return state;
        }

        #endregion

        #region Remove later

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            Debug.WriteLine(error.Message);
        }

        public bool HandleError(Exception error)
        {
            return true;
        }

        #endregion
    }

    //Speaker/communicator  //Client
}
