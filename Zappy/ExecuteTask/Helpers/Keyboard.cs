using System;
using System.Collections.Generic;
using System.Text;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Helpers
{
    public class Keyboard : IDisposable
    {
        private static Keyboard instance;
        private static List<char> listOfSpecialChar = GetSpecialCharater();
        private static readonly object lockObject = new object();

        protected Keyboard()
        {
        }

        internal static void Cleanup()
        {
            object lockObject = Keyboard.lockObject;
            lock (lockObject)
            {
                if (instance != null)
                {
                    instance.Dispose();
                    instance = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && ActionExecutorManager.Instance.ActionExecutors != null)
            {
                foreach (ZappyTaskActionExecutor executor in ActionExecutorManager.Instance.ActionExecutors)
                {
                    try
                    {
                        executor.ReleaseKeyboard();
                    }
                    catch (Exception)
                    {
                        object[] args = { executor.GetType().Name };
                        
                    }
                }
                ZappyTaskActionExecutorCore.Instance.ReleaseKeyboard();
            }
        }

        ~Keyboard()
        {
            Dispose(false);
        }

        private static List<char> GetSpecialCharater() =>
            new List<char> {
                '{',
                '}',
                '+',
                '%',
                '^',
                '#',
                '(',
                ')'
            };

        internal static string HandleSpecialCharacters(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            if (str.IndexOfAny(listOfSpecialChar.ToArray()) == -1)
            {
                return str;
            }
            StringBuilder builder = new StringBuilder();
            foreach (char ch in str)
            {
                if (listOfSpecialChar.Contains(ch))
                {
                    builder.Append("{");
                    builder.Append(ch);
                    builder.Append("}");
                }
                else
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }

        public static void PressModifierKeys(ModifierKeys keys)
        {
            PressModifierKeys(null, keys);
        }

        public static void PressModifierKeys(ZappyTaskControl control, ModifierKeys keys)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                    Instance.PressModifierKeysImplementation(control, keys);
                }
                return null;
            }, control, true, true);
        }

        protected virtual void PressModifierKeysImplementation(ZappyTaskControl control, ModifierKeys keys)
        {
            object[] args = { keys };
            
            try
            {
                ZappyTaskActionExecutor actionExecutor = ActionExecutorManager.Instance.GetActionExecutor(control);
                if (actionExecutor == null)
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().PressModifierKeys(control, keys);
                }
                else
                {
                    actionExecutor.PressModifierKeys(ALUtility.GetTechElementFromZappyTaskControl(control), keys);
                }
            }
            catch (Exception exception)
            {
                if (!ALUtility.RetryUsingDefaultExecutor(exception))
                {
                    throw;
                }
                ActionExecutorManager.Instance.GetDefaultExecutor().PressModifierKeys(control, keys);
            }
            finally
            {
                Execute.ExecutionHandler.WaitForDelayBetweenActivities();
            }
        }

        public static void ReleaseModifierKeys(ModifierKeys keys)
        {
            ReleaseModifierKeys(null, keys);
        }

        public static void ReleaseModifierKeys(ZappyTaskControl control, ModifierKeys keys)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                    Instance.ReleaseModifierKeysImplementation(control, keys);
                }
                return null;
            }, control, true, true);
        }

        protected virtual void ReleaseModifierKeysImplementation(ZappyTaskControl control, ModifierKeys keys)
        {
            object[] args = { keys };
            
            try
            {
                ZappyTaskActionExecutor actionExecutor = ActionExecutorManager.Instance.GetActionExecutor(control);
                if (actionExecutor == null)
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().ReleaseModifierKeys(control, keys);
                }
                else
                {
                    actionExecutor.ReleaseModifierKeys(ALUtility.GetTechElementFromZappyTaskControl(control), keys);
                }
            }
            catch (Exception exception)
            {
                if (!ALUtility.RetryUsingDefaultExecutor(exception))
                {
                    throw;
                }
                ActionExecutorManager.Instance.GetDefaultExecutor().ReleaseModifierKeys(control, keys);
            }
            finally
            {
                Execute.ExecutionHandler.WaitForDelayBetweenActivities();
            }
        }

        public static void SendKeys(string text)
        {
            SendKeys(null, text, ModifierKeys.None, false, true);
        }

        public static void SendKeys(ZappyTaskControl control, string text)
        {
            SendKeys(control, text, ModifierKeys.None, false, true);
        }

        public static void SendKeys(string text, bool isEncoded)
        {
            SendKeys(null, text, ModifierKeys.None, isEncoded, true);
        }

        public static void SendKeys(string text, ModifierKeys modifierKeys)
        {
            SendKeys(null, text, modifierKeys, false, true);
        }

        public static void SendKeys(ZappyTaskControl control, string text, bool isEncoded)
        {
            SendKeys(control, text, ModifierKeys.None, isEncoded, true);
        }

        public static void SendKeys(ZappyTaskControl control, string text, ModifierKeys modifierKeys)
        {
            SendKeys(control, text, modifierKeys, false, true);
        }

        public static void SendKeys(string text, ModifierKeys modifierKeys, bool isEncoded)
        {
            SendKeys(null, text, modifierKeys, isEncoded, true);
        }

        public static void SendKeys(ZappyTaskControl control, string text, ModifierKeys modifierKeys, bool isEncoded)
        {
            SendKeys(control, text, modifierKeys, isEncoded, true);
        }

        public static void SendKeys(string text, ModifierKeys modifierKeys, bool isEncoded, bool isUnicode)
        {
            SendKeys(null, text, modifierKeys, isEncoded, isUnicode);
        }

        public static void SendKeys(ZappyTaskControl control, string text, ModifierKeys modifierKeys, bool isEncoded, bool isUnicode)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                                        Instance.SendKeysImplementation(control, text, modifierKeys, isEncoded, isUnicode);
                }
                return null;
            }, control, true, true);
        }

        protected virtual void SendKeysImplementation(ZappyTaskControl control, string text, ModifierKeys modifierKeys, bool isEncoded, bool isUnicode)
        {
                                                            int? nullable = null;
            string dataToDecode = text;
            try
            {
                ZappyTaskActionExecutor actionExecutor = ActionExecutorManager.Instance.GetActionExecutor(control);
                if (actionExecutor == null)
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().SendKeys(control, text, modifierKeys, isUnicode, isEncoded);
                }
                else
                {
                    if (isEncoded)
                    {
                        nullable = Utility.DisablePlaybackLoggingContent(ScreenElement.Playback);
                        dataToDecode = EncodeDecode.DecodeString(dataToDecode);
                    }
                    dataToDecode = Utility.ConvertModiferKeysToString(modifierKeys, dataToDecode);
                    TaskActivityElement techElementFromZappyTaskControl = ALUtility.GetTechElementFromZappyTaskControl(control);
                    if (isEncoded && techElementFromZappyTaskControl != null && !techElementFromZappyTaskControl.IsPassword)
                    {
                        throw new ArgumentException(Resources.EncryptedText);
                    }
                    actionExecutor.SendKeys(techElementFromZappyTaskControl, dataToDecode, modifierKeys, isUnicode);
                }
            }
            catch (Exception exception)
            {
                if (ALUtility.RetryUsingDefaultExecutor(exception))
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().SendKeys(control, text, modifierKeys, isUnicode, isEncoded);
                }
                else
                {
                    if (isEncoded)
                    {
                        Execute.ExecutionHandler.MapAndThrowException(exception, "SendKeys", control);
                    }
                    throw;
                }
            }
            finally
            {
                if ((nullable.HasValue && nullable.HasValue) & isEncoded)
                {
                    ScreenElement.Playback.SetLoggingFlag(-1);
                }
                Execute.ExecutionHandler.WaitForDelayBetweenActivities();
            }
        }

        public static Keyboard Instance
        {
            get
            {
                if (instance == null)
                {
                    object lockObject = Keyboard.lockObject;
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new Keyboard();
                        }
                    }
                }
                return instance;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                instance = value;
            }
        }

        public static int SendKeysDelay
        {
            get =>
                Execute.ExecutionHandler.Settings.DelayBetweenSendKeys;
            set
            {
                Execute.ExecutionHandler.Settings.DelayBetweenSendKeys = value;
            }
        }
    }
}