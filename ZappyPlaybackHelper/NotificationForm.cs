using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZappyPlaybackHelper
{
    public partial class NotificationForm : Form
    {
        public string NotificationText;
        public NotificationForm(string text)
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ShowInTaskbar = false;
            NotificationText = text;            
            popupNotifier1.Image = Properties.Resources.lightingIcon.ToBitmap();
            this.Load += new EventHandler(Form1_Load);

        }
        void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(0, 0);
            popupNotifier1.TitleText = "Zappy Task";
            popupNotifier1.IsRightToLeft = false;
            ShowPopup();
        }

        public void ShowPopup()
        {
            if (IsHandleCreated)
            {
                if (InvokeRequired)
                {
                    this.BeginInvoke(new Action( this.ShowPopup));
                }
                else
                {
                    popupNotifier1.ContentText = NotificationText;
                    popupNotifier1.Popup();
                }
            }
        }
    }
}
