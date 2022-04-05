using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.ZappyTaskEditor;
using Zappy.ZappyTaskEditor.Helper;
using ZappyMessages;
using ZappyMessages.Helpers;

namespace Zappy.Invoker
{
    public partial class frmApplicationSettings : Form
    {
        public frmApplicationSettings()
        {
            InitializeComponent();
            checkBoxExcelRecordMouse.Checked = ApplicationSettingProperties.Instance.ExcelRecordMouseAction;
            launchAppCheckBox.Checked = ApplicationSettingProperties.Instance.EnableLaunchActivityRecording;
            checkBoxChromeXpath.Checked = ApplicationSettingProperties.Instance.ChromeXPAthOnlyRecord;
            //checkBoxChromePause.Checked = ApplicationSettingProperties.Instance.ChromeInsertPause;
            checkBoxTrapy.Checked = ApplicationSettingProperties.Instance.EnableTrapy;
            checkBoxTraceLog.Checked = ApplicationSettingProperties.Instance.EnableTraceLog;
            fullAuditCheckBox.Checked = ApplicationSettingProperties.Instance.EnableFullAuditLog;
            checkBoxChromeNative.Checked = ApplicationSettingProperties.Instance.EnableChromeNativeRecording;
            enableMLLearn.Checked = ApplicationSettingProperties.Instance.SaveMLLearningFile;
            checkboxEnableZappyShortcuts.Checked = ApplicationSettingProperties.Instance.EnableZappyShortcuts;
            checkBoxEnableScreenshot.Checked = ApplicationSettingProperties.Instance.EnableRecordScreenshots;
            enableJava32Bit.Checked = ApplicationSettingProperties.Instance.EnableJava32Bit;
            enableJava64Bit.Checked = ApplicationSettingProperties.Instance.EnableJava64Bit;


            if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            {
                ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
            }

        }

        private void Okay_Click(object sender, EventArgs e)
        {
            try
            {
                ApplicationSettingProperties.Instance.ExcelRecordMouseAction = checkBoxExcelRecordMouse.Checked;
                ApplicationSettingProperties.Instance.EnableLaunchActivityRecording = launchAppCheckBox.Checked;
                ApplicationSettingProperties.Instance.ChromeXPAthOnlyRecord = checkBoxChromeXpath.Checked;

                ApplicationSettingProperties.Instance.EnableTrapy = checkBoxTrapy.Checked;
                ApplicationSettingProperties.Instance.EnableFullAuditLog = fullAuditCheckBox.Checked;
                ApplicationSettingProperties.Instance.EnableTraceLog = checkBoxTraceLog.Checked;
                ApplicationSettingProperties.Instance.EnableChromeNativeRecording = checkBoxChromeNative.Checked;
                ApplicationSettingProperties.Instance.SaveMLLearningFile = enableMLLearn.Checked;
                ApplicationSettingProperties.Instance.EnableZappyShortcuts = checkboxEnableZappyShortcuts.Checked;
                ApplicationSettingProperties.Instance.EnableRecordScreenshots = checkBoxEnableScreenshot.Checked;

                ApplicationSettingProperties.Instance.EnableJava32Bit = enableJava32Bit.Checked;
                ApplicationSettingProperties.Instance.EnableJava64Bit = enableJava64Bit.Checked;

                File.WriteAllText(CrapyConstants.ZappySettings, ZappySerializer.SerializeObject(ApplicationSettingProperties.Instance));
            }
            catch
            {
                MessageBox.Show("Error Setting Requested Property");
            }
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ChangeLanguage(LanguageZappy languageZappy)
        {
            try
            {
                string lang = LocalizeTaskEditorHelper.LanguagePicker(languageZappy);

                ComponentResourceManager resources = null;
                foreach (Control c in this.Controls)
                {
                    resources = new ComponentResourceManager(typeof(frmApplicationSettings));
                    resources.ApplyResources(c, c.Name, new CultureInfo(lang));
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }  
    }
}
