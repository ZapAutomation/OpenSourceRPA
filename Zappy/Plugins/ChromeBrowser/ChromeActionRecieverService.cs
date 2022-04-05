using System;
using System.Linq;
using Zappy.Helpers;
using Zappy.Plugins.ChromeBrowser.Chrome;
using Zappy.Plugins.ChromeBrowser.Chrome.Helper;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Core;
using ZappyMessages;
using ZappyMessages.Chrome;
using ZappyMessages.Helpers;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.Plugins.ChromeBrowser
{
    public static class ChromeActionRecieverService
    {

        public static Subscription _ChromeActionSubscription;

        public static event EventHandler<string> TblClickEvent;
        public static void Init()
        {
            _ChromeActionSubscription = new Subscription();
            _ChromeActionSubscription.DataPublished += _ChromeActionSubscription_DataPublished;
            PubSubService.Instance.Subscribe("ZappyChromeActionReciever", _ChromeActionSubscription, new int[] { PubSubTopicRegister.Chrome2ZappyActionPlayback });

        }

        private static void _ChromeActionSubscription_DataPublished(int arg1, string arg2)
        {
            
            BaseChromeAction baseChromeAction = ZappySerializer.DeserializeObject<BaseChromeAction>(arg2);
            ChromeAction(baseChromeAction);
        }

        public static void ChromeAction(BaseChromeAction Action)
        {

                        if (String.IsNullOrEmpty(Action.CommandName))
            {
                return;
            }

            IZappyAction _Action;
            switch (Action.CommandName)
            {
                                case "open":
                    return;
                                                                                                case "selectFrame":
                    _Action = new ChromeAction_selectFrame();
                    break;
                                                                case "clickAt":
                                                            _Action = new ChromeMouseAction(ChromeMouseOptions.clickAt);
                    break;
                case "doubleClickAt":
                    _Action = new ChromeMouseAction(ChromeMouseOptions.doubleClickAt);
                    break;
                case "mouseDownAt":
                    return;
                                                case "mouseMoveAt":
                    return;
                                                case "mouseOut":
                    return;
                                                                case "mouseOver":
                    return;
                                        break;
                case "mouseUp":
                    return;
                case "mouseDown":
                    return;
                case "mouseUpAt":
                    return;
                                        break;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                case "dragAndDropToObject":
                    _Action = new ChromeAction_dragAndDropToObject();
                    break;
                case "sendKeys":
                    _Action = new ChromeAction_sendKeys();
                                                                                                                        break;
                case "submit":
                    _Action = new ChromeAction_submit();
                    break;
                case "type":
                    _Action = new ChromeAction_type();
                    break;
                case "editContent":
                    _Action = new ChromeAction_editContent();
                    break;
                                                                                                                case "select":
                    _Action = new ChromeAction_select();
                    break;
                                                                                                                                                                                                                                                                                                                                                                                                                case "pause":
                    _Action = new PauseAction();
                    break;
                case "runScript":
                    _Action = new ChromeAction_runScript();
                    break;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                default:
                    _Action = new ChromeAction(Action.CommandName);
                    break;
            }

            if (_Action is ChromeAction cAction)
            {
                cAction.CommandTarget = Action.CommandTarget;
                cAction.CommandValue = string.Empty;
                cAction.CommandValue.Value = Action.CommandValue;
                                cAction.ActionUrlTab = Action.ActionUrlTab;
                cAction.TabID = Action.TabID;
                cAction.WindowID = Action.WindowID;
                                cAction.ZappyWindowTitle = Action.ActionWindowtitle;

                                if (TblClickEvent != null)
                {
                    if ((Action.CommandName) == "clickAt")
                    {
                        bool tableFound = false;
                                                for (int i = 0; i < cAction.CommandTarget.Count; i++)
                        {
                            if (cAction.CommandTarget[i].Contains("table"))
                            {
                                TblClickEvent.Invoke(null, cAction.CommandTarget[i]);
                                tableFound = true;
                                break;
                            }
                        }
                        if (!tableFound)
                            TblClickEvent.Invoke(null, cAction.CommandTarget.First());
                    }
                }
            }
                                                                                                                                                            InternalNodeGenerator.AddAction(_Action);
        }
    }
}