using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Zappy.ZappyActions.ProgrammingScripts
{
    [ComVisible(true)]
    public class VBHiddenForm : Form
    {
        public VBHiddenForm()
        {
        }
        MSScriptControl.ScriptControlClass script;

        public object Evaluate(string Script)
        {
            script = new MSScriptControl.ScriptControlClass();
            script.Language = "VBScript";
            script.AddObject("me", this, true);

            return script.Eval(Script);

        }

        public void Execute(string Script)
        {
            script = new MSScriptControl.ScriptControlClass();
            script.Language = "VBScript";
            script.AddObject("me", this, true);

            script.ExecuteStatement(Script);

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // HiddenForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "HiddenForm";
            this.Load += new System.EventHandler(this.HiddenForm_Load);
            this.ResumeLayout(false);

        }

        private void HiddenForm_Load(object sender, EventArgs e)
        {

        }
    }
}