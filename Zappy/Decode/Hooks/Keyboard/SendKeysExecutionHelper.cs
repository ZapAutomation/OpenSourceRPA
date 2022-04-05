using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.Decode.Hooks.Keyboard
{
    static class SendKeysExecutionHelper
    {
        public static void InvokeSendKeys(ZappyTaskControl uITaskControl, SendKeysAction zappySendKeysAction)
        {
                        string text = zappySendKeysAction.Text;
            if (string.IsNullOrEmpty(text))
                return;
            string dataToDecode = text;
            dataToDecode = Utility.ConvertModiferKeysToString(zappySendKeysAction.ModifierKeys, dataToDecode);

            if (uITaskControl != null)
            {
                                                                                                                for (int i = 0; i < dataToDecode.Length; i++)
                {
                    NativeMethods.SendMessage(uITaskControl.WindowHandle, 0x0102, (int)dataToDecode[i],
                        0);
                }
            }
                                                                                                

                                                

        }
    }
}
