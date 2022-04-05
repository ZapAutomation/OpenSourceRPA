using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Graph;
using Zappy.Helpers;


namespace Crapy.Analytics
{
    public partial class TaskEditor : Form
    {
        public TaskEditor()
        {
            InitializeComponent();
        }

        private void cmdClearAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _Activities.Count; i++)
            {
                //_Activities[i].Select = false;
            }
            _Activities.ResetBindings();
        }

        private void lblExecute_Click(object sender, EventArgs e)
        {
            this.dataGridView1.EndEdit();

            ZappyTask _UitaskSelectedTasks = CreateTask();
            if (_UitaskSelectedTasks != null)
            {
                string _CustomExecutionTaskFileName = "CustomActivities.zappy";

                if (_UitaskSelectedTasks != null && _UitaskSelectedTasks.ExecuteActivities.Count > 0)
                {
                    string _FileName = CrapyWriter.Save(_UitaskSelectedTasks,
                        Path.Combine(CrapyConstants.SavedTaskFolder, "ZappyCustomTaskEditor"),
                        _CustomExecutionTaskFileName);
                    CommonProgram.StartPlaybackFromFile(_FileName);
                }
                else
                    MessageBox.Show("No Action to Execute task!!");
            }
        }

        private void lblExport_Click(object sender, EventArgs e)
        {
            try
            {
                this.dataGridView1.EndEdit();
                ZappyTask _UitaskSelectedTasks = CreateTask(); //new ZappyTask();
                if (_UitaskSelectedTasks != null && _UitaskSelectedTasks.ExecuteActivities.Count > 0)
                {
                    using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
                    {
                        saveFileDialog1.Filter = "Zappy Files|*.zappy";
                        saveFileDialog1.FilterIndex = 2;
                        saveFileDialog1.RestoreDirectory = true;
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            using (Stream myStream = saveFileDialog1.OpenFile())
                                _UitaskSelectedTasks.Save(myStream, false);
                        }
                    }
                }
                else
                    MessageBox.Show("No Action to Export!!");
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        ZappyTask CreateTask()
        {
            List<IZappyAction> _SelectedActivities = new List<IZappyAction>();
            int _RepeatCount = 1;
            if (!string.IsNullOrEmpty(cmbRepeatCount.Text))
            {
                if (!int.TryParse(cmbRepeatCount.Text.Trim(), out _RepeatCount))
                    _RepeatCount = 1;
            }

            cmbRepeatCount.SelectedIndex = 0;

            for (int j = 0; j < _RepeatCount; j++)
            {
                for (int i = 0; i < _Activities.Count; i++)
                {
                    if (_Activities[i].Select)
                        _SelectedActivities.Add(_Activities[i].TaskAction);
                }
            }

            if (_SelectedActivities.Count > 0)
                return new ZappyTask(_SelectedActivities);
            else
            {
                //MessageBox.Show("No Action Selected!");
                return null;
            }
        }


        //private SortableBindingList<TaskEditorDataObject> _Activities;


        public void Initialize()
        {
            List<IZappyAction> _RecordedEvents = InternalNodeGenerator.NodeGraphBuilder.RecordedEvents;

            int _RecordedActivitiesCount = _RecordedEvents.Count;

           _Activities = new SortableBindingList<TaskEditorDataObject>();
            

            for (int i = 0; i < _RecordedActivitiesCount; i++)
            {
                if (_RecordedEvents.Count > i)
                {
                    
                    IZappyAction _Action = _RecordedEvents[i];
                    Type _ActionType = _Action.GetType();
                    if (!String.IsNullOrEmpty(_Action.ExeName))
                    {
                        TaskEditorDataObject _AnalyticsDataObject = new TaskEditorDataObject()
                        {
                            TaskAction = _Action,
                            Process = _Action.ExeName
                        };
                        if (Array.IndexOf(GraphBuilder._IgnoredActivities, _ActionType) < 0)
                        {
                            _Activities.Add(_AnalyticsDataObject);
                        }
                    }
                }
            }
        }
        private void TaskEditor_Load(object sender, EventArgs e)
        {

            this.dataGridView1.DataSource = _Activities;
            this.dataGridView1.ReadOnly = false;
            this.dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.Columns["Time"].DefaultCellStyle.Format = "HH:mm:ss";
            this.dataGridView1.Columns["Time"].ReadOnly =
                this.dataGridView1.Columns["Process"].ReadOnly =
                    this.dataGridView1.Columns["ScreenElement"].ReadOnly =
                        this.dataGridView1.Columns["Value"].ReadOnly =
                            this.dataGridView1.Columns["Action"].ReadOnly = true;

            cmbRepeatCount.SelectedIndex = 0;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void cmdShowTest_Click(object sender, EventArgs e)
        {
            bool _DisplayText = cmdShowTest.Text == "Show Text";
            cmdShowTest.Text = _DisplayText ? "Hide Text" : "Show Text";

            for (int i = 0; i < _Activities.Count; i++)
            {
                _Activities[i].DisplayFullText = _DisplayText;
            }

            _Activities.ResetBindings();
        }
    }
}
