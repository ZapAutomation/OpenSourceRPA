using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Security;
using System.Xml.Serialization;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Aggregator;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Helpers;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Serializable]
    [Description("Launch The Application")]
    public class LaunchApplicationAction : AggregatedAction
    {
        private string alternateFileName;
        private string arguments;
        private string domain;
        private string fileName;
        private EncryptionInformation password;
        private bool runAsAdmin;
        private string userName;

        public LaunchApplicationAction()
        {
        }

        public LaunchApplicationAction(string fileName)
        {
            FileName = fileName;
        }

        public LaunchApplicationAction(string fileName, string arguments) : this(fileName)
        {
            Arguments = arguments;
        }

        internal override string GetParameterString()
        {
            object[] args = { FileName, Arguments };
            return string.Format(CultureInfo.InvariantCulture, "\"{0}\" {1}", args);
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ApplicationUnderTask test;

            if (string.IsNullOrEmpty(this.UserName))
            {
                test = ApplicationUnderTask.Launch(this.FileName, this.AlternateFileName, this.Arguments);
            }
            else
            {
                using (SecureString str = UIActionInterpreter.StringToSecureString(this.PasswordText))
                {
                    test = ApplicationUnderTask.Launch(this.FileName, this.AlternateFileName, this.Arguments, this.UserName, str, this.Domain);
                }
            }
            test.CloseOnPlaybackCleanup = false;
            if (!string.IsNullOrEmpty(this.TaskActivityIdentifier) && test.Process.Id != 0 && test.Process.MainWindowHandle != IntPtr.Zero)
            {
                TaskActivityObject uIObjectFromUIObjectId = WindowIdentifier.GetUIObjectFromUIObjectId(this.TaskActivityIdentifier);
                test.SessionId = uIObjectFromUIObjectId.SessionId;
                SearchHelper.Instance.UpdateTopLevelElementCache(new ZappyTaskControlSearchArgument(test), test);
                UIActionInterpreter.UpdateControlsCache(this.TaskActivityIdentifier, test);
            }
            else
            {
                test.Dispose();
            }

        }

        public string AlternateFileName
        {
            get =>
                alternateFileName;
            set
            {
                alternateFileName = value;
                NotifyPropertyChanged("AlternateFileName");
            }
        }

        public string Arguments
        {
            get =>
                arguments;
            set
            {
                arguments = value;
                NotifyPropertyChanged("Arguments");
            }
        }

        public string Domain
        {
            get =>
                domain;
            set
            {
                domain = value;
                NotifyPropertyChanged("Domain");
            }
        }

        public string FileName
        {
            get =>
                fileName;
            set
            {
                ZappyTaskUtilities.CheckForNull(value, "value");
                fileName = value;
                NotifyPropertyChanged("FileName");
                AlternateFileName = EnviromentPathVariables.Transform(fileName);
            }
        }

        public EncryptionInformation Password
        {
            get =>
                password;
            set
            {
                password = value;
                NotifyPropertyChanged("Password");
            }
        }

        internal string PasswordText
        {
            get
            {
                if (Password.Encoded)
                {
                    try
                    {
                        return EncodeDecode.DecodeString(Password.Value);
                    }
                                                                                                                        catch (FileNotFoundException)
                    {
                        Password.Encoded = false;
                        throw;
                    }
                }
                return Password.Value;
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public bool RunAsAdmin
        {
            get =>
                runAsAdmin;
            set
            {
                runAsAdmin = value;
                NotifyPropertyChanged("RunAsAdmin");
            }
        }

        [XmlElement(ElementName = "RunAsAdmin")]
        public string RunAsAdminWrapper
        {
            get
            {
                if (RunAsAdmin)
                {
                    return bool.TrueString;
                }
                return null;
            }
            set
            {
                bool flag;
                RunAsAdmin = false;
                if (bool.TryParse(value, out flag) & flag)
                {
                    RunAsAdmin = true;
                }
                NotifyPropertyChanged("RunAsAdminWrapper");
            }
        }

        public string UserName
        {
            get =>
                userName;
            set
            {
                userName = value;
                NotifyPropertyChanged("UserName");
            }
        }

        private static class EnviromentPathVariables
        {
            private static string[] allDrives = InitializeDriveNames();
            private static KeyValuePair<string, string>[] pathVariables = InitializeEnvironmentPathVariables();

            private static int CompareLengthOfValue(KeyValuePair<string, string> left, KeyValuePair<string, string> right) =>
                right.Value.Length - left.Value.Length;

            private static string[] InitializeDriveNames()
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                string[] strArray = new string[drives.Length];
                for (int i = 0; i < drives.Length; i++)
                {
                    strArray[i] = drives[i].Name;
                }
                return strArray;
            }

            private static KeyValuePair<string, string>[] InitializeEnvironmentPathVariables()
            {
                List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
                {
                    if (entry.Key != null && entry.Value != null)
                    {
                        string key = entry.Key.ToString().Trim();
                        string str2 = entry.Value.ToString().Trim();
                        if (IsValidEnvironmentPathVariable(key, str2))
                        {
                            object[] args = { key };
                            key = string.Format(CultureInfo.InvariantCulture, "%{0}%", args);
                            list.Add(new KeyValuePair<string, string>(key, str2));
                        }
                    }
                }
                list.Sort(CompareLengthOfValue);
                return list.ToArray();
            }

            private static bool IsValidEnvironmentPathVariable(string key, string value)
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && StartsWithDriveName(value))
                {
                    try
                    {
                        return string.Equals(value, Path.GetFullPath(value), StringComparison.OrdinalIgnoreCase) && Directory.Exists(value);
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (SecurityException)
                    {
                    }
                    catch (NotSupportedException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                }
                return false;
            }

            private static bool StartsWithDriveName(string value)
            {
                for (int i = 0; i < allDrives.Length; i++)
                {
                    if (value.StartsWith(allDrives[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }

            internal static string Transform(string filename)
            {
                string str = filename;
                if (!string.IsNullOrEmpty(filename))
                {
                    foreach (KeyValuePair<string, string> pair in pathVariables)
                    {
                        if (filename.StartsWith(pair.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            return pair.Key + filename.Substring(pair.Value.Length);
                        }
                    }
                }
                return str;
            }
        }
    }
}