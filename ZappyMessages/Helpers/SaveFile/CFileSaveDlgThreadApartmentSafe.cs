using System;
using System.Threading;
using System.Windows.Forms;

namespace ZappyMessages.Helpers.SaveFile
{
    /// <summary>
    /// Simple wrapper class to show the save file dialog by using a STA thread instance in order to prevent exceptions when called from MTA threaded instances
    /// </summary>
    public class CFileSaveDlgThreadApartmentSafe : CFileDlgBase
    {
        /// <summary>
        /// uses the open file dialog in an STA thread in order to get rid of the STA/MTA issue with the open file dialog
        /// </summary>
        public override DialogResult ShowDialog()
        {
            DialogResult dlgRes = DialogResult.Cancel;

            Thread theThread = new Thread((ThreadStart)delegate
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.RestoreDirectory = true;
                //sfd.ShowHelp = true;

                if (!string.IsNullOrEmpty(this.FilePath))
                    sfd.FileName = this.FilePath;
                if (!string.IsNullOrEmpty(this.Filter))
                    sfd.Filter = this.Filter;
                if (!string.IsNullOrEmpty(this.DefaultExt))
                    sfd.DefaultExt = this.DefaultExt;
                if (!string.IsNullOrEmpty(this.Title))
                    sfd.Title = this.Title;
                if (!string.IsNullOrEmpty(this.InitialDirectory))
                    sfd.InitialDirectory = this.InitialDirectory;

                //Create a layout dialog instance on the current thread to align the file dialog
                Form frmLayout = new Form();
                frmLayout.BringToFront();
                frmLayout.TopMost = true;
                frmLayout.ShowInTaskbar = true;
                frmLayout.ShowIcon = true;
                if (this.StartupLocation != null)
                {
                    //set the hidden layout form to manual form start position
                    frmLayout.StartPosition = FormStartPosition.Manual;

                    //set the location of the form
                    frmLayout.Location = this.StartupLocation;
                    frmLayout.DesktopLocation = this.StartupLocation;
                }

                //the layout form is not visible
                frmLayout.Width = 0;
                frmLayout.Height = 0;

                //show the dialog 
                dlgRes = sfd.ShowDialog(ForegroundWindow.CurrentWindow);

                if (dlgRes == DialogResult.OK)
                    this.FilePath = sfd.FileName;
            });

            try
            {
                //set STA as the Save file dialog needs it to work
                theThread.TrySetApartmentState(ApartmentState.STA);
                theThread.Start();

                // Wait for thread to get started
                while (!theThread.IsAlive) { Thread.Sleep(1); }

                // Wait a tick more (@see: http://scn.sap.com/thread/45710)
                Thread.Sleep(1);

                //wait for the dialog thread to finish
                theThread.Join();

                DialogSuccess = true;
            }
            catch (Exception)
            {
                DialogSuccess = false;
            }

            return (dlgRes);
        }
    }
}
