using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Zappy.ZappyActions.Picture
{
    public partial class BitmapDetectorForm : Form
    {
        public BitmapDetectorForm()
        {
            InitializeComponent();
        }

        private void bitmap1Button_Click(object sender, EventArgs e)
        {
            bitmapOpenFileDialog.Filter = "BMP and PNG files (*.bmp, *.png)|*.bmp;*.png";
            if (bitmapOpenFileDialog.ShowDialog() != DialogResult.OK)
                return;
            bitmap1TextBox.Text = bitmapOpenFileDialog.FileName;
        }

        private void bitmap2Button_Click(object sender, EventArgs e)
        {
            bitmapOpenFileDialog.Filter = "BMP and PNG files (*.bmp, *.png)|*.bmp;*.png";
            if (bitmapOpenFileDialog.ShowDialog() != DialogResult.OK)
                return;
            bitmap2TextBox.Text = bitmapOpenFileDialog.FileName;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            xTextBox.Text = ""; xTextBox.Refresh();
            yTextBox.Text = ""; yTextBox.Refresh();
            widthTextBox.Text = ""; widthTextBox.Refresh();
            heightTextBox.Text = ""; heightTextBox.Refresh();
            elapsedTimeLabel.Text = ""; elapsedTimeLabel.Refresh();

            string validateMsg = validateData();
            if (validateMsg != "")
            {
                MessageBox.Show(validateMsg, "Data error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bitmap bitmap1 = new Bitmap(bitmap1TextBox.Text);
            Bitmap bitmap2 = new Bitmap(bitmap2TextBox.Text);

            if (bitmap1.Width > bitmap2.Width || bitmap1.Height > bitmap2.Height)
            {
                Bitmap aux = bitmap2;
                bitmap2 = bitmap1;
                bitmap1 = aux;
            }

            if (bitmap1.Height > bitmap2.Height)
            {
                MessageBox.Show("None of the Bitmaps can contain the other.", "Data error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            Rectangle location = Rectangle.Empty;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (!autoToleranceCheckBox.Checked)
            {
                double tolerance = Convert.ToDouble(toleranceTrackBar.Value) / 100.0;
                location = searchBitmap(bitmap1, bitmap2, tolerance);
            }
            else
            {
                location = autoSearchBitmap(bitmap1, bitmap2);
            }
            stopWatch.Stop();

            if (location.Width != 0)
            {
                elapsedTimeLabel.Text = "Bitmap found in " + stopWatch.ElapsedMilliseconds + " ms.";
                xTextBox.Text = location.X.ToString();
                yTextBox.Text = location.Y.ToString();
                widthTextBox.Text = bitmap1.Width.ToString();
                heightTextBox.Text = bitmap1.Height.ToString();
            }
            else
            {
                elapsedTimeLabel.Text = "Bitmap not found.";

            }

            bitmap1.Dispose();
            bitmap2.Dispose();
            this.Cursor = Cursors.Default;
        }

        private string validateData()
        {
            if (bitmap1TextBox.Text == "")
                return "You must choose a bitmap for Bitmap 1.";
            if (bitmap2TextBox.Text == "")
                return "You must choose a bitmap for Bitmap 2.";
            return "";
        }

        private Rectangle autoSearchBitmap(Bitmap bitmap1, Bitmap bitmap2)
        {
            Rectangle location = Rectangle.Empty;
            for (int i = 0; i <= toleranceTrackBar.Maximum; i++)
            {
                toleranceTrackBar.Value = i;
                toleranceTrackBar.Refresh();
                double tolerance = Convert.ToDouble(i) / 100.0;

                location = searchBitmap(bitmap1, bitmap2, tolerance);

                if (location.Width != 0)
                    break;
            }
            return location;
        }

        private Rectangle searchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance)
        {
            BitmapData smallData =
              smallBmp.LockBits(new Rectangle(0, 0, smallBmp.Width, smallBmp.Height),
                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                       System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bigData =
              bigBmp.LockBits(new Rectangle(0, 0, bigBmp.Width, bigBmp.Height),
                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                       System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int smallStride = smallData.Stride;
            int bigStride = bigData.Stride;

            int bigWidth = bigBmp.Width;
            int bigHeight = bigBmp.Height - smallBmp.Height + 1;
            int smallWidth = smallBmp.Width * 3;
            int smallHeight = smallBmp.Height;

            Rectangle location = Rectangle.Empty;
            int margin = Convert.ToInt32(255.0 * tolerance);

            unsafe
            {
                byte* pSmall = (byte*)(void*)smallData.Scan0;
                byte* pBig = (byte*)(void*)bigData.Scan0;

                int smallOffset = smallStride - smallBmp.Width * 3;
                int bigOffset = bigStride - bigBmp.Width * 3;

                bool matchFound = true;

                for (int y = 0; y < bigHeight; y++)
                {
                    for (int x = 0; x < bigWidth; x++)
                    {
                        byte* pBigBackup = pBig;
                        byte* pSmallBackup = pSmall;

                        //Look for the small picture.
                        for (int i = 0; i < smallHeight; i++)
                        {
                            int j = 0;
                            matchFound = true;
                            for (j = 0; j < smallWidth; j++)
                            {
                                //With tolerance: pSmall value should be between margins.
                                int inf = pBig[0] - margin;
                                int sup = pBig[0] + margin;
                                if (sup < pSmall[0] || inf > pSmall[0])
                                {
                                    matchFound = false;
                                    break;
                                }

                                pBig++;
                                pSmall++;
                            }

                            if (!matchFound) break;

                            //We restore the pointers.
                            pSmall = pSmallBackup;
                            pBig = pBigBackup;

                            //Next rows of the small and big pictures.
                            pSmall += smallStride * (1 + i);
                            pBig += bigStride * (1 + i);
                        }

                        //If match found, we return.
                        if (matchFound)
                        {
                            location.X = x;
                            location.Y = y;
                            location.Width = smallBmp.Width;
                            location.Height = smallBmp.Height;
                            break;
                        }
                        //If no match found, we restore the pointers and continue.
                        else
                        {
                            pBig = pBigBackup;
                            pSmall = pSmallBackup;
                            pBig += 3;
                        }
                    }

                    if (matchFound) break;

                    pBig += bigOffset;
                }
            }

            bigBmp.UnlockBits(bigData);
            smallBmp.UnlockBits(smallData);

            return location;
        }

        private void toleranceTrackBar_ValueChanged(object sender, EventArgs e)
        {
            toleranceValueLabel.Text = toleranceTrackBar.Value.ToString() + " %";
            toleranceValueLabel.Refresh();
        }

        private void autoToleranceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autoToleranceCheckBox.Checked)
            {
                toleranceTrackBar.Value = 0;
                toleranceTrackBar.Enabled = false;
            }
            else
            {
                toleranceTrackBar.Enabled = true;
            }
        }
    }
}
