using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Zappy.Invoker;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages;
using ZappyMessages.Helpers;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;
using ZappyMessages.Triggers;

namespace Zappy.Helpers
{
    public partial class frmTriggerManager : Form, IPubSubSubscriber
    {
        //private const string AutorunTask = "Auto Run Zappy Tasks";

        //private static Dictionary<string, string> runSelectedZappyActionTrigger { get; set; }
        public static List<TriggerResponseInfo> _RegisteredTriggers;

        public frmTriggerManager()
        {
            InitializeComponent();
            //Gives cross thread exception
            //triggerListView.ItemChecked += TriggerListView_ItemChecked;
        }

        public void OnPublished(int channel, string PublishedString)
        {
            //publish = StringCipher.Decrypt(publish, ZappyMessagingConstants.MessageKey);

            if (channel == PubSubTopicRegister.ActiveTriggersReponse)
            {
                //_TriggerRegisteredTasks = ZappySerializer.DeserializeObject<Dictionary<IZappyAction, ZappyTask>>(
                //    PublishedString);
                //foreach (IZappyAction trigger in _TriggerRegisteredTasks.Keys)
                //{
                //    ListViewItem listViewItem = new ListViewItem();
                //    listViewItem.Tag = trigger;
                //    listViewItem.Text = trigger.DisplayName;
                //    triggerListView.Items.Add(listViewItem);
                //}
                _RegisteredTriggers = ZappySerializer.DeserializeObject<List<TriggerResponseInfo>>(PublishedString);

                UpdateTriggerList();

                //Add published messages to list
            }
            //throw new NotImplementedException();
        }

        public void OnPublishedBinary(int channel, byte[] PublishedBinaryData)
        {
            //throw new NotImplementedException();
        }

        public void PingClient()
        {
            //throw new NotImplementedException();
        }

        private void UpdateTriggerList()
        {
            if (triggerListView.Items.Count > 0)
            {
                //Hold the first item and release the rest
                //var tempItem = triggerListView.Items[0];
                triggerListView.Clear();

                //If fixed item then add it back else continue
                //if (tempItem.Text.Equals(AutorunTask))
                //    triggerListView.Items.Add(tempItem);
            }

            foreach (var triggerInfo in _RegisteredTriggers)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Tag = triggerInfo.TriggerAction as IZappyAction;
                listViewItem.Text = string.IsNullOrEmpty(triggerInfo.TriggerName)
                    ? HelperFunctions.HumanizeNameForIZappyAction((IZappyAction)triggerInfo.TriggerAction)
                    : Path.GetFileNameWithoutExtension(triggerInfo.TriggerName);
                triggerListView.Items.Add(listViewItem);
            }
        }

        private void frmTriggerManager_Load(object sender, EventArgs e)
        {            
            triggerListView.MultiSelect = false;

            sendRequestForActiveTriggers();

            PubSubService.Instance.Subscribe("ZappySubscriber", this,
                new int[] { PubSubTopicRegister.ActiveTriggersReponse });
            //Auto for robot hub
            //Items.Add(runSelectedZappyActionTrigger);
        }

        private void sendRequestForActiveTriggers()
        {
            //send request for active triggers to playback helper
            PubSubService.Instance.Publish(
                PubSubTopicRegister.ActiveTriggersRequest,
                PubSubMessages.TriggerRequestMessage);
        }

        private void triggerListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (triggerListView.SelectedItems.Count > 0)
                propertyGrid1.SelectedObject = triggerListView.SelectedItems[0].Tag;
        }

        private void TriggerListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var action = new Action(() =>
            {
                if (e.Item.Checked && !(e.Item.Tag is IZappyAction))
                {
                    e.Item.Checked = false;
                }
            });

            if (triggerListView.InvokeRequired)
                triggerListView.Invoke(action);
            else
                action();
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            var triggerList = new List<IZappyAction>();
            foreach(var trigger in _RegisteredTriggers)
            {
                triggerList.Add(trigger.TriggerAction as IZappyAction);
            }
            try
            {
                //File.WriteAllText(ZappyMessagingConstants.AutoExecFileLoc,
                //    ZappySerializer.SerializeObject(ZappyInvoker.runSelectedZappyActionTrigger));

                //send request for triggers which are inactivated
                PubSubService.Instance.Publish(
                    PubSubTopicRegister.ActiveTriggersRequest,
                    ZappySerializer.SerializeObject(
                        new TriggerRequestInfo
                        {
                            RequestType = TriggerRequest.Update,
                            RequestBody = ZappySerializer.SerializeObject(triggerList)
                        }));
            }
            catch
            {
                MessageBox.Show("Error Setting Requested Property");
            }

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            var checkedItems = triggerListView.CheckedItems;
            var deletedTasks = new List<IZappyAction>();

            if (checkedItems.Count > 0)
            {
                foreach (ListViewItem item in checkedItems)
                {
                    if (item.Tag is IZappyAction)
                        deletedTasks.Add((IZappyAction)item.Tag);
                }
            }
            //Why??
            //else if (triggerListView.SelectedItems.Count > 0)
            //{
            //    var selectedItem = triggerListView.SelectedItems[0];

            //    if (selectedItem.Tag is IZappyAction)
            //        deletedTasks.Add((IZappyAction)selectedItem.Tag);
            //}

            if (deletedTasks.Count>0)
            {
                
                //File.WriteAllText(ZappyMessagingConstants.AutoExecFileLoc,
                //    ZappySerializer.SerializeObject(ZappyInvoker.runSelectedZappyActionTrigger));

                //Report PlaybackHelper of all the deleted triggers

                //TODO get a response and then only delete these things

                PubSubService.Instance.Publish(
                    PubSubTopicRegister.ActiveTriggersRequest,
                    ZappySerializer.SerializeObject(
                        new TriggerRequestInfo
                        {
                            RequestType = TriggerRequest.Delete,
                            RequestBody = ZappySerializer.SerializeObject(deletedTasks)
                        }));
                _RegisteredTriggers.RemoveAll(r => deletedTasks.Contains((IZappyAction)r.TriggerAction));
                UpdateTriggerList();
                //wait for a bit and then send this
                //sendRequestForActiveTriggers();
            }          
        }

    }
}
