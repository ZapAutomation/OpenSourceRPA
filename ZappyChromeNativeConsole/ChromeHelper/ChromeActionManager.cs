using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZappyMessages;
using ZappyMessages.Chrome;
using ZappyMessages.Logger;

namespace ZappyChromeNativeConsole.ChromeHelper
{
    public static class ChromeActionManager
    {

        //list of all actions from this console to chrome
        static List<ChromeActionRequest> _OutBoundBufferedActions =
            new List<ChromeActionRequest>();


        static BaseChromeAction _InitTuple;

        private static Thread t1, t2;
        private static BinaryWriter _OutWriter;
        static AutoResetEvent readingfinished = new AutoResetEvent(false);

        public static void Init()
        {

            _InitTuple = new BaseChromeAction()
            {
                Timestamp = DateTime.Now,
                WindowID = 0,//Process.GetCurrentProcess().Parent().Parent().Id,
                TabID = 0
            };
            //ConsoleLogger.Info("parent chrome processid=" + _InitTuple.WindowID.ToString());

            _OutWriter = new BinaryWriter(Console.OpenStandardOutput());

            t1 = new Thread(RunStdInSniffer);
            t1.SetApartmentState(ApartmentState.STA);
            t1.Start();

            t2 = new Thread(RunStdOutWriter);
            t2.SetApartmentState(ApartmentState.STA);
            t2.Start();

            readingfinished.WaitOne();
            //readingfinished.WaitOne(System.Threading.Timeout.Infinite);

            t1.Abort();
            t2.Abort();
        }

        static void RunStdInSniffer()
        {
            BinaryReader _InReader = new BinaryReader(Console.OpenStandardInput());
            string input = string.Empty;
            //string[] _Separators = {"^^"};
            try
            {
                int _Len = _InReader.ReadInt32();

                while ((input = Encoding.UTF8.GetString(_InReader.ReadBytes(_Len))) != null)
                {

                    ReceivedJsonMessage messageReceived = null;
                    try
                    {

                        input = JsonConvert.DeserializeObject(input).ToString();
#if DEBUG
                        ConsoleLogger.Info(input);
#endif
                        //ChromeNativeConsoleLogger.Info(input);
                        messageReceived = JsonConvert.DeserializeObject<ReceivedJsonMessage>(input);
                        messageReceived.targetModified = new List<string>(messageReceived.target.Count);
                        for (int i = 0; i < messageReceived.target.Count; i++)
                        {
                            messageReceived.targetModified.Add(messageReceived.target[i][0]);
                        }
#if DEBUG
                        ConsoleLogger.Info(messageReceived.ToString());
                        ConsoleLogger.Info(string.Join("^^", messageReceived.targetModified));
#endif
                    }
                    catch (Exception)
                    {
#if DEBUG
                        ConsoleLogger.Info(" Cannot convert to JSON");
#endif
                        messageReceived = null;
                        //ChromeNativeConsoleLogger.Error(ex);
                    }

                    //send line
                    if (!ReferenceEquals(messageReceived, null))//chrome action triggered by user
                    {
                        try
                        {
                            ChromeNativeService.PublishChromeAction(DateTime.Now, messageReceived.windowId, messageReceived.tabId,
                                  messageReceived.url,
                                  messageReceived.command, messageReceived.targetModified, messageReceived.value, messageReceived.title);
                        }
                        catch (Exception ex)
                        {
                            ConsoleLogger.Error(ex);
                        }
                    }
                    else
                    {
                        if (_LastSentAction != null && _LastSentAction.TimeOutEvent != null) //it is a client originated command
                        {
                            _LastSentAction.CommandResult = input;

                            if (input.StartsWith("success"))
                            {
                                //_Retry = true;

                                try
                                {
                                    if (input.StartsWith("successCopy"))
                                    {
                                        string[] strSplit = input.Split(new string[] { "^^" }, StringSplitOptions.None);

                                        _LastSentAction._BaseAction.ActionUrlTab = strSplit[1];
                                        if (!SetTextInClipboard(strSplit[1], true, 5, 300))
                                            throw new Exception("Unable to \"Set\" text to clipboard!");
                                    }

                                    AutoResetEvent _Timeout = _LastSentAction.TimeOutEvent;

                                    if (_Timeout != null)
                                    {
                                        _LastSuccessfulAction = _LastSentAction;
                                        _Timeout.Set();
                                    }
                                    else
                                    {
#if DEBUG

                                        ConsoleLogger.Info(
                                            "Ignore-ReplayConfirmation for retry open:" +
                                            _LastSuccessfulAction.CommandName);
#endif
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ConsoleLogger.Error(ex);
                                }
                            }
                            else
                                _LastSentAction.TimeOutEvent?.Set();
                        }
                        else
                        {
#if DEBUG

                            ConsoleLogger.Info("recieved  " +
                                                           input);
#endif
                        }

                        _OutActionAcknowledgement.Set();
                    }

                    _Len = _InReader.ReadInt32();
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.Error(ex);
                //Stops when chrome stops here
            }

            readingfinished.Set();

        }

        public static bool SetTextInClipboard(string Text, bool Copy = true, int MaxRetryCount = 5, int RetryMillisecs = 200)
        {
            int _RetryCount = 0;
        __RETRYCOPY:
            try
            {
                Clipboard.SetDataObject(Text, true, MaxRetryCount, RetryMillisecs);
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.Error(ex);

                if (++_RetryCount <= MaxRetryCount)
                {
                    Thread.Sleep(RetryMillisecs);
                    goto __RETRYCOPY;
                }
                else
                    return false;
            }
        }

        internal static void EnqueueOutboundActionToChrome(ChromeActionRequest action)
        {
#if DEBUG
            ConsoleLogger.Info("New command : " + action.DisplayString());
#endif
            lock (_OutBoundBufferedActions)
            {
                _OutBoundBufferedActions.Add(action);
            }
        }

        static AutoResetEvent _OutActionAcknowledgement = new AutoResetEvent(true);

        private static ChromeActionRequest _LastSentAction, _LastSuccessfulAction;

        static void RunStdOutWriter()
        {
            try
            {
                while (true)
                {
                    if (_OutBoundBufferedActions.Count > 0)
                    {
                        if (_OutActionAcknowledgement.WaitOne(10000))
                        {
                            if (_OutBoundBufferedActions.Count > 0)
                            {
                                _LastSentAction = _OutBoundBufferedActions[0];
                                lock (_OutBoundBufferedActions)
                                    _OutBoundBufferedActions.RemoveAt(0);
                                SendChromeAction(_OutWriter, _LastSentAction);
                            }
                        }
                        else
                            Reset("Time-Out Waiting for Result!!");
                    }
                    else
                        Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.Error(ex);
            }
        }

        static void Reset(string Tag)
        {
            int _ActionCount = 0;

            lock (_OutBoundBufferedActions)
            {
                _ActionCount = _OutBoundBufferedActions.Count;
                _OutBoundBufferedActions.Clear();
                _OutActionAcknowledgement.Set();
            }
#if DEBUG
            ConsoleLogger.Info(Tag + " Task Failed with ActionCount:" + _ActionCount.ToString());
#endif
        }

        static void SendChromeAction(BinaryWriter _OutWriter, ChromeActionRequest action)
        {

            try
            {
                //ChromeNativeConsoleLogger.Info(string.Format("Executing request {0}{1}{2} on URL:{3}", action.Item5, action.Item6, action.Item7, action.Item4));

                byte[] _bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(action));
                //byte[] _bytes = UTF8Encoding.UTF8.GetBytes("\"hELLO tEST\"");
#if DEBUG
                ConsoleLogger.Info("Executing request" + Encoding.UTF8.GetString(_bytes) +
                                               _bytes.Length);
#endif
                _OutWriter.Write(_bytes.Length);
                _OutWriter.Write(_bytes);
                _OutWriter.Flush();
            }
            catch (Exception ex)
            {
                ConsoleLogger.Error(ex);
            }


        }

    }
}