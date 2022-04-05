namespace Zappy.Decode.Hooks
{
    partial class frmHotKeyManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHotKeyManager));
            this.comboBoxLearnedActions = new System.Windows.Forms.ComboBox();
            this.comboBoxExecuteFirstTask = new System.Windows.Forms.ComboBox();
            this.comboBoxCancelExecution = new System.Windows.Forms.ComboBox();
            this.comboBoxExportLearnedActions = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cancel = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxLearnedActions
            // 
            resources.ApplyResources(this.comboBoxLearnedActions, "comboBoxLearnedActions");
            this.comboBoxLearnedActions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLearnedActions.FormattingEnabled = true;
            this.comboBoxLearnedActions.Name = "comboBoxLearnedActions";
            // 
            // comboBoxExecuteFirstTask
            // 
            resources.ApplyResources(this.comboBoxExecuteFirstTask, "comboBoxExecuteFirstTask");
            this.comboBoxExecuteFirstTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExecuteFirstTask.FormattingEnabled = true;
            this.comboBoxExecuteFirstTask.Name = "comboBoxExecuteFirstTask";
            // 
            // comboBoxCancelExecution
            // 
            resources.ApplyResources(this.comboBoxCancelExecution, "comboBoxCancelExecution");
            this.comboBoxCancelExecution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCancelExecution.FormattingEnabled = true;
            this.comboBoxCancelExecution.Name = "comboBoxCancelExecution";
            // 
            // comboBoxExportLearnedActions
            // 
            resources.ApplyResources(this.comboBoxExportLearnedActions, "comboBoxExportLearnedActions");
            this.comboBoxExportLearnedActions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExportLearnedActions.FormattingEnabled = true;
            this.comboBoxExportLearnedActions.Name = "comboBoxExportLearnedActions";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // cancel
            // 
            resources.ApplyResources(this.cancel, "cancel");
            this.cancel.Name = "cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // ok
            // 
            resources.ApplyResources(this.ok, "ok");
            this.ok.Name = "ok";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Cursor = System.Windows.Forms.Cursors.Default;
            this.label5.Name = "label5";
            // 
            // frmHotKeyManager
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxExportLearnedActions);
            this.Controls.Add(this.comboBoxCancelExecution);
            this.Controls.Add(this.comboBoxExecuteFirstTask);
            this.Controls.Add(this.comboBoxLearnedActions);
            this.Name = "frmHotKeyManager";
            this.Load += new System.EventHandler(this.HotKeyManager_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxLearnedActions;
        private System.Windows.Forms.ComboBox comboBoxExecuteFirstTask;
        private System.Windows.Forms.ComboBox comboBoxCancelExecution;
        private System.Windows.Forms.ComboBox comboBoxExportLearnedActions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Label label5;
    }
}