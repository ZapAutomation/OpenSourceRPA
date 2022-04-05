using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Graph;
using Zappy.Helpers;
using Zappy.Plugins.ChromeBrowser.Chrome;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.Trapy;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Invoker
{
    public class LearnedActions
    {
        private static string messageToSend = String.Empty;

        public static void ExportZappyLearnedActivities()
        {
            if (!string.IsNullOrEmpty(ClientUI._LearnedActivitiesFilename))
            {
                ZappyTask _UitaskSelectedTasks = ZappyTask.Create(ClientUI._LearnedActivitiesFilename);
                using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
                {
                    saveFileDialog1.Filter = "Zappy Files|*.zappy";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (Stream myStream = saveFileDialog1.OpenFile())
                            _UitaskSelectedTasks.Save(myStream, saveFileDialog1.FileName, false);
                    }
                }
            }
        }

        private static bool ignoreZappyLearningActivities(IZappyAction Action)
        {
            ZappyTaskAction _Action = Action as ZappyTaskAction;

            if (_Action != null)
            {
                //removing toolbar actions
                if (!string.IsNullOrEmpty(_Action.ActivityElement?.ClassName))
                    if (_Action.ActivityElement.ClassName == "ToolbarWindow32")
                        return true;
                //Allow it if selected from settings??
                if (!ApplicationSettingProperties.Instance.EnableLaunchActivityRecording)
                {
                    if (_Action is LaunchApplicationAction)
                        return true;
                }
            }



            return false;
        }


        static long _ActionPlaceholder;
        public static string CreateLearnedActions(string activityName = "LearnedActivities.zappy", bool showLearningActionsUI = true,
            long actionID = -1)
        {
            try
            {

                if (ClientUI._TaskRecording)
                {

                    long _LastGeneratedTaskID = actionID == -1 ? ActionIDRegister.PeekUniqueId() : actionID;

                    int _ActionCount = InternalNodeGenerator.NodeGraphBuilder.RecordedEvents.Count;
                    List<IZappyAction> _RecordedEvents = InternalNodeGenerator.NodeGraphBuilder.RecordedEvents;
                    List<IZappyAction> _RecordedTaskActivities =
                        new List<IZappyAction>((int)(_LastGeneratedTaskID - _ActionPlaceholder) + 1);

                    for (int i = 0; i < _ActionCount; i++)
                    {

                        IZappyAction _Action = _RecordedEvents[i];
                        //CrapyLogger.log.Error(_Action.ToString());
                        if (_Action.Id > _ActionPlaceholder)
                        {
                            if (_Action.Id <= _LastGeneratedTaskID)
                            {
                                if (!ignoreZappyLearningActivities(_Action))
                                    if (!String.IsNullOrEmpty(_Action.ExeName))
                                        _RecordedTaskActivities.Add(_Action);
                            }
                            else break;
                        }
                    }
                    if (_RecordedTaskActivities.Count > 0)
                    {
                        FilterAndAttachRecordedActivites(_RecordedTaskActivities, activityName);
                    }
                    else
                    {
                        messageToSend = "No Recording Actions Performed";
                    }
                    if (messageToSend.Equals(String.Empty))
                    {
                        messageToSend = "Stop Recording";
                    }
                    ZappyInvoker.ShowNotificationZappy(messageToSend);
                    messageToSend = String.Empty;
                }
                //Start Learning
                else
                {
                    //Start Learning
                    //TrapyService.Start();
                    _ActionPlaceholder = actionID == -1 ? ActionIDRegister.PeekUniqueId() : actionID;
                    ZappyInvoker.ShowNotificationZappy("Start Recording");
                }

                ClientUI._TaskRecording = !ClientUI._TaskRecording;

                if (Properties.Settings.Default.ShowRecordedStepNotification && showLearningActionsUI)
                {
                    if (ClientUI._TaskRecording)
                        frmActionLearner.LearningStepInstance.ShowInactiveTopmost();
                    else
                        frmActionLearner.LearningStepInstance.Hide();
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                ClientUI._TaskRecording = false;
                MessageBox.Show("Task Recording Failed");
            }
            return ClientUI._LearnedActivitiesFilename;
        }

        public static void FilterAndAttachRecordedActivites(List<IZappyAction> _RecordedTaskActivities, string activityName)
        {

            string _ProcessFileName = Path.GetFileNameWithoutExtension(_RecordedTaskActivities[0].ExeName);
            string _DirName;
            string MainWindowName;

            //Removing Alt+Ctrl+s if it is the first action
            if (_RecordedTaskActivities.Count > 1 && _RecordedTaskActivities[0] is SendKeysAction)
            {
                SendKeysAction recordedSendKeysAction = _RecordedTaskActivities[0] as SendKeysAction;
                if (recordedSendKeysAction.ModifierKeys ==
                    (ModifierKeys.Alt | ModifierKeys.Control))
                {
                    if (recordedSendKeysAction.ValueAsString.Equals("s"))
                    {
                        _RecordedTaskActivities.RemoveAt(0);
                    }
                }
            }
            _RecordedTaskActivities = applyCustomFilters(_RecordedTaskActivities);

            if (_RecordedTaskActivities.Count > 0)
            {
                if (_RecordedTaskActivities[0] is ChromeAction)
                {
                    _DirName = Path.Combine(CrapyConstants.SavedTaskFolder, _ProcessFileName);
                    MainWindowName = string.Empty;
                }
                else //(_RecordedTaskActivities[0] is ZappyTaskAction)
                {
                    ZappyTaskAction zaction = _RecordedTaskActivities[0] as ZappyTaskAction;
                    if (!string.IsNullOrEmpty(zaction.ZappyWindowTitle))
                        MainWindowName = zaction.ZappyWindowTitle;
                    else
                        MainWindowName = zaction.ActivityElement
                            ?.TopLevelElement.Name;

                    if (MainWindowName == null)
                    {
                        messageToSend = "Undefined main window";
                        MainWindowName = "Undefined";
                    }

                    string MainWindowNameWithoutInvaidChars = CrapyWriter.r.Replace(MainWindowName, "");

                    string ApplicationAndWindow =
                        Path.Combine(_ProcessFileName, MainWindowNameWithoutInvaidChars);
                    _DirName = Path.Combine(CrapyConstants.SavedTaskFolder, ApplicationAndWindow);

                }

                if (!Directory.Exists(_DirName))
                {
                    Directory.CreateDirectory(_DirName);
                }

                ZappyTask _thisTask = new ZappyTask(_RecordedTaskActivities);
                ClientUI._LearnedActivitiesFilename =
                    Path.Combine(_DirName, activityName); //saveFileDialog1.FileName

                CrapyWriter.SaveWindowInformationRecordedTask(_DirName, ClientUI._LearnedActivitiesFilename,
                    _ProcessFileName, MainWindowName,
                    _thisTask);
                ZappyInvoker.AddTask(_DirName, ClientUI._LearnedActivitiesFilename, _thisTask,
                    _RecordedTaskActivities[0]);
            }
            else
            {
                messageToSend = "No Recording Actions Performed";
            }
        }

        private static List<IZappyAction> applyCustomFilters(List<IZappyAction> recordedTaskActivities)
        {
            try
            {
                LearnedActionFilters learnedActionFilters = new LearnedActionFilters();
                if (!ApplicationSettingProperties.Instance.ExcelRecordMouseAction)
                {
                    recordedTaskActivities = learnedActionFilters.removeUnnessaryMouseActions(recordedTaskActivities);
                }
                if (!ApplicationSettingProperties.Instance.EnableChromeNativeRecording)
                {
                    recordedTaskActivities = learnedActionFilters.removeChromeNativeActions(recordedTaskActivities);
                }
                recordedTaskActivities = learnedActionFilters.removeExcelEmptyActionsAndAddSetFocus(recordedTaskActivities);
                recordedTaskActivities = learnedActionFilters.createChromeOpenActionCommand(recordedTaskActivities);
                recordedTaskActivities = learnedActionFilters.getPDFSearchActivity(recordedTaskActivities);
                if (ApplicationSettingProperties.Instance.ChromeXPAthOnlyRecord)
                    learnedActionFilters.useChromeXPath(recordedTaskActivities);
                if (ApplicationSettingProperties.Instance.ChromeInsertPause)
                    learnedActionFilters.insertPauseChromeActions(recordedTaskActivities);
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }

            return recordedTaskActivities;
        }

    }
}