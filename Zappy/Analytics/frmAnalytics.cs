using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Msagl.Drawing;

namespace Zappy.Analytics
{
    internal partial class frmAnalytics : Form
    {
        public frmAnalytics()
        {
            InitializeComponent();

        }

        public void initData(IEnumerable<Node> processnodes)
        {
            BEGIN_REFRESH:

            chart1.Series[0].Points.Clear();
            try
            {
                Dictionary<string, int> _ProcessTimeUtil = new Dictionary<string, int>();
                //foreach (Node _ProcessNode in processnodes)
                //{

                //    int _MillisecsSpent;
                //    string _ProcessName = Path.GetFileName(_ProcessNode.Text);
                //    _ProcessTimeUtil.TryGetValue(_ProcessName, out _MillisecsSpent);

                //    foreach (Node _WindowNode in _ProcessNode.Nodes)
                //        _MillisecsSpent += _WindowNode.MillisecsSpent;

                //    _ProcessTimeUtil[_ProcessName] = _MillisecsSpent;

                //}
                foreach (KeyValuePair<string, int> keyValuePair in _ProcessTimeUtil)
                    chart1.Series[0].Points.AddXY(keyValuePair.Key, keyValuePair.Value / 60000);
            }
            catch
            {
                goto BEGIN_REFRESH;
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            //
        }

        private void chart1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HitTestResult htr = chart1.HitTest(e.X, e.Y, ChartElementType.DataPoint);
            if (htr.PointIndex >= 0)
            {
                DataPoint dp =chart1.Series[0].Points[htr.PointIndex];
            }
        }
    }
}
