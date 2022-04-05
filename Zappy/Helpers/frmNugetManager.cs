using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Invoker;
using Zappy.ZappyTaskEditor;
using Zappy.ZappyTaskEditor.Helper;
using ZappyMessages;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace Zappy.Helpers
{

    public partial class frmNugetManager : Form
    {
        public frmNugetManager()
        {
            InitializeComponent();
        }

        private void frmNugetManager_Load(object sender, EventArgs e)
        {
            ShowInstalledDll();

            if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            {
                ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
            }
        }


        private void ShowInstalledDll()
        {
            string[] dllfiles = Directory.GetFiles(CrapyConstants.ProfileDllDirectory);
            foreach (string dllfile in dllfiles)
            {
                string fname = Path.GetFileName(dllfile);
                CustomListBoxHelper customListBoxHelper = new CustomListBoxHelper();
                customListBoxHelper.DisplayText = fname;
                customListBoxHelper.Path = dllfile;
                checkedListBox1.Items.Add(customListBoxHelper);
                //    txtinfo.AppendText(Environment.NewLine);
            }
            string[] dllfolders = Directory.GetDirectories(CrapyConstants.ProfileDllDirectory);
            foreach (string dllfolder in dllfolders)
            {
                string fname = Path.GetFileName(dllfolder);
                CustomListBoxHelper customListBoxHelper = new CustomListBoxHelper();
                customListBoxHelper.DisplayText = fname;
                customListBoxHelper.Path = dllfolder;
                checkedListBox1.Items.Add(customListBoxHelper);
                //    txtinfo.AppendText(Environment.NewLine);
            }
        }

        private void loadDll_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "DLL Files|*.dll";
            openFileDialog1.Title = "Select a DLL File To Load";
            openFileDialog1.Multiselect = true;
            openFileDialog1.ValidateNames = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog1.FileNames)
                {
                    try
                    {

                        //Copy DLL it to appdata folder
                        File.Copy(file, Path.Combine(CrapyConstants.ProfileDllDirectory, Path.GetFileName(file)));
                        //start in seperate thread - loads it from folder
                        loadAndShowLoadedPackages();


                    }
                    catch (Exception ex)
                    {
                        CrapyLogger.log.Error(ex);
                    }
                }
                MessageBox.Show("Successfully Loaded Extensions");
            }
        }

        void loadAndShowLoadedPackages()
        {
            ActionTypeInfo.LoadActionTypeInfo();

            //Save it in settings so that Zappy Loads it again during launch

            //send it to playback helper
            PubSubService.Instance.Publish(PubSubTopicRegister.ControlSignals, PubSubMessages.ReloadAssembliesFromDllXMLMessage);
            ShowInstalledDll();
        }

        private void btnuninstall_Click(object sender, EventArgs e)
        {
            foreach (var checkeditem in checkedListBox1.CheckedItems.OfType<CustomListBoxHelper>().ToList())
            {
                CustomListBoxHelper item = checkeditem as CustomListBoxHelper;
                if (Directory.Exists(item.Path))
                {
                    CommonProgram.CleanupFolderAndAllFiles(item.Path);
                    Directory.Delete(item.Path);
                }
                else
                    File.Delete(item.Path);
                checkedListBox1.Items.Remove(checkeditem);
            }
            loadAndShowLoadedPackages();
        }

        private void loadNugetPackage_Click(object sender, EventArgs e)
        {
            //string extractPath = CrapyConstants.ProfileDllDirectory;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Nuget / Zip package to load";
            openFileDialog.Multiselect = true;
            openFileDialog.ValidateNames = true;
            openFileDialog.Filter = "Nuget or Zip files|*.nupkg;*.zip*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    try
                    {
                        ZipFile.ExtractToDirectory(file, 
                            Path.Combine(CrapyConstants.ProfileDllDirectory, Path.GetFileNameWithoutExtension(file)));
                        loadAndShowLoadedPackages();
                    }
                    catch
                    {
                    }
                }
            }
        }


        public void ChangeLanguage(LanguageZappy languageZappy)
        {
            try
            {
                string lang = LocalizeTaskEditorHelper.LanguagePicker(languageZappy);
                ComponentResourceManager resources = null;
                foreach (Control c in this.Controls)
                {
                    if (c is Panel)
                    {
                        foreach (Control c1 in c.Controls)
                        {
                            resources = new ComponentResourceManager(typeof(frmNugetManager));
                            resources.ApplyResources(c1, c1.Name, new CultureInfo(lang));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }
    }
}
