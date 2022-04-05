namespace Zappy.ZappyActions.Picture
{
    partial class BitmapDetectorForm
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
            this.bitmap1TextBox = new System.Windows.Forms.TextBox();
            this.bitmap1Button = new System.Windows.Forms.Button();
            this.bitmap2Button = new System.Windows.Forms.Button();
            this.bitmap2TextBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.bitmapOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.bitmap1Label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.xTextBox = new System.Windows.Forms.TextBox();
            this.yTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.heightTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.coordinatesGroupBox = new System.Windows.Forms.GroupBox();
            this.elapsedTimeLabel = new System.Windows.Forms.Label();
            this.toleranceTrackBar = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.toleranceValueLabel = new System.Windows.Forms.Label();
            this.autoToleranceCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.toleranceTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // bitmap1TextBox
            // 
            this.bitmap1TextBox.Location = new System.Drawing.Point(80, 13);
            this.bitmap1TextBox.Name = "bitmap1TextBox";
            this.bitmap1TextBox.Size = new System.Drawing.Size(216, 20);
            this.bitmap1TextBox.TabIndex = 0;
            // 
            // bitmap1Button
            // 
            this.bitmap1Button.Location = new System.Drawing.Point(303, 13);
            this.bitmap1Button.Name = "bitmap1Button";
            this.bitmap1Button.Size = new System.Drawing.Size(60, 20);
            this.bitmap1Button.TabIndex = 1;
            this.bitmap1Button.Text = "Browse";
            this.bitmap1Button.UseVisualStyleBackColor = true;
            this.bitmap1Button.Click += new System.EventHandler(this.bitmap1Button_Click);
            // 
            // bitmap2Button
            // 
            this.bitmap2Button.Location = new System.Drawing.Point(303, 39);
            this.bitmap2Button.Name = "bitmap2Button";
            this.bitmap2Button.Size = new System.Drawing.Size(60, 20);
            this.bitmap2Button.TabIndex = 3;
            this.bitmap2Button.Text = "Browse";
            this.bitmap2Button.UseVisualStyleBackColor = true;
            this.bitmap2Button.Click += new System.EventHandler(this.bitmap2Button_Click);
            // 
            // bitmap2TextBox
            // 
            this.bitmap2TextBox.Location = new System.Drawing.Point(80, 39);
            this.bitmap2TextBox.Name = "bitmap2TextBox";
            this.bitmap2TextBox.Size = new System.Drawing.Size(216, 20);
            this.bitmap2TextBox.TabIndex = 2;
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(147, 101);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 23);
            this.searchButton.TabIndex = 5;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // bitmapOpenFileDialog
            // 
            this.bitmapOpenFileDialog.Title = "Please choose a Bitmap";
            // 
            // bitmap1Label
            // 
            this.bitmap1Label.AutoSize = true;
            this.bitmap1Label.Location = new System.Drawing.Point(23, 16);
            this.bitmap1Label.Name = "bitmap1Label";
            this.bitmap1Label.Size = new System.Drawing.Size(51, 13);
            this.bitmap1Label.TabIndex = 7;
            this.bitmap1Label.Text = "Bitmap 1:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Bitmap 2:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 157);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "X:";
            // 
            // xTextBox
            // 
            this.xTextBox.Location = new System.Drawing.Point(56, 153);
            this.xTextBox.Name = "xTextBox";
            this.xTextBox.ReadOnly = true;
            this.xTextBox.Size = new System.Drawing.Size(37, 20);
            this.xTextBox.TabIndex = 6;
            // 
            // yTextBox
            // 
            this.yTextBox.Location = new System.Drawing.Point(125, 153);
            this.yTextBox.Name = "yTextBox";
            this.yTextBox.ReadOnly = true;
            this.yTextBox.Size = new System.Drawing.Size(37, 20);
            this.yTextBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(102, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Y:";
            // 
            // widthTextBox
            // 
            this.widthTextBox.Location = new System.Drawing.Point(215, 153);
            this.widthTextBox.Name = "widthTextBox";
            this.widthTextBox.ReadOnly = true;
            this.widthTextBox.Size = new System.Drawing.Size(37, 20);
            this.widthTextBox.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(171, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Width:";
            // 
            // heightTextBox
            // 
            this.heightTextBox.Location = new System.Drawing.Point(303, 153);
            this.heightTextBox.Name = "heightTextBox";
            this.heightTextBox.ReadOnly = true;
            this.heightTextBox.Size = new System.Drawing.Size(37, 20);
            this.heightTextBox.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(259, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Height:";
            // 
            // coordinatesGroupBox
            // 
            this.coordinatesGroupBox.Location = new System.Drawing.Point(26, 130);
            this.coordinatesGroupBox.Name = "coordinatesGroupBox";
            this.coordinatesGroupBox.Size = new System.Drawing.Size(337, 58);
            this.coordinatesGroupBox.TabIndex = 17;
            this.coordinatesGroupBox.TabStop = false;
            this.coordinatesGroupBox.Text = "Rectangle";
            // 
            // elapsedTimeLabel
            // 
            this.elapsedTimeLabel.AutoSize = true;
            this.elapsedTimeLabel.Location = new System.Drawing.Point(239, 106);
            this.elapsedTimeLabel.Name = "elapsedTimeLabel";
            this.elapsedTimeLabel.Size = new System.Drawing.Size(0, 13);
            this.elapsedTimeLabel.TabIndex = 18;
            // 
            // toleranceTrackBar
            // 
            this.toleranceTrackBar.Location = new System.Drawing.Point(80, 66);
            this.toleranceTrackBar.Maximum = 20;
            this.toleranceTrackBar.Name = "toleranceTrackBar";
            this.toleranceTrackBar.Size = new System.Drawing.Size(140, 42);
            this.toleranceTrackBar.TabIndex = 4;
            this.toleranceTrackBar.TickFrequency = 5;
            this.toleranceTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.toleranceTrackBar.Value = 20;
            this.toleranceTrackBar.ValueChanged += new System.EventHandler(this.toleranceTrackBar_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Tolerance:";
            // 
            // toleranceValueLabel
            // 
            this.toleranceValueLabel.AutoSize = true;
            this.toleranceValueLabel.Location = new System.Drawing.Point(222, 75);
            this.toleranceValueLabel.Name = "toleranceValueLabel";
            this.toleranceValueLabel.Size = new System.Drawing.Size(30, 13);
            this.toleranceValueLabel.TabIndex = 21;
            this.toleranceValueLabel.Text = "20 %";
            // 
            // autoToleranceCheckBox
            // 
            this.autoToleranceCheckBox.AutoSize = true;
            this.autoToleranceCheckBox.Location = new System.Drawing.Point(262, 74);
            this.autoToleranceCheckBox.Name = "autoToleranceCheckBox";
            this.autoToleranceCheckBox.Size = new System.Drawing.Size(48, 17);
            this.autoToleranceCheckBox.TabIndex = 22;
            this.autoToleranceCheckBox.Text = "Auto";
            this.autoToleranceCheckBox.UseVisualStyleBackColor = true;
            this.autoToleranceCheckBox.CheckedChanged += new System.EventHandler(this.autoToleranceCheckBox_CheckedChanged);
            // 
            // BitmapDetectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 201);
            this.Controls.Add(this.autoToleranceCheckBox);
            this.Controls.Add(this.toleranceValueLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.elapsedTimeLabel);
            this.Controls.Add(this.heightTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.widthTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.yTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.xTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bitmap1Label);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.bitmap2Button);
            this.Controls.Add(this.bitmap2TextBox);
            this.Controls.Add(this.bitmap1Button);
            this.Controls.Add(this.bitmap1TextBox);
            this.Controls.Add(this.coordinatesGroupBox);
            this.Controls.Add(this.toleranceTrackBar);
            this.Name = "BitmapDetectorForm";
            this.Text = "BitmapDetector v0.1";
            ((System.ComponentModel.ISupportInitialize)(this.toleranceTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox bitmap1TextBox;
        private System.Windows.Forms.Button bitmap1Button;
        private System.Windows.Forms.Button bitmap2Button;
        private System.Windows.Forms.TextBox bitmap2TextBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.OpenFileDialog bitmapOpenFileDialog;
        private System.Windows.Forms.Label bitmap1Label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox xTextBox;
        private System.Windows.Forms.TextBox yTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox widthTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox heightTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox coordinatesGroupBox;
        private System.Windows.Forms.Label elapsedTimeLabel;
        private System.Windows.Forms.TrackBar toleranceTrackBar;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label toleranceValueLabel;
        private System.Windows.Forms.CheckBox autoToleranceCheckBox;
    }
}

