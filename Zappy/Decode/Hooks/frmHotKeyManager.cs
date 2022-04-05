using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.Invoker;
using Zappy.ZappyTaskEditor;
using Zappy.ZappyTaskEditor.Helper;
using ZappyMessages;
using ZappyMessages.Helpers;
using Enum = System.Enum;

namespace Zappy.Decode.Hooks
{
    public partial class frmHotKeyManager : Form
    {
        public frmHotKeyManager()
        {
            InitializeComponent();
        }

        private void HotKeyManager_Load(object sender, EventArgs e)
        {
            string[] _Names = Enum.GetNames(typeof(Keys));
            comboBoxLearnedActions.Items.AddRange(_Names);
            comboBoxLearnedActions.SelectedItem = HotKeyHandler.Instance.learnedActionsKey.ToString();
            comboBoxExecuteFirstTask.Items.AddRange(_Names);
            comboBoxExecuteFirstTask.SelectedItem = HotKeyHandler.Instance.executeFirstTaskKey.ToString();
            comboBoxCancelExecution.Items.AddRange(_Names);
            comboBoxCancelExecution.SelectedItem = HotKeyHandler.Instance.cancelExecutionKey.ToString();
            comboBoxExportLearnedActions.Items.AddRange(_Names);
            comboBoxExportLearnedActions.SelectedItem = HotKeyHandler.Instance.exportLearnedActions.ToString();

            if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            {
                ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
            }
        }

        private void ok_Click(object sender, EventArgs e)
        {
            HotKeyHandler.Instance.learnedActionsKey = (Keys)Enum.Parse(typeof(Keys), comboBoxLearnedActions.SelectedItem.ToString());
            HotKeyHandler.Instance.executeFirstTaskKey = (Keys)Enum.Parse(typeof(Keys), comboBoxExecuteFirstTask.SelectedItem.ToString());
            HotKeyHandler.Instance.cancelExecutionKey = (Keys)Enum.Parse(typeof(Keys), comboBoxCancelExecution.SelectedItem.ToString());
            HotKeyHandler.Instance.exportLearnedActions = (Keys)Enum.Parse(typeof(Keys), comboBoxExportLearnedActions.SelectedItem.ToString());
            File.WriteAllText(CrapyConstants.ZappyShortcuts, ZappySerializer.SerializeObject(HotKeyHandler.Instance));
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
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
                    resources = new ComponentResourceManager(typeof(frmHotKeyManager));
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
