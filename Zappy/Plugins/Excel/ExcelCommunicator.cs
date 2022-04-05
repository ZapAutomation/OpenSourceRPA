


using System;
using System.Drawing;
using System.Threading;
using Zappy.Helpers;
using ZappyMessages;
using ZappyMessages.ExcelMessages;
using ZappyMessages.Helpers;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.Plugins.Excel
{
                public class ExcelCommunicator : IExcelZappyTaskCommunication, IPubSubSubscriber
    {
        public static void Init()
        {
            Instance = new ExcelCommunicator(PubSubService.Instance, PubSubTopicRegister.ZappyExcelRequest, PubSubTopicRegister.ZappyExcelResponse);
            PubSubService.Instance.Subscribe("Zappy2Excel", Instance as ExcelCommunicator, new int[] { PubSubTopicRegister.ZappyExcelResponse });
        }
        public static void Init(PubSubClient Client)
        {
            Instance = new ExcelCommunicator(Client, PubSubTopicRegister.ZappyPlaybackHelper2ExcelRequest, PubSubTopicRegister.Excel2ZappyPlaybackHelperResponse);
            Client.DataPublished += ((ExcelCommunicator)Instance).OnPublished;

        }

        ManualResetEventSlim _mre;
        string _Response;
        IPubSubService _PubSub;
        readonly int _RequestChannel, _ResponseChannel;
                                internal static IExcelZappyTaskCommunication Instance { get; private set; }

                                public ExcelCommunicator(IPubSubService PubSub, int RequestChannel, int ResponseChannel)
        {
            _RequestChannel = RequestChannel;
            _ResponseChannel = ResponseChannel;
            _mre = new ManualResetEventSlim(false);
            _PubSub = PubSub;
        }

        int _RequestCount = 0, _ExcelResponseTimeout = 1500;

        public ExcelElementInfo GetElementFromPoint(int x, int y)
        {
            lock (Instance)
            {
                Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++,
                    ExcelRequest.GetElementFromPoint, ZappySerializer.SerializeObject(new Point(x, y)));
                _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
                _mre.Reset();
                if (_mre.Wait(_ExcelResponseTimeout))
                    return ZappySerializer.DeserializeObject<Tuple<int, ExcelElementInfo>>(_Response).Item2;
                return
                    null;
            }
        }

        public ExcelElementInfo GetFocussedElement()
        {
            lock (Instance)
            {
                Tuple<int, ExcelRequest, string> _Request =
                    new Tuple<int, ExcelRequest, string>(_RequestCount++, ExcelRequest.GetFocussedElement,
                        string.Empty);
                _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
                _mre.Reset();
                if (_mre.Wait(_ExcelResponseTimeout))
                {
                    ExcelElementInfo o = ZappySerializer.DeserializeObject<Tuple<int, ExcelElementInfo>>(_Response)
                        .Item2;
                    return o;
                }

                return
                    null;
            }
        }

        public double[] GetBoundingRectangle(ExcelCellInfo cellInfo)
        {
            lock (Instance)
            {
                Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++,
                    ExcelRequest.GetBoundingRectangle, ZappySerializer.SerializeObject(cellInfo));
                _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
                _mre.Reset();
                if (_mre.Wait(_ExcelResponseTimeout))
                    return ZappySerializer.DeserializeObject<Tuple<int, double[]>>(_Response).Item2;
                return
                    null;
            }
        }

        public object GetCellProperty(ExcelCellInfo cellInfo, string propertyName)
        {
            lock (Instance)
            {
                Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++,
                    ExcelRequest.GetCellProperty,
                    ZappySerializer.SerializeObject(cellInfo) + CrapyConstants.StringArrayDelemiter + propertyName);
                _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
                _mre.Reset();
                if (_mre.Wait(_ExcelResponseTimeout))
                    return ZappySerializer.DeserializeObject<Tuple<int, object>>(_Response).Item2;
                throw new Exception("Unable To Get Cell Value");
            }
        }



        public void SetFocus(ExcelCellInfo cellInfo)
        {
            Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++, ExcelRequest.SetFocus, ZappySerializer.SerializeObject(cellInfo));
            _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
            _mre.Reset();
            _mre.Wait(_ExcelResponseTimeout);
        }

        public void ScrollIntoView(ExcelCellInfo cellInfo)
        {
            Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++, ExcelRequest.ScrollIntoView, ZappySerializer.SerializeObject(cellInfo));
            _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));

            _mre.Reset();

            _mre.Wait(_ExcelResponseTimeout);
        }

        public bool SetCellProperty(ExcelCellInfo cellInfo, string propertyName, object propertyValue)
        {

            Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++, ExcelRequest.SetCellProperty,
                ZappySerializer.SerializeObject(new Tuple<ExcelCellInfo, string, object>(cellInfo, propertyName, propertyValue)));

            _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));

            _mre.Reset();

            if (_mre.Wait(_ExcelResponseTimeout))
                return true;
            throw new Exception("Unable To Set Value");

        }

        public object RunExcelCustomAction(ExcelCellInfo cellInfo, string propertyName, object propertyValue)
        {
            lock (Instance)
            {
                Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++,
                    ExcelRequest.RunExcelCustomAction,
                    ZappySerializer.SerializeObject(
                        new Tuple<ExcelCellInfo, string, object>(cellInfo, propertyName, propertyValue)));
                _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
                _mre.Reset();
                if (_mre.Wait(_ExcelResponseTimeout))
                    return ZappySerializer.DeserializeObject<Tuple<int, object>>(_Response).Item2;
                return
                    null;
            }
        }

        public bool SortExcelRange(ExcelCellInfo cellInfo, string sortingOrder, int columnToSort)
        {
            lock (Instance)
            {
                Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++,
                    ExcelRequest.SortRange,
                    ZappySerializer.SerializeObject(
                        new Tuple<ExcelCellInfo, string, int>(cellInfo, sortingOrder, columnToSort)));
                _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
                _mre.Reset();

                if (_mre.Wait(_ExcelResponseTimeout))
                    return true;
                throw new Exception("Unable To Sort");
            }
        }

                                                                                                                                
        public void ActivateWorksheet(ExcelCellInfo cellInfo)
        {
            Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++, ExcelRequest.ActivateWorksheet, ZappySerializer.SerializeObject(cellInfo));
            SendRequest(_Request);
        }

        public void SaveWorkbook(ExcelCellInfo cellInfo)
        {
            Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++, ExcelRequest.SaveWorkbook, ZappySerializer.SerializeObject(cellInfo));
            SendRequest(_Request);
        }

        public void SaveWorkbookAs(ExcelCellInfo cellInfo)
        {
            Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++, ExcelRequest.SaveWorkbookAs, ZappySerializer.SerializeObject(cellInfo));
            SendRequest(_Request);
        }
        private void SendRequest(Tuple<int, ExcelRequest, string> _Request)
        {
            _PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
            _mre.Reset();
            _mre.Wait(_ExcelResponseTimeout);
        }
        
        public void OnPublished(PubSubClient clnt, int channel, string PublishedString)
        {
            if (channel == _ResponseChannel)
            {
                _Response = PublishedString;
                _mre.Set();
            }
        }

        public void OnPublishedBinary(int channel, byte[] PublishedBinaryData)
        {

        }

        public void PingClient()
        {

        }

        public void OnPublished(int channel, string PublishedString)
        {
            if (channel == _ResponseChannel)
            {
                _Response = PublishedString;
                _mre.Set();
            }
                    }
    }
}
