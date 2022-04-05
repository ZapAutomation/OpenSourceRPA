using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Zappy.ZappyActions.Picture
{
    public partial class ClickLocationForm : Form
    {
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);
        public string SelectedItem { get; set; }
        Point lpPoint = new Point();
        bool flag = false;
        public ClickLocationForm(object selectedItem)
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            SelectedItem = selectedItem as string;

        }
        private void okButton_Click(object sender, EventArgs e)
        {
            if (flag == true)
                SelectedItem = lpPoint.ToString();
            else
                SelectedItem = (string)comboBox1.SelectedItem;

        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            //this.Hide();
            ////Point lpPoint = new Point();
            //this.MouseDown += new MouseEventHandler(MouseDownDrawings);
            //this.MouseUp += new MouseEventHandler(MouseUpDrawings);
            //this.Show();
            //lpPoint.X = MousePosition.X;
            //lpPoint.Y = MousePosition.Y;
            //GetCursorPos(out lpPoint);

            //flag = true;
            //MessageBox.Show(SelectedItem);
        }
        void MouseDownDrawings(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //lpPoint = e.Location;
            //flag = true;
            //SelectedItem = lpPoint.ToString();
        }
        //void MouseUpDrawings(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    ClickLocationForm.ActiveForm.Show();
        //}
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Point lpPoint = new Point();
            //GetCursorPos(out lpPoint);
            //SelectedItem = lpPoint.ToString();
        }
    }
}
