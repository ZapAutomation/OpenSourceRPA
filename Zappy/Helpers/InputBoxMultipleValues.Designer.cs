using System;
using System.Drawing;
using System.Windows.Forms;

namespace Zappy.Helpers
{
    partial class InputBoxMultipleValues
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

        Label lblTitle;
        TextBox txtInputValue1;
        Button buttonOk;
        Button buttonCancel;
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputBoxMultipleValues));
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtInputValue1 = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblFieldName1 = new System.Windows.Forms.Label();
            this.lblFieldName2 = new System.Windows.Forms.Label();
            this.txtInputValue2 = new System.Windows.Forms.TextBox();
            this.lblFieldName3 = new System.Windows.Forms.Label();
            this.txtInputValue3 = new System.Windows.Forms.TextBox();
            this.lblFieldName4 = new System.Windows.Forms.Label();
            this.txtInputValue4 = new System.Windows.Forms.TextBox();
            this.lblFieldName5 = new System.Windows.Forms.Label();
            this.txtInputValue5 = new System.Windows.Forms.TextBox();
            this.lblFieldName6 = new System.Windows.Forms.Label();
            this.txtInputValue6 = new System.Windows.Forms.TextBox();
            this.lblFieldName7 = new System.Windows.Forms.Label();
            this.txtInputValue7 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(76, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(114, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "<Title / Prompt >";
            // 
            // txtInputValue1
            // 
            this.txtInputValue1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputValue1.Location = new System.Drawing.Point(73, 65);
            this.txtInputValue1.Name = "txtInputValue1";
            this.txtInputValue1.Size = new System.Drawing.Size(747, 20);
            this.txtInputValue1.TabIndex = 1;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOk.Location = new System.Drawing.Point(390, 439);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(121, 32);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(542, 439);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(122, 32);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(9, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(53, 48);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // lblFieldName1
            // 
            this.lblFieldName1.AutoSize = true;
            this.lblFieldName1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFieldName1.Location = new System.Drawing.Point(76, 43);
            this.lblFieldName1.Name = "lblFieldName1";
            this.lblFieldName1.Size = new System.Drawing.Size(90, 19);
            this.lblFieldName1.TabIndex = 0;
            this.lblFieldName1.Text = "FieldName";
            // 
            // lblFieldName2
            // 
            this.lblFieldName2.AutoSize = true;
            this.lblFieldName2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFieldName2.Location = new System.Drawing.Point(76, 96);
            this.lblFieldName2.Name = "lblFieldName2";
            this.lblFieldName2.Size = new System.Drawing.Size(90, 19);
            this.lblFieldName2.TabIndex = 5;
            this.lblFieldName2.Text = "FieldName";
            // 
            // txtInputValue2
            // 
            this.txtInputValue2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputValue2.Location = new System.Drawing.Point(73, 127);
            this.txtInputValue2.Name = "txtInputValue2";
            this.txtInputValue2.Size = new System.Drawing.Size(747, 20);
            this.txtInputValue2.TabIndex = 6;
            // 
            // lblFieldName3
            // 
            this.lblFieldName3.AutoSize = true;
            this.lblFieldName3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFieldName3.Location = new System.Drawing.Point(72, 166);
            this.lblFieldName3.Name = "lblFieldName3";
            this.lblFieldName3.Size = new System.Drawing.Size(90, 19);
            this.lblFieldName3.TabIndex = 7;
            this.lblFieldName3.Text = "FieldName";
            // 
            // txtInputValue3
            // 
            this.txtInputValue3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputValue3.Location = new System.Drawing.Point(73, 188);
            this.txtInputValue3.Name = "txtInputValue3";
            this.txtInputValue3.Size = new System.Drawing.Size(747, 20);
            this.txtInputValue3.TabIndex = 8;
            // 
            // lblFieldName4
            // 
            this.lblFieldName4.AutoSize = true;
            this.lblFieldName4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFieldName4.Location = new System.Drawing.Point(72, 211);
            this.lblFieldName4.Name = "lblFieldName4";
            this.lblFieldName4.Size = new System.Drawing.Size(90, 19);
            this.lblFieldName4.TabIndex = 9;
            this.lblFieldName4.Text = "FieldName";
            // 
            // txtInputValue4
            // 
            this.txtInputValue4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputValue4.Location = new System.Drawing.Point(73, 233);
            this.txtInputValue4.Name = "txtInputValue4";
            this.txtInputValue4.Size = new System.Drawing.Size(747, 20);
            this.txtInputValue4.TabIndex = 10;
            // 
            // lblFieldName5
            // 
            this.lblFieldName5.AutoSize = true;
            this.lblFieldName5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFieldName5.Location = new System.Drawing.Point(75, 267);
            this.lblFieldName5.Name = "lblFieldName5";
            this.lblFieldName5.Size = new System.Drawing.Size(90, 19);
            this.lblFieldName5.TabIndex = 11;
            this.lblFieldName5.Text = "FieldName";
            // 
            // txtInputValue5
            // 
            this.txtInputValue5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputValue5.Location = new System.Drawing.Point(73, 289);
            this.txtInputValue5.Name = "txtInputValue5";
            this.txtInputValue5.Size = new System.Drawing.Size(747, 20);
            this.txtInputValue5.TabIndex = 12;
            // 
            // lblFieldName6
            // 
            this.lblFieldName6.AutoSize = true;
            this.lblFieldName6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFieldName6.Location = new System.Drawing.Point(75, 327);
            this.lblFieldName6.Name = "lblFieldName6";
            this.lblFieldName6.Size = new System.Drawing.Size(90, 19);
            this.lblFieldName6.TabIndex = 13;
            this.lblFieldName6.Text = "FieldName";
            // 
            // txtInputValue6
            // 
            this.txtInputValue6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputValue6.Location = new System.Drawing.Point(73, 349);
            this.txtInputValue6.Name = "txtInputValue6";
            this.txtInputValue6.Size = new System.Drawing.Size(747, 20);
            this.txtInputValue6.TabIndex = 14;
            // 
            // lblFieldName7
            // 
            this.lblFieldName7.AutoSize = true;
            this.lblFieldName7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFieldName7.Location = new System.Drawing.Point(76, 381);
            this.lblFieldName7.Name = "lblFieldName7";
            this.lblFieldName7.Size = new System.Drawing.Size(90, 19);
            this.lblFieldName7.TabIndex = 15;
            this.lblFieldName7.Text = "FieldName";
            // 
            // txtInputValue7
            // 
            this.txtInputValue7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputValue7.Location = new System.Drawing.Point(73, 403);
            this.txtInputValue7.Name = "txtInputValue7";
            this.txtInputValue7.Size = new System.Drawing.Size(747, 20);
            this.txtInputValue7.TabIndex = 16;
            // 
            // InputBoxMultipleValues
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(835, 483);
            this.Controls.Add(this.lblFieldName7);
            this.Controls.Add(this.txtInputValue7);
            this.Controls.Add(this.lblFieldName6);
            this.Controls.Add(this.txtInputValue6);
            this.Controls.Add(this.lblFieldName5);
            this.Controls.Add(this.txtInputValue5);
            this.Controls.Add(this.lblFieldName4);
            this.Controls.Add(this.txtInputValue4);
            this.Controls.Add(this.lblFieldName3);
            this.Controls.Add(this.txtInputValue3);
            this.Controls.Add(this.lblFieldName2);
            this.Controls.Add(this.txtInputValue2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblFieldName1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtInputValue1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBoxMultipleValues";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InputBox";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private PictureBox pictureBox1;
        private Label lblFieldName1;
        private Label lblFieldName2;
        private TextBox txtInputValue2;
        private Label lblFieldName3;
        private TextBox txtInputValue3;
        private Label lblFieldName4;
        private TextBox txtInputValue4;
        private Label lblFieldName5;
        private TextBox txtInputValue5;
        private Label lblFieldName6;
        private TextBox txtInputValue6;
        private Label lblFieldName7;
        private TextBox txtInputValue7;
    }
}