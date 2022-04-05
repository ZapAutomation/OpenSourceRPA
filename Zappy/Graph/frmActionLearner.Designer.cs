
namespace Zappy.Graph
{
    partial class frmActionLearner
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmActionLearner));
            this.lblLearning = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblActionType = new System.Windows.Forms.Label();
            this.lblApplicationName = new System.Windows.Forms.Label();
            this.txtActionValue = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtNotification = new System.Windows.Forms.TextBox();
            this.hideButton = new System.Windows.Forms.Button();
            this.stopRecording = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblLearning
            // 
            this.lblLearning.AutoSize = true;
            this.lblLearning.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLearning.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblLearning.Image = ((System.Drawing.Image)(resources.GetObject("lblLearning.Image")));
            this.lblLearning.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLearning.Location = new System.Drawing.Point(11, 8);
            this.lblLearning.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLearning.Name = "lblLearning";
            this.lblLearning.Size = new System.Drawing.Size(97, 15);
            this.lblLearning.TabIndex = 0;
            this.lblLearning.Text = "     Learning...";
            this.lblLearning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 57);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "App Name :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 34);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 14);
            this.label3.TabIndex = 1;
            this.label3.Text = "Action :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 80);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 14);
            this.label4.TabIndex = 1;
            this.label4.Text = "Details :";
            // 
            // lblActionType
            // 
            this.lblActionType.AutoSize = true;
            this.lblActionType.Location = new System.Drawing.Point(70, 34);
            this.lblActionType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblActionType.Name = "lblActionType";
            this.lblActionType.Size = new System.Drawing.Size(56, 14);
            this.lblActionType.TabIndex = 1;
            this.lblActionType.Text = "<Action>";
            // 
            // lblApplicationName
            // 
            this.lblApplicationName.AutoSize = true;
            this.lblApplicationName.Location = new System.Drawing.Point(70, 57);
            this.lblApplicationName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblApplicationName.Name = "lblApplicationName";
            this.lblApplicationName.Size = new System.Drawing.Size(42, 14);
            this.lblApplicationName.TabIndex = 1;
            this.lblApplicationName.Text = "<App>";
            // 
            // txtActionValue
            // 
            this.txtActionValue.BackColor = System.Drawing.Color.White;
            this.txtActionValue.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtActionValue.Location = new System.Drawing.Point(70, 82);
            this.txtActionValue.Name = "txtActionValue";
            this.txtActionValue.ReadOnly = true;
            this.txtActionValue.Size = new System.Drawing.Size(251, 13);
            this.txtActionValue.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Interval = 400;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtNotification
            // 
            this.txtNotification.BackColor = System.Drawing.Color.White;
            this.txtNotification.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNotification.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNotification.Location = new System.Drawing.Point(22, 34);
            this.txtNotification.Multiline = true;
            this.txtNotification.Name = "txtNotification";
            this.txtNotification.ReadOnly = true;
            this.txtNotification.Size = new System.Drawing.Size(299, 59);
            this.txtNotification.TabIndex = 3;
            // 
            // hideButton
            // 
            this.hideButton.BackColor = System.Drawing.Color.Transparent;
            this.hideButton.ForeColor = System.Drawing.Color.Transparent;
            this.hideButton.Image = ((System.Drawing.Image)(resources.GetObject("hideButton.Image")));
            this.hideButton.Location = new System.Drawing.Point(306, 4);
            this.hideButton.Margin = new System.Windows.Forms.Padding(0);
            this.hideButton.Name = "hideButton";
            this.hideButton.Size = new System.Drawing.Size(26, 23);
            this.hideButton.TabIndex = 4;
            this.hideButton.UseVisualStyleBackColor = false;
            this.hideButton.Click += new System.EventHandler(this.hideButton_Click);
            // 
            // stopRecording
            // 
            this.stopRecording.BackColor = System.Drawing.Color.Transparent;
            this.stopRecording.ForeColor = System.Drawing.Color.Transparent;
            this.stopRecording.Image = ((System.Drawing.Image)(resources.GetObject("stopRecording.Image")));
            this.stopRecording.Location = new System.Drawing.Point(264, 4);
            this.stopRecording.Margin = new System.Windows.Forms.Padding(0);
            this.stopRecording.Name = "stopRecording";
            this.stopRecording.Size = new System.Drawing.Size(42, 23);
            this.stopRecording.TabIndex = 5;
            this.stopRecording.Text = "Stop";
            this.stopRecording.UseVisualStyleBackColor = false;
            this.stopRecording.Click += new System.EventHandler(this.stopRecording_Click);
            // 
            // frmActionLearner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(333, 105);
            this.Controls.Add(this.stopRecording);
            this.Controls.Add(this.hideButton);
            this.Controls.Add(this.txtNotification);
            this.Controls.Add(this.txtActionValue);
            this.Controls.Add(this.lblApplicationName);
            this.Controls.Add(this.lblActionType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblLearning);
            this.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "frmActionLearner";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmActionLearner";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLearning;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblActionType;
        private System.Windows.Forms.Label lblApplicationName;
        private System.Windows.Forms.TextBox txtActionValue;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txtNotification;
        private System.Windows.Forms.Button hideButton;
        private System.Windows.Forms.Button stopRecording;
    }
}