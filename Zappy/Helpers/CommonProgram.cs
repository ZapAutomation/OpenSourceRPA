using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;
using Zappy.ZappyTaskEditor.ExecutionHelpers;
using ZappyMessages;
using ZappyMessages.Helpers;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;
using ZappyMessages.Util.Helper;

namespace Zappy.Helpers
{
    public static class CommonProgram
    {
        //public static StringWriter textWriterXML;// = new StringWriter()

        public static void sendFolderToZappyTeam(string folderName)
        {
            try
            {

                //Copy Folder to allow Zipping
                DirectoryCopy(folderName, CrapyConstants.LogFolderCopy, true);
                //Zip
                if (File.Exists(CrapyConstants.ZipFileLoc))
                {
                    File.Delete(CrapyConstants.ZipFileLoc);
                }

                System.IO.Compression.ZipFile.CreateFromDirectory(CrapyConstants.LogFolderCopy,
                    CrapyConstants.ZipFileLoc);
                //Delete Temp Folder Contents
                CleanupFolderAndAllFiles(CrapyConstants.LogFolderCopy);

            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        public static bool SetTextInClipboard(object Text, bool Copy = true, int MaxRetryCount = 5,
            int RetryMillisecs = 200)
        {
            int _RetryCount = 0;
        __RETRYCOPY:
            try
            {
                Clipboard.SetDataObject(Text, true, MaxRetryCount, RetryMillisecs);
                return true;
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);

                if (++_RetryCount <= MaxRetryCount)
                {
                    Thread.Sleep(RetryMillisecs);
                    goto __RETRYCOPY;
                }
                else
                    return false;
            }
        }

        public static bool GetTextFromClipboard(out string Text, int MaxRetryCount = 5, int RetryMillisecs = 200)
        {
            int _RetryCount = 0;
        __RETRYCOPY:
            try
            {
                Text = Clipboard.GetText();
                return true;
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);

                if (++_RetryCount <= MaxRetryCount)
                {
                    Thread.Sleep(RetryMillisecs);
                    goto __RETRYCOPY;
                }
                else
                {
                    Text = "<ERROR>";
                    return false;
                }
            }
        }      

        public static T DeepClone<T>(this T toClone) // where T : IZappyAction
        {
            //lock (textWriterXML)
            using (StringWriter textWriterXML = new StringWriter())
            {
                //textWriterXML = new StringWriter();
                XmlSerializer serilizer = null;
                int _Index = Array.IndexOf(ActionTypeInfo.SupportedActionTypes, toClone.GetType());
                if (_Index >= 0)
                {
                    serilizer = ActionTypeInfo.SupportedActionTypesSerializers[_Index];
                }
                else
                {
                    serilizer = new XmlSerializer(typeof(T));
                }

                serilizer.Serialize(textWriterXML, toClone);
                //required for preserving whitespaces
                XmlReader xr = XmlReader.Create(new StringReader(textWriterXML.ToString()));
                return (T)serilizer.Deserialize(xr);
            }

            return default(T);
        }

        public static DialogResult ShowInputBox(string title, string promptText, string FieldName,
            System.Drawing.Icon Icon, ref string value, bool password)
        {
            InputBox form = new InputBox(title, promptText, FieldName, Icon, value, password);
            DialogResult dialogResult = form.ShowDialog();
            value = form.Input;
            return dialogResult;
        }

        public static IZappyAction DeepCloneAction(this IZappyAction Action)
        {
            Action = DeepClone<IZappyAction>(Action);
            Action.NextGuid = Action.SelfGuid = Guid.Empty;
            return Action;
        }

        public static void CleanupFolderAndAllFiles(string folderToDelete)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(folderToDelete);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch
            {

            }

        }

        public static void CleanupFoldersForAppLaunch()
        {
            for (int i = 0; i < CrapyConstants.CleanupFoldersOnAppLaunch.Length; i++)
            {
                CleanupFolderAndAllFiles(CrapyConstants.CleanupFoldersOnAppLaunch[i]);
            }
        }

        public static Process StartInvisibleConsole(string _AppPath, string _Arguments)
        {
            ProcessStartInfo _info = new ProcessStartInfo();
            _info.FileName = _AppPath;
            _info.Arguments = _Arguments;
            _info.UseShellExecute = false;
            _info.CreateNoWindow = true;
            _info.WindowStyle = ProcessWindowStyle.Hidden;
            return Process.Start(_info);
        }

        public static string GetZappyTempFile()
        {
            return Path.Combine(CrapyConstants.TempFolder, string.Format("_Zappy_{0}_{1}.tmp",
                DateTime.Now.ToString("yyyyMMddHHmmss"),
                Guid.NewGuid().ToString()));
        }

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static void CSharpPreCompileRunSctipt()
        {
            RunScriptActivity runScript = new RunScriptActivity();
            runScript.Invoke(new ZappyExecutionContext(new ZappyTask()), null);
        }

        public static void PrecompileAssemblies()
        {
            //Loads supported action type in a seperate thread
            Task T1 = Task.Factory.StartNew(() =>
                {
                    //loads supportedActionTypes - one of the slowest calls - Static loading
                    var supportedActionTypes = ActionTypeInfo.SupportedActionTypes;
                    //Added so that optimisation does not delete above code
                    if (supportedActionTypes == null)
                    {
                        Console.WriteLine("Null supportedActionTypes");
                    }
                }
            );
            //Task T1 = Task.Factory.StartNew(() =>
            //    {
            //        try
            //        {
            //            List<Type> _TypeList = new List<Type>(GetLoadableTypes(Assembly.GetExecutingAssembly()));
            //            _TypeList.Insert(0, typeof(frmZappy));

            //            foreach (var type in _TypeList)
            //            {
            //                if (!type.IsAbstract)
            //                    //CrapyLogger.log.ErrorFormat("Starting to compile :" + type.Name);
            //                {
            //                    foreach (var method in type.GetMethods(BindingFlags.DeclaredOnly |
            //                                                           BindingFlags.NonPublic |
            //                                                           BindingFlags.Public | BindingFlags.Instance |
            //                                                           BindingFlags.Static))
            //                    {
            //                        try
            //                        {
            //                            System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(
            //                                method.MethodHandle);

            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            //CrapyLogger.log.Error(ex);
            //                        }

            //                    }
            //                }

            //                //CrapyLogger.log.Error("Finished compiling :" + type.Name);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            CrapyLogger.log.Error(ex);
            //        }

            //        //CrapyLogger.log.Error(":-----------------------------------------------------------------Finished compiling :-----------------------------------------------------------------");

            //    }

            //);
        }

        public static void GenerateGUID()
        {
            string _UserKey = string.Empty;

            if (!File.Exists(CrapyConstants.InstIdPath))
            {
                _UserKey = Guid.NewGuid().ToString("N");
                File.WriteAllText(CrapyConstants.InstIdPath, _UserKey);
            }
        }

        public static Dictionary<string, Assembly> _AssemblyCahce = new Dictionary<string, Assembly>();

        public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_AssemblyCahce.ContainsKey(args.Name))
                return _AssemblyCahce[args.Name];
            return null;
        }

        public static void AssemblyResolve()
        {
            //Environment.SetEnvironmentVariable("BaseFolder", CrapyConstants.BaseFolder);
            if (!File.Exists(CrapyConstants.InstIdPath))
                CommonProgram.GenerateGUID();
            CrapyConstants.InstID = File.ReadAllText(CrapyConstants.InstIdPath);

            String[]
                _FileNames =
                    {"II.dll"}; //new string[] { "UiaComWrapper.dll", "Interop.UIAutomationClient.dll" };
            for (int i = 0; i < _FileNames.Length; i++)
            {
                string _FileName = Path.Combine(ZappyMessagingConstants.StartupPath, _FileNames[i]);
                if (File.Exists(_FileName))
                {
                    Assembly asm = Assembly.UnsafeLoadFrom(_FileName);
                    _AssemblyCahce[asm.FullName] = asm;
                }

            }
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            //Remove if not using BitMiraclep
            //BitMiracle.Docotic.LicenseManager.AddLicenseData("68FXY-EDOLV-WVQP2-KTZ3L-BCFSA");
        }

        public static void LoadExternalAssembly(Assembly asm)
        {
            _AssemblyCahce[asm.FullName] = asm;
            //foreach (Type type in asm.GetExportedTypes())
            //{
            //    foreach (var method in type.GetMethods(BindingFlags.DeclaredOnly |
            //             BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            //    {
            //        try
            //        {
            //            System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(
            //                method.MethodHandle);

            //        }
            //        catch (Exception ex)
            //        {
            //            CrapyLogger.log.Error(ex);
            //        }

            //    }
            //}
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public static void FreeVersionMessageBox()
        {
            MessageBox.Show(
                        "This feature is available for premium and " +
                        "enterprise editions only - " +
                        "please visit https://zappy.ai/ to get this feature");
        }

        internal static Process RunProcessAsTask(string exePath, string args = "")
        {

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = exePath;

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = args;
            Process _ReturnProcess = new Process();
            _ReturnProcess.StartInfo = startInfo;
            _ReturnProcess.EnableRaisingEvents = true;
            try
            {
                _ReturnProcess.Start();
            }
            catch // (Exception e)
            {
            }

            //Process process = Process.Start(Path.Combine( CrapyConstants.StartupPath , "ZappyHelper.exe"));
            // Add the Process to ChildProcessTracker.
            ChildProcessTracker.AddProcess(_ReturnProcess);
            return _ReturnProcess;

        }

        public static void StartPlaybackFromFile(string FilePath, bool Debug = false)
        {
            try
            {

                try
                {
                    PubSubService.Instance.Publish(
                        Debug ? PubSubTopicRegister.DebugTask : PubSubTopicRegister.StartPlaybackFromFile, FilePath);
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.ErrorFormat("Error in Action Replay! {0}", ex);
                    MessageBox.Show("Error in Action Replay!");
                }

            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                MessageBox.Show("ERR:1501 - Error in task execution!");
            }
        }

        public static void StartPlaybackFromIZappyAction(IZappyAction action)
        {
            string data = string.Empty;

            using (StringWriter s = new StringWriter())
            {
                ActionTypeInfo
                    .SupportedActionTypesSerializers[
                        Array.IndexOf(ActionTypeInfo.SupportedActionTypes, action.GetType())]
                    .Serialize(s, action);
                data = s.ToString();
            }
            if (!string.IsNullOrEmpty(data))
            {
                CommonProgram.StartPlaybackFromAction(data);
            }
        }

        public static void StartPlaybackFromAction(string ActionDetails)
        {
            try
            {
                //UserTextDataManager.SaveTextData();
                try
                {
                    PubSubService.Instance.Publish(PubSubTopicRegister.StartPlaybackFromActions, ActionDetails);
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.ErrorFormat("Error in Action Replay! {0}", ex);

                    MessageBox.Show("Error in Action Replay!");
                }

            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                MessageBox.Show("ERR:1501 - Error in task execution!");
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);
                try
                {
                    // Copy the file. overwrite if already existing
                    file.CopyTo(temppath, true);
                }
                catch
                {

                }
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }

        }
       
        public static string HumanizeNameForGivenType(Type type)
        {
            string humanisedType = string.Empty;
            HelperFunctions.TypeToHumanizedString.TryGetValue(type, out humanisedType);
            //Commenting out based on feedback
            //if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            //{
            //    humanisedType = LocalizeTaskEditorHelper.ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage, humanisedType);
            //}

            return humanisedType;
        }
    }
}