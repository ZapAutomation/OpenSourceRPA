using System;
using System.Drawing;
using System.Windows.Forms;
using Zappy.Decode.Helper;
using Zappy.Invoker;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.EditorPage;

namespace Zappy.Graph
{
    public partial class frmActionLearner : Form
    {
        static frmActionLearner _LearningStepInstance, _NotificationInstance;

        public static frmActionLearner LearningStepInstance
        {
            get
            {
                if (_LearningStepInstance == null)
                    _LearningStepInstance = new frmActionLearner();
                return _LearningStepInstance;
            }
        }

        public static frmActionLearner NotificationInstance
        {
            get
            {
                if (_NotificationInstance == null)
                    _NotificationInstance = new frmActionLearner();
                return _NotificationInstance;
            }
        }

        public frmActionLearner()
        {
            InitializeComponent();
            //#if DEBUG
            //            txtActionValue.BorderStyle = BorderStyle.FixedSingle;
            //#else
            txtActionValue.BorderStyle = BorderStyle.None;
            hideButton.FlatAppearance.BorderSize = 0;
            hideButton.TabStop = false;
            hideButton.FlatStyle = FlatStyle.Flat;
            hideButton.FlatAppearance.BorderSize = 0;
            hideButton.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255); //transparent
            if (!DesignMode)
            {
                int x = Screen.PrimaryScreen.WorkingArea.Width - this.Width - 10;
                int y = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 10;
                Location = new Point(x, y);
            }
        }

        IZappyAction _LastAction, _LastRecievedAction;
        int _ActionCount = 0;

        public void UpdateAction(IZappyAction Action)
        {
            _LastRecievedAction = Action;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (txtNotification.Visible)
            {
                if (Visible)
                    this.Hide();
                return;
            }

            if (lblLearning.ForeColor == Color.DodgerBlue)
                lblLearning.ForeColor = Color.ForestGreen;
            else
                lblLearning.ForeColor = Color.DodgerBlue;

            bool _ActionChanged = !ReferenceEquals(_LastRecievedAction, _LastAction);
            _LastAction = _LastRecievedAction;
            if (_ActionChanged)
                _ActionCount++;
            RefreshActionDetails(_ActionChanged);            
        }

        void RefreshActionDetails(bool ActionChanged = true)
        {

            if (_LastAction != null)
            {
                if (ActionChanged)
                {
                    lblApplicationName.Text = _LastAction.ExeName + " @ " + DisplayHelper.GetActionElementName(_LastAction); ;
                    lblActionType.Text = string.Format("Action # :{0} , {1}", _ActionCount.ToString(), _LastAction.GetType().Name);
                }
                txtActionValue.Text = DisplayHelper.ActionValueHelper(_LastAction);
            }
            else
            {
                _ActionCount = 0;
                lblApplicationName.Text = lblActionType.Text = txtActionValue.Text = string.Empty;
            }
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            //Can change to something like teamviewer
            this.Hide();
        }

        private void stopRecording_Click(object sender, EventArgs e)
        {
            string file = LearnedActions.CreateLearnedActions();
            PageFormTabbed frm = new PageFormTabbed(file);
            frm.Show();
        }

        public void ShowInactiveTopmost(string NotificationText = "")
        {
            this.stopRecording.Visible = ClientUI._TaskRecording;

            bool LearningActions = string.IsNullOrEmpty(NotificationText);

            txtNotification.Visible = !LearningActions;

            label2.Visible = label3.Visible = label4.Visible =
                lblActionType.Visible = lblApplicationName.Visible =
                txtActionValue.Visible = LearningActions;

            if (LearningActions)
            {
                lblLearning.Text = "     Learning...";
                _LastAction = null;
                this.timer1.Interval = 400;
                RefreshActionDetails();
            }
            else
            {
                this.timer1.Interval = 4000;
                lblLearning.Text = "     Zappy";
                txtNotification.Text = NotificationText;
            }


            NativeMethods.ShowWindow(Handle, NativeMethods.WindowShowStyle.ShowNormalNoActivate);
            NativeMethods.SetWindowPos(Handle,
                new IntPtr(NativeMethods.HWND_TOPMOST),
           Left, Top, Width, Height,
           NativeMethods.SWP_NOACTIVATE);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            timer1.Enabled = this.Visible;
        }
    }
}
