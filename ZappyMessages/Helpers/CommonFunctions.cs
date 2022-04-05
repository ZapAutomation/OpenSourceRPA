using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZappyMessages.Helpers
{
    public static class CommonFunctions
    {
        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSEnableChildSessions(bool enable);

        public static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public static void SetStartup(bool setup, string[] args = null)
        {
            try
            {
                RegistryKey rkStartup = Registry.LocalMachine.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                //gets the location from where the aseembly is executed - EXE file and not the class library
                string _ExePath = Assembly.GetEntryAssembly().Location;
                string _ExeDirectory = Path.GetDirectoryName(_ExePath);
                string _ExeFolderWithJson = Path.Combine(_ExeDirectory, "com.zappy.integerai.json");
                string _ExeFolderWithFirefoxJson = Path.Combine(_ExeDirectory, "com.zappy.firefox.json");

                string _AppName = Path.GetFileNameWithoutExtension(_ExePath);

                RegistryKey rkChromeNative = Registry.LocalMachine.CreateSubKey
                    ("Software\\Google\\Chrome\\NativeMessagingHosts\\com.zappy.integerai", true);
                RegistryKey rkChromeExtensionInstall = Registry.LocalMachine.CreateSubKey
                    ("Software\\Google\\Chrome\\Extensions\\pdjnknajejdelpmkkocjbcoplofadlcg", true);

                //Firefox
                RegistryKey rkFirefoxNative = Registry.LocalMachine.CreateSubKey
                    ("SOFTWARE\\Mozilla\\NativeMessagingHosts\\com.zappy.integerai", true);

                RegistryKey rkExcelPluginx86 = Registry.LocalMachine.CreateSubKey
                    ("SOFTWARE\\Microsoft\\Office\\Excel\\Addins\\Zappy.ExcelAddIn", true);
                RegistryKey rkOutlookPluginx86 = Registry.LocalMachine.CreateSubKey
                    ("SOFTWARE\\Microsoft\\Office\\Outlook\\Addins\\Zappy.OutlookAddIn", true);
                //Todo add support for 64 bit
                RegistryKey localKey;
                try
                {
                    localKey =
                        RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine,
                            RegistryView.Registry64);
                }
                catch
                {
                    localKey =
                        RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Default);
                }
                RegistryKey rkExcelPluginx64 = localKey.CreateSubKey
                    ("SOFTWARE\\Microsoft\\Office\\Excel\\Addins\\Zappy.ExcelAddIn", true);
                RegistryKey rkOutlookPluginx64 = localKey.CreateSubKey
                    ("SOFTWARE\\Microsoft\\Office\\Outlook\\Addins\\Zappy.OutlookAddIn", true);

                RegistryKey rkFirefoxNativex64 = localKey.CreateSubKey
                   ("SOFTWARE\\Mozilla\\NativeMessagingHosts\\com.zappy.integerai", true);
                //RegistryKey rkExcelPluginx64 = Registry.LocalMachine.CreateSubKey
                //    ("Software\\Wow6432Node\\Microsoft\\Office\\Excel\\Addins\\Zappy.ExcelAddIn", true);
                //RegistryKey rkOutlookPluginx64 = Registry.LocalMachine.CreateSubKey
                //    ("Software\\Wow6432Node\\Microsoft\\Office\\Outlook\\Addins\\Zappy.OutlookAddIn", true);


                //string _RdpWrapFileName = Path.Combine(CrapyConstants.StartupPath, "RDP", "RDPWInst.exe");
                //CrapyLogger.log.Error(_RdpWrapFileName);

                if (setup)
                {
                    try
                    {
                        ProcessStartInfo _info = new ProcessStartInfo();
                        string vc2017x86 = Path.Combine(ZappyMessagingConstants.ExtensionsFolder,
                            "VC_redist.x862017.exe");
                        _info.FileName = vc2017x86;
                        _info.Arguments = "/install /passive /norestart";
                        _info.UseShellExecute = false;
                        Process proc = Process.Start(_info);
                    }
                    catch
                    {
                    }

                    rkStartup.SetValue(_AppName, _ExePath);
                    try
                    {
                        if (rkChromeNative.GetValue("") != null)
                            rkChromeNative.DeleteValue("");
                    }
                    catch
                    {

                    }

                    rkChromeNative.SetValue("", _ExeFolderWithJson, RegistryValueKind.String);
                    rkChromeExtensionInstall.SetValue("update_url", "https://clients2.google.com/service/update2/crx");

                    //update path to contain firefox manifest
                    rkFirefoxNative.SetValue("", _ExeFolderWithFirefoxJson, RegistryValueKind.String);
                    rkFirefoxNativex64.SetValue("", _ExeFolderWithFirefoxJson, RegistryValueKind.String);


                    rkExcelPluginx86.SetValue("FriendlyName", "ZappyExcel");
                    rkExcelPluginx86.SetValue("Manifest",
                        "file:///" + _ExeDirectory + "\\Plugins\\ZappyExcelAddIn.vsto|vstolocal");
                    rkExcelPluginx86.SetValue("Description", "Zappy Excel AddIn");
                    rkExcelPluginx86.SetValue("LoadBehavior", 3);

                    rkExcelPluginx64.SetValue("FriendlyName", "ZappyExcel");
                    rkExcelPluginx64.SetValue("Manifest",
                        "file:///" + _ExeDirectory + "\\Plugins\\ZappyExcelAddIn.vsto|vstolocal");
                    rkExcelPluginx64.SetValue("Description", "Zappy Excel AddIn");
                    rkExcelPluginx64.SetValue("LoadBehavior", 3);

                    //rkExcelPluginx64.SetValue("FriendlyName", "ZappyExcel");
                    //rkExcelPluginx64.SetValue("Manifest", "file:///" + _ExeDirectory + "ZappyExcelAddIn.vsto|vstolocal");

                    rkOutlookPluginx86.SetValue("FriendlyName", "ZappyOutlook");
                    rkOutlookPluginx86.SetValue("Manifest",
                        "file:///" + _ExeDirectory + "\\Plugins\\ZapppyOutlookAddIn.vsto|vstolocal");
                    rkOutlookPluginx86.SetValue("Description", "Zappy Outlook AddIn");
                    rkOutlookPluginx86.SetValue("LoadBehavior", 3);

                    rkOutlookPluginx64.SetValue("FriendlyName", "ZappyOutlook");
                    rkOutlookPluginx64.SetValue("Manifest",
                        "file:///" + _ExeDirectory + "\\Plugins\\ZapppyOutlookAddIn.vsto|vstolocal");
                    rkOutlookPluginx64.SetValue("Description", "Zappy Outlook AddIn");
                    rkOutlookPluginx64.SetValue("LoadBehavior", 3);

                    //RunJavaBridgeProcess("/enable");

                    //call this while installation - requires admin previlages
                    var invoke = WTSEnableChildSessions(true);

                }
                else
                {
                    rkStartup.DeleteValue(_AppName, false);
                    rkChromeNative.DeleteValue("", false);
                    rkChromeExtensionInstall.DeleteValue("update_url", false);
                    rkFirefoxNative.DeleteValue("", false);
                    rkFirefoxNativex64.DeleteValue("", false);

                    rkExcelPluginx86.DeleteValue("FriendlyName", false);
                    rkExcelPluginx86.DeleteValue("Manifest", false);

                    rkOutlookPluginx86.DeleteValue("FriendlyName", false);
                    rkOutlookPluginx86.DeleteValue("Manifest", false);

                    rkExcelPluginx64.DeleteValue("FriendlyName", false);
                    rkExcelPluginx64.DeleteValue("Manifest", false);

                    rkOutlookPluginx64.DeleteValue("FriendlyName", false);
                    rkOutlookPluginx64.DeleteValue("Manifest", false);

                    //CleanupFolderAndAllFiles(CrapyConstants.SamplesFolder);
                    //rkExcelPluginx64.DeleteValue("FriendlyName", false);
                    //rkExcelPluginx64.DeleteValue("Manifest", false);
                    ////if (File.Exists(_RdpWrapFileName))
                    //{
                    //    ProcessStartInfo psi = new ProcessStartInfo(_RdpWrapFileName);
                    //    psi.Arguments = "-u";
                    //    psi.UseShellExecute = false;
                    //    Process.Start(psi);
                    //}
                    //RunJavaBridgeProcess("/disable");

                    //call this while installation - requires admin previlages
                    var invoke = WTSEnableChildSessions(false);
                }

                rkStartup.Close();
                rkChromeNative.Close();
                rkChromeExtensionInstall.Close();
                rkExcelPluginx86.Close();
                rkOutlookPluginx86.Close();
                rkExcelPluginx64.Close();
                rkOutlookPluginx64.Close();
            }
            catch (Exception ex)
            {
                //CrapyLogger.log.Error(ex);
            }

        }

        const string JAVA_KEY = "SOFTWARE\\JavaSoft\\Java Runtime Environment\\";

        public static string GetJavaInstallationPath()
        {
            string environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(environmentPath))
            {
                return environmentPath;
            }
            string javapath = GetJava32bitInstallationPath();

            if (javapath != null)
                return javapath;
            else
                return GetJava64bitInstallationPath();
        }

        public static string GetJava32bitInstallationPath()
        {
            var localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
            using (var rk = localKey.OpenSubKey(JAVA_KEY))
            {
                if (rk != null)
                {
                    string currentVersion = rk.GetValue("CurrentVersion").ToString();
                    using (var key = rk.OpenSubKey(currentVersion))
                    {
                        return key.GetValue("JavaHome").ToString();
                    }
                }
            }
            return null;
        }

        public static string GetJava64bitInstallationPath()
        {
            var localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
            using (var rk = localKey.OpenSubKey(JAVA_KEY))
            {
                if (rk != null)
                {
                    string currentVersion = rk.GetValue("CurrentVersion").ToString();
                    using (var key = rk.OpenSubKey(currentVersion))
                    {
                        return key.GetValue("JavaHome").ToString();
                    }
                }
            }
            return null;
        }

        public static void RunJavaBridgeProcess(string argument)
        {
            try
            {
                string JavainstallPath = CommonFunctions.GetJava32bitInstallationPath();

                if(JavainstallPath != null)
                {
                    string JabswitchfilePath = System.IO.Path.Combine(JavainstallPath, "bin\\jabswitch.exe");
                    ProcessStartInfo _info = new ProcessStartInfo();
                    _info.FileName = JabswitchfilePath;
                    _info.Arguments = argument;
                    _info.UseShellExecute = false;
                    Process proc = Process.Start(_info);
                    proc.WaitForExit();
                }

                //for 64 bit java as well
                JavainstallPath = CommonFunctions.GetJava64bitInstallationPath();
                if (JavainstallPath != null)
                {
                    string JabswitchfilePath = System.IO.Path.Combine(JavainstallPath, "bin\\jabswitch.exe");
                    ProcessStartInfo _info = new ProcessStartInfo();
                    _info.FileName = JabswitchfilePath;
                    _info.Arguments = argument;
                    _info.UseShellExecute = false;
                    Process proc = Process.Start(_info);
                    proc.WaitForExit();
                }
            }
            catch
            {
            }
        }
    
    }
}
