using System;
using System.IO;
using System.Windows.Forms;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.Invoker;
using ZappyMessages;
using ZappyMessages.Helpers;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.Decode.Hooks
{
    public class HotKeyHandler
    {
        public static HotKeyHandler Instance = new HotKeyHandler();

        static HotKeyHandler()
        {
            if (File.Exists(CrapyConstants.ZappyShortcuts))
            {
                try
                {
                    Instance =
                        ZappySerializer.DeserializeObject<HotKeyHandler>(File.ReadAllText(CrapyConstants.ZappyShortcuts));
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.Error(ex);
                    Instance = new HotKeyHandler();
                }
            }
            else
            {
                Instance = new HotKeyHandler();
            }
        }
        private DateTime timeEsclapsed;

        internal ModifierKeys ZappyDefaultModifierKeys = ModifierKeys.Alt | ModifierKeys.Control;
        public Keys learnedActionsKey { get; set; }
        public Keys executeFirstTaskKey { get; set; }
        public Keys cancelExecutionKey { get; set; }
        public Keys exportLearnedActions { get; set; }

        public HotKeyHandler()
        {
            timeEsclapsed = DateTime.Now;
            setDefaultKeys();

        }

        public void setDefaultKeys()
        {
            learnedActionsKey = Keys.S;
            executeFirstTaskKey = Keys.Z;
            cancelExecutionKey = Keys.K;
            exportLearnedActions = Keys.E;
        }


        public bool raiseHotKeyEvents(KeyboardAction keyInfo, bool hotKeysEnabled)
        {
            if (keyInfo.ModifierKeys == ZappyDefaultModifierKeys && keyInfo.Key == cancelExecutionKey)
            {
                PubSubService.Instance.Publish(PubSubTopicRegister.ControlSignals, PubSubMessages.CancelZappyExecutionMessage);
            }
            if (hotKeysEnabled)
            {
                bool isHotKeyCombination = true;
                if (keyInfo.ModifierKeys == ZappyDefaultModifierKeys && keyInfo.Key == executeFirstTaskKey)
                {
                    if (DateTime.Now > timeEsclapsed.AddMilliseconds(300))
                    {
                        Program.UI_Instance.BeginInvoke(
                            new Action(() => ZappyInvoker.ExecuteFirstZappyTask()));
                        timeEsclapsed = DateTime.Now;
                    }
                }
                else if (keyInfo.ModifierKeys == ZappyDefaultModifierKeys && keyInfo.Key == cancelExecutionKey)
                {
                                                        }
                else if (keyInfo.ModifierKeys == ZappyDefaultModifierKeys && keyInfo.Key == learnedActionsKey)

                {
                    if (DateTime.Now > timeEsclapsed.AddMilliseconds(300))
                    {
                        Program.UI_Instance.BeginInvoke(
                                                new Action(() => LearnedActions.CreateLearnedActions()));
                        timeEsclapsed = DateTime.Now;
                    }
                }
                else if (keyInfo.ModifierKeys == ZappyDefaultModifierKeys && keyInfo.Key == exportLearnedActions)
                {
                    if (DateTime.Now > timeEsclapsed.AddMilliseconds(300))
                    {
                        Program.UI_Instance.BeginInvoke(
                            new Action(() => LearnedActions.ExportZappyLearnedActivities()));
                        timeEsclapsed = DateTime.Now;
                    }
                }
                else
                {
                    isHotKeyCombination = false;
                }
                return isHotKeyCombination;
            }
            return false;
        }
    }
}
