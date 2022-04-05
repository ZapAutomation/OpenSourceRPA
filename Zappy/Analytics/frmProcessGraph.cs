using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using SharedZappyInterface;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.Invoker;
using ZappyMessages.Helpers;
using Color = Microsoft.Msagl.Drawing.Color;

namespace Zappy.Analytics
{
    internal partial class frmProcessGraph : Form
    {
        public frmProcessGraph()
        {
            InitializeComponent();
        }

        SortableBindingList<AnalyticsDataObject> _ActionNodes = new SortableBindingList<AnalyticsDataObject>();
        SortableBindingList<AnalyticsDataObject> _FilteredActionNodes = new SortableBindingList<AnalyticsDataObject>();

        Node _StartNode, _EndNode;
        DateTime _MinDateTime, _MaxDateTime;
        List<InternalNode> _ProcessNodeList;

        public void initData(IEnumerable<InternalNode> processnodes)
        {

            // GeometryGraph graph = new GeometryGraph();
            _ProcessNodeList = processnodes.ToList();

            _StartNode = new Node(GenerateNodeID());
            _StartNode.LabelText = "Begin";
            _StartNode.Attr.Shape = Shape.Diamond;
            _StartNode.Attr.FillColor = Color.Red;
            _StartNode.Label.FontColor = Color.White;

            _EndNode = new Node(GenerateNodeID());
            _EndNode.LabelText = "End";
            _EndNode.Attr.Shape = Shape.Diamond;
            _EndNode.Attr.FillColor = Color.Red;
            _EndNode.Label.FontColor = Color.White;

            DrawGraph(DateTime.MinValue, DateTime.MaxValue, _ActionNodes);
            this.dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.Columns["Time"].DefaultCellStyle.Format = "HH:mm:ss";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            if (_ActionNodes.Count > 2)
            {
                _MaxDateTime = _ActionNodes[_ActionNodes.Count - 1].Time;
                _MinDateTime = _ActionNodes[0].Time;
                int _TotalMinutes = Convert.ToInt32(Math.Ceiling((_MaxDateTime - _MinDateTime).TotalMinutes));
                tbrFrom.Trackbar.Maximum = tbrTo.Trackbar.Maximum = _TotalMinutes;
            }

            lblFrom.Text = _MinDateTime.ToString("\'From \'HH:mm");
            lblTo.Text = _MaxDateTime.ToString("\'To \'HH:mm");
        }

        List<Edge> _Edges = new List<Edge>();

        private void ClearNodeEdges(Node Node, List<Edge> Edges)
        {
            Edges.AddRange(Node.InEdges);
            for (int i = 0; i < Edges.Count; i++)
                Node.RemoveInEdge(Edges[i]);
            Edges.Clear();
            Edges.AddRange(Node.OutEdges);
            for (int i = 0; i < Edges.Count; i++)
                Node.RemoveOutEdge(Edges[i]);

            Edges.Clear();
            Edges.AddRange(Node.SelfEdges);
            for (int i = 0; i < Edges.Count; i++)
                Node.RemoveSelfEdge(Edges[i]);
            Edges.Clear();
        }


        void DrawGraph(DateTime From, DateTime To, SortableBindingList<AnalyticsDataObject> ActionNodes)
        {
            int _RetryCount = 0;
            bool _IsFirstRun = _ActionNodes.Count == 0;

            Dictionary<string, int> _ProcessTimeUtil = new Dictionary<string, int>();


            BEGIN_REFRESH:

            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");

            _ProcessTimeUtil.Clear();

            chart1.Series[0].Points.Clear();
            try
            {

                int _ProcessCount = 0;
                ClearNodeEdges(_StartNode, _Edges);
                ClearNodeEdges(_EndNode, _Edges);

                graph.AddNode(_StartNode);
                graph.AddNode(_EndNode);


                Node _PreviousNode = _StartNode;

                foreach (InternalNode _ProcessNode in _ProcessNodeList)
                {

                    int _MillisecsSpent;
                    string _ProcessName = _ProcessNode.AltText;
                    _ProcessTimeUtil.TryGetValue(_ProcessName, out _MillisecsSpent);
                    Color _ProcessColor = GetProcessColor(_ProcessCount);
                    _ProcessCount++;

                    Node _ProcessGraphNode = new Node(GenerateNodeID());
                    _ProcessGraphNode.LabelText = Path.GetFileNameWithoutExtension(_ProcessName);
                    _ProcessGraphNode.Attr.FillColor = _ProcessColor; //Color.LightGray;
                    _ProcessGraphNode.Attr.Shape = Shape.Diamond;

                    graph.AddNode(_ProcessGraphNode);
                    CreateEdge(_PreviousNode, _ProcessGraphNode, Color.Red);
                    _PreviousNode = _ProcessGraphNode;

                    foreach (InternalNode _WindowNode in _ProcessNode.Nodes)
                    {
                        _MillisecsSpent += _WindowNode.MillisecsSpent;
                        _ProcessTimeUtil[_ProcessName] = _MillisecsSpent;

                        //if (_WindowNode is GreyListNode)
                        //    continue;

                        var _WindowGraphNode = new Node(GenerateNodeID());
                        _WindowGraphNode.Attr.Shape = Shape.Box;
                        _WindowGraphNode.Attr.FillColor = Color.CornflowerBlue;
                        _WindowGraphNode.Label.FontColor = Color.White;

                        _WindowGraphNode.LabelText = _WindowNode.Text;
                        graph.AddNode(_WindowGraphNode);
                        CreateEdge(_ProcessGraphNode, _WindowGraphNode, Color.RoyalBlue);

                        Node _WindowStatsNode = new Node(GenerateNodeID());
                        _WindowStatsNode.Attr.Shape = Shape.Box;
                        _WindowStatsNode.Attr.FillColor = Color.Aqua;
                        List<Node> _WindowActionNodes = new List<Node>();
                        _WindowStatsNode.UserData = new Tuple<bool, List<Node>>(false, _WindowActionNodes);
                        graph.AddNode(_WindowStatsNode);
                        CreateEdge(_WindowGraphNode, _WindowStatsNode, Color.RoyalBlue);

                        int _KeystrokeCount = 0, _Other = 0;

                        foreach (InternalNode _ActionNode in _WindowNode.Nodes)
                        {
                            IZappyAction _Action = _ActionNode.Tag as IZappyAction;
                            if (_Action.Timestamp >= From && _Action.Timestamp <= To)
                            {
                                AnalyticsDataObject _AnalyticsDataObject = null;
                                if (_IsFirstRun)
                                {
                                    Node _ActionGraphNode = new Node(GenerateNodeID());

                                    _ActionGraphNode.LabelText =
                                        DisplayHelper.NodeTextHelper(_Action, true) + " (" +
                                        DisplayHelper.GetActionElementName(_Action) + ")";
                                    _ActionGraphNode.Attr.Shape = Shape.Box;
                                    _ActionGraphNode.Attr.FillColor = Color.LightBlue;

                                    _AnalyticsDataObject = new AnalyticsDataObject()
                                    {
                                        TaskAction = _Action,
                                        GraphNode = _ActionGraphNode,
                                        ProcessNode = _ProcessNode,
                                        Process = _ProcessGraphNode.LabelText
                                    };
                                    _ActionGraphNode.UserData = _AnalyticsDataObject;
                                }
                                else
                                {
                                    for (int i = 0; i < _ActionNodes.Count; i++)
                                    {
                                        if (_ActionNodes[i].TaskAction == _Action)
                                        {
                                            _AnalyticsDataObject = _ActionNodes[i];
                                            break;
                                        }
                                    }

                                    if (_AnalyticsDataObject != null)
                                    {
                                        Node _TempActionNode = _AnalyticsDataObject.GraphNode;

                                        ClearNodeEdges(_TempActionNode, _Edges);

                                    }
                                }
                                if (_AnalyticsDataObject != null)
                                {
                                    _WindowActionNodes.Add(_AnalyticsDataObject.GraphNode);

                                    //if (_ActionNode.Tag is SendKeysAction)
                                    //    _KeystrokeCount += (_ActionNode.Tag as SendKeysAction).Text.Length;
                                    //else if (_ActionNode.Tag is Chrome.ChromeActionKeyboard)
                                    //    _KeystrokeCount += (_ActionNode.Tag as Chrome.ChromeActionKeyboard)
                                    //        .CommandValueDisplay.Length;
                                    //else
                                    //    _Other++;

                                    ActionNodes.Add(_AnalyticsDataObject);
                                }
                            }
                        }

                        _WindowStatsNode.LabelText = string.Format("Activities:{0}{1}Time:{2}{1}Keystrokes:{3}, Mouse:{4}",
                            _WindowNode.Nodes.Count,
                            Environment.NewLine,
                            TimeSpan.FromMilliseconds(_WindowNode.MillisecsSpent).ToString(),
                            _KeystrokeCount.ToString(),
                            _Other.ToString());
                    }

                }

                int _TotalTime = 0;
                foreach (KeyValuePair<string, int> keyValuePair in _ProcessTimeUtil)
                    _TotalTime += keyValuePair.Value;
                foreach (KeyValuePair<string, int> keyValuePair in _ProcessTimeUtil)
                    chart1.Series[0].Points.AddXY(keyValuePair.Key,
                        ((double) keyValuePair.Value / (double) _TotalTime) * 100.0);
                CreateEdge(_PreviousNode, _EndNode, Color.Red);
                viewer.Graph = graph;

                ActionNodes.Sort(AnalyticsDataObjectTimeStampComparer.Instance);

                dataGridView1.DataSource = _ActionNodes;
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                _RetryCount++;
                if (_IsFirstRun)
                {
                    if (_RetryCount < 3)
                        goto BEGIN_REFRESH;
                    else
                        return;
                }
                else
                    MessageBox.Show(ex.ToString());
            }
        }

        private int _NodeCount = 0;

        string GenerateNodeID()
        {
            return (++_NodeCount).ToString();
        }

        Edge CreateEdge(Node Src, Node Dest, Color EdgeColor)
        {
            Edge edge = new Edge(Src, Dest,
                   Microsoft.Msagl.Drawing.ConnectionToGraph.Connected);
            edge.Attr.ArrowheadAtTarget = ArrowStyle.Normal;
            edge.Attr.Color = EdgeColor;
            return edge;
        }

        private class AnalyticsDataObjectTimeStampComparer : IComparer<AnalyticsDataObject>
        {
            public static AnalyticsDataObjectTimeStampComparer Instance = new AnalyticsDataObjectTimeStampComparer();

            private AnalyticsDataObjectTimeStampComparer()
            {

            }
            int IComparer<AnalyticsDataObject>.Compare(
                AnalyticsDataObject x,
                AnalyticsDataObject y)
            {
                return x.Time.CompareTo(y.Time);
            }
        }

        Node _CurrentSelection;
        private void viewer_Click(object sender, EventArgs e)
        {
            object sel = viewer.SelectedObject;
            AnalyticsDataObject _PropertyObj = null;
            if (sel is Node)
            {
                var n = sel as Node;
                if (n.UserData != null && n.UserData is AnalyticsDataObject)
                    _PropertyObj = (n.UserData as AnalyticsDataObject);
            }
            if (_PropertyObj != null)
                dataGridView1.Rows[_ActionNodes.IndexOf(_PropertyObj)].Selected = true;
        }

#region Process Discovery

        string _Original;

        Microsoft.Msagl.Drawing.Graph _ProcessDiscoveryGraph;

        private void cmdRunDiscovery_Click(object sender, EventArgs e)
        {
            // Predict.PredictionEngineClient.EnqueuePredictionAction
            _ProcessDiscoveryGraph = null;
            string _ProcessExeName = string.IsNullOrEmpty(txtProcessName.Text) ? string.Empty : txtProcessName.Text;
            _Original = cmdRunDiscovery.Text;
            txtAccuracyThreshold.Enabled = txtProcessName.Enabled = txtSteps.Enabled = cmdRunDiscovery.Enabled = false;
            tmrProcessDiscoveryCheck.Enabled = true;

            Task.Factory.StartNew(() => RunDiscovery(txtProcessName.Text, float.Parse(txtAccuracyThreshold.Text), Convert.ToInt32(txtSteps.Value)));
        }


        void RunDiscovery(string StartElement, float PredictionAccuracyMinThreshold, int MaxSteps)
        {
            //List<List<KeyValuePair<ZappyTaskAction, float>>> _DiscoveredProcesses = PredictionEngineClient.DiscoverProcesses(StartElement, PredictionAccuracyMinThreshold, MaxSteps);


            //Dictionary<string, Node> _ExistingNodes = new Dictionary<string, Node>();
            //Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");

            //for (int i = 0; i < _DiscoveredProcesses.Count; i++)
            //{


            //    List<KeyValuePair<ZappyTaskAction, float>> _DiscoveredProcess = _DiscoveredProcesses[i];

            //    string _LabelText = "Begin";
            //    if (_DiscoveredProcess.Count > 0)
            //    {
            //        if (!string.IsNullOrEmpty(_DiscoveredProcess[0].Key.TaskActivityIdentifier))
            //        {
            //            if (!string.IsNullOrEmpty(_DiscoveredProcess[0].Key.TaskActivityIdentifier))
            //            {
            //                //_LabelText = _DiscoveredProcess[0].Key.TaskActivityIdentifier;
            //                string[] UIObjSplit = _DiscoveredProcess[0].Key.TaskActivityIdentifier.Split('.');
            //                if (UIObjSplit.Length > 1)
            //                    _LabelText = UIObjSplit[1];
            //            }
            //        }
            //    }

            //    Color _ProcessColor = GetProcessColor(i);
            //    Node _StartNode = new Node(GenerateNodeID());
            //    _StartNode.LabelText = _LabelText;
            //    _StartNode.Attr.Shape = Shape.Diamond;
            //    _StartNode.Attr.FillColor = _ProcessColor;
            //    _StartNode.Label.FontColor = Color.White;
            //    graph.AddNode(_StartNode);

            //    Node _PrevActionGraphNode = _StartNode;


            //    float _ProcessWeight = 0;
            //    for (int j = 0; j < _DiscoveredProcess.Count; j++)
            //    {
            //        ZappyTaskAction uit = _DiscoveredProcess[j].Key;
            //        Node _ActionGraphNode = null;

            //        string _NodeKey = DisplayHelper.NodeTextHelper(uit, true) + " (" + DisplayHelper.GetActionElementName(uit) + ")";// Weight:";
            //        if (!_ExistingNodes.TryGetValue(_NodeKey, out _ActionGraphNode))
            //        {
            //            _ActionGraphNode = new Node(GenerateNodeID());
            //            _ExistingNodes[_NodeKey] = _ActionGraphNode;

            //            _ProcessWeight += _DiscoveredProcess[j].Value;

            //            _ActionGraphNode.LabelText = _NodeKey;// + _DiscoveredProcess[j].Value.ToString();

            //            _ActionGraphNode.UserData = uit;
            //            _ActionGraphNode.Attr.Shape = Shape.Box;
            //            _ActionGraphNode.Attr.FillColor = Color.LightBlue;

            //            graph.AddNode(_ActionGraphNode);
            //        }

            //        if (_PrevActionGraphNode != _ActionGraphNode)
            //            CreateEdge(_PrevActionGraphNode, _ActionGraphNode, _ProcessColor);
            //        _PrevActionGraphNode = _ActionGraphNode;
            //    }

            //    var _EndNode = new Node(GenerateNodeID());
            //    _EndNode.LabelText = "End";
            //    _EndNode.Attr.Shape = Shape.Diamond;
            //    _EndNode.Attr.FillColor = _ProcessColor;
            //    _EndNode.Label.FontColor = Color.White;

            //    //_StartNode.LabelText = "Begin" + _ProcessWeight.ToString();


            //    graph.AddNode(_EndNode);
            //    CreateEdge(_PrevActionGraphNode, _EndNode, _ProcessColor);
            //}
            //_ProcessDiscoveryGraph = graph;
        }

        private void tmrProcessDiscoveryCheck_Tick(object sender, EventArgs e)
        {
            if (_ProcessDiscoveryGraph != null)
            {
                tmrProcessDiscoveryCheck.Enabled = false;
                txtAccuracyThreshold.Enabled = txtProcessName.Enabled = txtSteps.Enabled = cmdRunDiscovery.Enabled = true;
                cmdRunDiscovery.Text = _Original;
                gViewer1.Graph = _ProcessDiscoveryGraph;
                _ProcessDiscoveryGraph = null;

            }
            else
            {
                if (cmdRunDiscovery.Text.Length <= 1)
                    cmdRunDiscovery.Text = _Original;
                else
                    cmdRunDiscovery.Text = cmdRunDiscovery.Text.Substring(1);
            }
        }

#endregion
       

        Color[] _ProcessColors;

        public Color GetProcessColor(int ProcessIndex)
        {
            if (_ProcessColors == null)
                _ProcessColors = new Color[] { Color.Green,
                    Color.Orange, Color.Teal,
                    Color.Gold, Color.MediumSpringGreen,
                    Color.Red, Color.Blue,
                    Color.DarkCyan, Color.DarkOrange,
                    Color.GreenYellow, Color.Tomato,
                    Color.Tan, Color.SteelBlue, Color.Yellow };

            return _ProcessColors[ProcessIndex % _ProcessColors.Length];
        }

#region Time Range Selection

        private void cmdFromMinus_Click(object sender, EventArgs e)
        {
            if (tbrFrom.Trackbar.Value - 15 < tbrFrom.Trackbar.Minimum)
                tbrFrom.Trackbar.Value = tbrFrom.Trackbar.Minimum;
            else
                tbrFrom.Trackbar.Value -= 15;
        }

        private void cmdFromPlus_Click(object sender, EventArgs e)
        {
            if (tbrFrom.Trackbar.Value + 15 > tbrFrom.Trackbar.Maximum)
                tbrFrom.Trackbar.Value = tbrFrom.Trackbar.Maximum;
            else
                tbrFrom.Trackbar.Value += 15;
        }

        private void cmdToMinus_Click(object sender, EventArgs e)
        {
            if (tbrTo.Trackbar.Value - 15 < tbrTo.Trackbar.Minimum)
                tbrTo.Trackbar.Value = tbrTo.Trackbar.Minimum;
            else
                tbrTo.Trackbar.Value -= 15;
        }

        private void cmdToPlus_Click(object sender, EventArgs e)
        {
            if (tbrTo.Trackbar.Value + 15 > tbrTo.Trackbar.Maximum)
                tbrTo.Trackbar.Value = tbrTo.Trackbar.Maximum;
            else
                tbrTo.Trackbar.Value += 15;
        }

        private void tbrFrom_ValueChanged(object sender, EventArgs e)
        {
            DateTime _FilterFrom = _MinDateTime.AddMinutes(tbrFrom.Trackbar.Value);
            if (_MaxDateTime.Date == _MinDateTime.Date)
                lblFrom.Text = _FilterFrom.ToString("\'From \'HH:mm");
            else
                lblFrom.Text = _FilterFrom.ToString("\'From \'dd-MM@HH:mm");
        }

        private void tbrTo_ValueChanged(object sender, EventArgs e)
        {
            DateTime _Filterto = _MinDateTime.AddMinutes(tbrTo.Trackbar.Value);
            if (_MaxDateTime.Date == _MinDateTime.Date)
                lblTo.Text = _Filterto.ToString("\'To \'HH:mm");
            else
                lblTo.Text = _Filterto.ToString("\'To \'dd-MM@HH:mm");
        }

#endregion
        
        private void txtFilterValue_ValueChanged(object sender, EventArgs e)
        {

        }

        private void frmProcessGraph_Load(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = (splitContainer1.Width / 10) * 3;
            splitContainer2.SplitterDistance = (splitContainer2.Height / 10) * 7;
            splitContainer1.Panel1Collapsed = true;
        }

        private void cmdFilter_Click(object sender, EventArgs e)
        {
            if (tbrFrom.Trackbar.Value < tbrTo.Trackbar.Value)
            {
                DateTime _FilteredMinDate = _MinDateTime.AddMinutes(tbrFrom.Trackbar.Value), _FilteredMaxDate = _MinDateTime.AddMinutes(tbrTo.Trackbar.Value);
                _FilteredActionNodes.RaiseListChangedEvents = false;
                _FilteredActionNodes.Clear();
                try
                {
                    DrawGraph(_FilteredMinDate, _FilteredMaxDate, _FilteredActionNodes);
                }
                catch(Exception ex)
                { MessageBox.Show(ex.ToString());}
                _FilteredActionNodes.RaiseListChangedEvents = true;
            }
        }

        private void cmdExport_Click(object sender, EventArgs e)
        {
            List<IZappyAction> _SelectedActivities = null;

            for (int i = 0; i < _ActionNodes.Count; i++)
            {
                if (_ActionNodes[i].Select)
                {
                    if (_SelectedActivities == null)
                        _SelectedActivities = new List<IZappyAction>();
                    _SelectedActivities.Add(_ActionNodes[i].TaskAction);
                }
            }

            if(_SelectedActivities!=null)
            {
                try
                {
                    ZappyTask _UitaskSelectedTasks = new ZappyTask(_SelectedActivities);
                    Stream myStream;
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Filter = "Zappy Files|*.zappy";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;
                    //TODO - initialize topnode
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        if ((myStream = saveFileDialog1.OpenFile()) != null)
                        {
                            // Code to write the stream goes here.
                            if (_UitaskSelectedTasks != null && _UitaskSelectedTasks.ExecuteActivities.Count > 0)
                                _UitaskSelectedTasks.Save(myStream);
                            else
                                MessageBox.Show("Error exporting task!!");
                            myStream.Close();
                        }
                    }
                }
                catch(Exception ex)
                { MessageBox.Show(ex.ToString());}
               

            }
        }

        private void cmdReset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _ActionNodes.Count; i++)
            {
                if (_ActionNodes[i].Select)
                {
                    _ActionNodes[i].Select = false;
                    _ActionNodes.ResetItem(i);
                }
            }
        }

        private void advanceEditor_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                AnalyticsDataObject _Selection = dataGridView1.SelectedRows[0].DataBoundItem as AnalyticsDataObject;

                propertyGrid1.SelectedObject = _Selection;

                if (_Selection.GraphNode.IsVisible)
                {
                    if(_CurrentSelection!=null)
                        _CurrentSelection.Attr.FillColor = Color.LightBlue;
                    _CurrentSelection = _Selection.GraphNode;
                    _CurrentSelection.Attr.FillColor = Color.DarkGoldenrod;
                    viewer.Refresh();
                }
            }
        }

        private void viewer_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                object sel = viewer.SelectedObject;
                if (sel is Node)
                {
                    Node _PrevActionGraphNode = sel as Node;
                    if (_PrevActionGraphNode.UserData != null && _PrevActionGraphNode.UserData is Tuple<bool, List<Node>>)
                    {
                        Tuple<bool, List<Node>> _Tuple = _PrevActionGraphNode.UserData as Tuple<bool, List<Node>>;
                        List<Node> _WindowActionNodes = _Tuple.Item2;
                        viewer.SuspendLayout();
                        Microsoft.Msagl.Drawing.Graph graph = viewer.Graph;
                        viewer.Graph = null;
                        if (!_Tuple.Item1)
                        {
                            for (int i = 0; i < _WindowActionNodes.Count; i++)
                            {
                                graph.AddNode(_WindowActionNodes[i]);
                                CreateEdge(_PrevActionGraphNode, _WindowActionNodes[i], Color.RoyalBlue);
                                _PrevActionGraphNode = _WindowActionNodes[i];
                            }
                        }
                        else
                        {
                            for (int i = _WindowActionNodes.Count - 1; i >= 0; i--)
                                graph.RemoveNode(_WindowActionNodes[i]);
                        }

                       (sel as Node).UserData = new Tuple<bool, List<Node>>(!_Tuple.Item1, _Tuple.Item2);

                        bool _AllExpanded = true;
                        foreach (var item in graph.Nodes)
                        {
                            if (item.UserData != null && item.UserData is Tuple<bool, List<Node>>)
                                _AllExpanded &= (item.UserData as Tuple<bool, List<Node>>).Item1;
                        }


                        if (_AllExpanded)
                        {
                            _ActionNodes.Sort(AnalyticsDataObjectTimeStampComparer.Instance);
                            _PrevActionGraphNode = _StartNode;
                            for (int i = 0; i < _ActionNodes.Count; i++)
                            {
                                CreateEdge(_PrevActionGraphNode, _ActionNodes[i].GraphNode, Color.Red);
                                _PrevActionGraphNode = _ActionNodes[i].GraphNode;
                            }
                            CreateEdge(_PrevActionGraphNode, _EndNode, Color.Red);
                        }
                        else
                        {
                            List<Edge> _RemovalEdges = new List<Edge>();
                            foreach (var item in graph.Edges)
                            {
                                if (item.Attr.Color == Color.Red && item.SourceNode.UserData is ZappyTaskAction)
                                {
                                    _RemovalEdges.Add(item);
                                }
                            }

                            foreach (var item in _RemovalEdges)
                            {
                                graph.RemoveEdge(item);
                            }
                            _RemovalEdges.Clear();
                        }
                        viewer.Graph = graph;
                        viewer.ResumeLayout(true);
                    }
                }
            }
            catch (Exception ex)
            {
                var temp = ex;
                throw;
            }

        }

    }
}
