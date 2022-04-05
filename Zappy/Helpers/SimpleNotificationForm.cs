using System;
using System.Windows.Forms;

namespace Zappy.Helpers
{
    public partial class SimpleNotificationForm : Form
    {
        public SimpleNotificationForm(string label)
        {
            InitializeComponent();
            labelNotify.AutoSize = true;
            labelNotify.Text = label;
        }
        public SimpleNotificationForm()
        {
            InitializeComponent();
        }



        private void SimpleNotificationForm_Load(object sender, EventArgs e)
        {
            //if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            //{
            //    ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //public void ChangeLanguage(LanguageZappy languageZappy)
        //{
        //    try
        //    {
        //        string lang = "en";
        //        if (languageZappy == LanguageZappy.jp)
        //        {
        //            lang = "ja-JP";
        //        }
        //        else if (languageZappy == LanguageZappy.pol)
        //        {
        //            lang = "pl";
        //        }
        //        ComponentResourceManager resources = null;
        //        foreach (Control c in this.Controls)
        //        {
        //            //if (c is Panel)
        //            //{
        //            foreach (Control c1 in c.Controls)
        //            {
        //                resources = new ComponentResourceManager(typeof(SimpleNotificationForm));
        //                resources.ApplyResources(c1, c1.Name, new CultureInfo(lang));
        //            }
        //            //  }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CrapyLogger.log.Error(ex);
        //    }
        //}
    }
}
