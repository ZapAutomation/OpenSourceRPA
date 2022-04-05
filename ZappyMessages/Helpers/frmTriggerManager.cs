using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ZappyMessages;

namespace Zappy.Predict
{
    public partial class frmTriggerManager : Form
    {
        private static Dictionary<string, string> runSelectedZappyActionTrigger;
        public frmTriggerManager()
        {
            InitializeComponent();
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            try
            {
                if (processDataBox.Lines.Length > 0)
                {
                    //Not very efficient but works
                    foreach (var fileName in processDataBox.Lines)
                    {
                        runSelectedZappyActionTrigger = new Dictionary<string, string>();
                        runSelectedZappyActionTrigger[Path.GetDirectoryName(fileName)] = fileName;
                    }
                    File.WriteAllText(ZappyMessagingConstants.AutoExecFileLoc,
                        ZappySerializer.SerializeObject(runSelectedZappyActionTrigger));
                }
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

        private void frmMLLearningProcesses_Load(object sender, EventArgs e)
        {
            if (File.Exists(ZappyMessagingConstants.AutoExecFileLoc))
            {
                try
                {
                    runSelectedZappyActionTrigger = ZappySerializer.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ZappyMessagingConstants.AutoExecFileLoc));
                }
                catch { }
            }

            if (runSelectedZappyActionTrigger == null)
                runSelectedZappyActionTrigger = new Dictionary<string, string>();
            processDataBox.Lines = runSelectedZappyActionTrigger.Values.ToArray();
        }
    }
}
